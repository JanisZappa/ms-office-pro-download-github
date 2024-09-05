using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScatterMaster : MonoBehaviour
{
    public ComputeShader compute;

    [Space] public TextAsset doorData;
    
    public Mesh[] meshes;
    public Material mat;

    private int groupCount;
    
    private ComputeBuffer[] argsBuffer;
    private uint[][] args;

    private Material[] mats;
    
    private readonly Bounds worldBounds = new Bounds(Vector3.zero, Vector3.one * 100000);

    private int CollectKernel;
    [HideInInspector]public int[] renderCounter;
    private int[] counterReset;
    
    private ComputeBuffer renderBuffer, renderCountBuffer;
    private int total;
    
    
    public struct ScatterObject
    {
        private Vector3 pos, rot;

        public ScatterObject(Vector3 pos, Vector3 rot)
        {
            this.pos = pos;
            this.rot = rot;
        }
    }
    
    
    private void Start()
    {
        CollectKernel = compute.FindKernel("CollectKernel");

        ScatterObject[] objects;
        using (MemoryStream m = new MemoryStream(doorData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            total = r.ReadInt32();
            objects = new ScatterObject[total];
            for (int i = 0; i < total; i++)
            {
                r.ReadInt32();
                r.ReadInt32();
                
                Vector3 pos = r.HoudiniVector3(true);
                Vector3 rot = Hou.EulerRad(r.ReadInt32());
                objects[i] = new ScatterObject(pos, rot);
            }
        }
        
        renderBuffer = Buff.New(objects, 4 * 6);
        
        groupCount = meshes.Length;
        
        renderCounter = new int[groupCount];
        counterReset = new int[groupCount];
        renderCountBuffer = Buff.Add(new ComputeBuffer(groupCount, 4));
        renderCountBuffer.SetData(counterReset);
        
       
        
        args = new uint[groupCount][];
        argsBuffer = new ComputeBuffer[groupCount];
        
        mats = new Material[groupCount];
        
        int RenderBuffer = Shader.PropertyToID("RenderBuffer");
        for (int i = 0; i < groupCount; i++)
        {
            args[i] = new uint[5];
            argsBuffer[i] = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            mats[i] = Instantiate(mat);
            mats[i].SetBuffer(RenderBuffer, renderBuffer);
            Mesh mesh = meshes[i];
            uint[] a = args[i];
            a[0] = mesh.GetIndexCount(0);
            a[2] = mesh.GetIndexStart(0);
            a[3] = mesh.GetBaseVertex(0);
        }
    }

    public void ShowObjectsForRooms(int roomCount)
    {
        return;
        //compute.Dispatch(CollectKernel, tileCollectKernelSteps.x, tileCollectKernelSteps.y, 1);
        
        //renderCountBuffer.GetData(renderCounter);
        
        
        for (int i = 0; i < groupCount; i++)
        {
            uint[] a = args[i];
            a[1] = (uint)total;
            argsBuffer[i].SetData(a);
            
            Graphics.DrawMeshInstancedIndirect(
                meshes[i], 0, mats[i], worldBounds, argsBuffer[i], 0);
        }
    }
}
