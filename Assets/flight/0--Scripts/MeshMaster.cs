using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;


public class MeshMaster : Singleton<MeshMaster>
{
    private struct PosRot
    {
        private Vector3 pos;
        private Vector3 euler;
        private Vector2 anim;

        public PosRot(Vector3 pos, Vector3 euler)
        {
            this.pos   = pos;
            this.euler = euler * Mathf.Deg2Rad;
            this.anim   = Vector2.zero;
        }
        
        public PosRot(Vector3 pos, Vector3 euler, Vector2 anim)
        {
            this.pos   = pos;
            this.euler = euler * Mathf.Deg2Rad;
            this.anim  = anim;
        }
    }
    
    
    public Material mat, singleStaticMat, singleStaticTexMat;
    
    private const int maxCount   = 10 * SnesPixel.FrameCount;
    private const int maxObjects = 1000;
    private static readonly PosRot[] data = new PosRot[maxCount];
    private ComputeBuffer[] buffers;
    private Material[] materials;
    private static int drawIndex, objectIndex;

    private static readonly List<MeshFrame> all             = new List<MeshFrame>(1000);
    private static readonly List<MeshFrame> collect         = new List<MeshFrame>(1000);
    private static readonly List<MeshFrame> staticSingle    = new List<MeshFrame>(1000);
    private static readonly List<MeshFrame> staticSingleTex = new List<MeshFrame>(1000);
    
    private readonly Bounds worldBounds = new Bounds(Vector3.zero, Vector3.one * 100000);
    
    private ComputeBuffer[] argsBuffer;
    private static readonly int buffer1 = Shader.PropertyToID("_Buffer");
    private readonly uint[] args = new uint[5];
    
    
    private static readonly Matrix4x4[] camMatrixes = new Matrix4x4[SnesPixel.FrameCount];
    private ComputeBuffer camMX;
    private FlightCam cam;
    private static readonly int camBuffer = Shader.PropertyToID("CamBuffer");


    private void Start()
    {
        materials = new Material[maxObjects];
        buffers = new ComputeBuffer[maxObjects];
        argsBuffer = new ComputeBuffer[maxObjects];
        for (int i = 0; i < maxObjects; i++)
        {
            ComputeBuffer bf = Buff.New(maxCount, Marshal.SizeOf(typeof(PosRot)));
            Material m = Instantiate(mat);
            m.SetBuffer(buffer1, bf);
            materials[i] = m;
            buffers[i] = bf;
            argsBuffer[i] = Buff.New(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        }
        
        cam = FindObjectOfType<FlightCam>();
        
        camMX = Buff.New(SnesPixel.FrameCount, Marshal.SizeOf(typeof(Matrix4x4)));
        Shader.SetGlobalBuffer(camBuffer, camMX);
    }


    public static void Add(MeshFrame meshFrames)
    {
        switch (meshFrames.type)
        {
            default:                             all.Add(meshFrames); break;
            case MeshType.StaticSingle: staticSingle.Add(meshFrames); break;
            case MeshType.StaticSingleTex: staticSingleTex.Add(meshFrames); break;
        }
    }
    
    
    public static void Remove(MeshFrame meshFrames)
    {
        switch (meshFrames.type)
        {
            default:                             all.Remove(meshFrames); break;
            case MeshType.StaticSingle: staticSingle.Remove(meshFrames); break;
        }
    }


    private void LateUpdate()
    {
        MeshFrame.Step = Time.deltaTime / SnesPixel.FrameCount;
        
        cam.PrepareFrames();
        camMX.SetData(camMatrixes);
        
        
        int count = all.Count;
        
        objectIndex = 0;
        collect.Clear();
        
        for (int i = 0; i < count; i++)
            collect.Add(all[i]);
        
        while (count > 0)
        {
            MeshFrame a = collect[count - 1];
            collect.RemoveAt(count - 1);
            count--;
            
            drawIndex = 0;
            a.GetFrames();
            

            for (int i = 0; i < count; i++)
            {
                int index = count - 1 - i;
                
                MeshFrame b = collect[index];

                if (b.mesh == a.mesh)
                {
                    collect.RemoveAt(index);
                    count--;
                    i--;
                    b.GetFrames();

                    if (drawIndex == maxCount)
                    {
                        DrawMesh(a.mesh);
                        goto StartNewBuffer;
                    }
                }
            }
            
            DrawMesh(a.mesh);
            
            StartNewBuffer:;
        }
        
    //  Static Single
        count = staticSingle.Count;
        for (int i = 0; i < count; i++)
            DrawSingleMeshStatic(staticSingle[i].mesh, false);
        
    //  Static SingleTex
        count = staticSingleTex.Count;
        for (int i = 0; i < count; i++)
            DrawSingleMeshStatic(staticSingleTex[i].mesh, true);
    }


    private void DrawMesh(Mesh mesh)
    {
        //Debug.LogFormat("Drawing {0} {1} Times.{2}", mesh.name, drawIndex / 30, maxxedOut? " Maxxed" : "");
        
        args[0] = mesh.GetIndexCount(0);
        args[1] = (uint)drawIndex;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        
        ComputeBuffer argB = argsBuffer[objectIndex];
        argB.SetData(args);
        
        buffers[objectIndex].SetData(data);
                        
        Graphics.DrawMeshInstancedIndirect(mesh, 0, materials[objectIndex], worldBounds, argB, 0);
        objectIndex++;
    }
    
    
    private void DrawSingleMeshStatic(Mesh mesh, bool tex)
    {     
        args[0] = mesh.GetIndexCount(0);
        args[1] = 30;
        args[2] = mesh.GetIndexStart(0);
        args[3] = mesh.GetBaseVertex(0);
        
        ComputeBuffer argB = argsBuffer[objectIndex];
        argB.SetData(args);
        
        Graphics.DrawMeshInstancedIndirect(mesh, 0, tex? singleStaticTexMat : singleStaticMat, worldBounds, argB, 0);
        objectIndex++;
    }


    public static void SetFrame(Placement placement)
    {
        Profiler.BeginSample("PosRot");
        
        data[drawIndex++] = new PosRot(placement.pos, placement.rot.eulerAngles);
        
        Profiler.EndSample();
    }
    
    public static void SetFrame(Placement placement, Vector2 anim)
    {
        Profiler.BeginSample("PosRot");
        
        data[drawIndex++] = new PosRot(placement.pos, placement.rot.eulerAngles, anim);
        
        Profiler.EndSample();
    }


    public static void SetCamOffsets(int frame, Placement plc)
    {
        Matrix4x4 r = Matrix4x4.Rotate(plc.rot);
        Vector3 p = plc.rot * plc.pos;
        r.m03 = p.x;
        r.m13 = p.y;
        r.m23 = p.z;
        
        camMatrixes[frame] = r;
    }
}
