using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class WaterVolume : MonoBehaviour
{
    public ComputeShader compute;
    
    [Space]
    public float maxDist;
    
    private float cell;
    private const int cells = 64;
    private const int cellCount = cells * cells * cells;
    private Vector3 cellMin;
    private float size;
    
    [Space]
    public WaterColor[] waterColors;
    
    private Transform t;
    private static readonly int Fog       = Shader.PropertyToID("_Fog");
    private static readonly int Min       = Shader.PropertyToID("_Min");
    private static readonly int VolumeTex = Shader.PropertyToID("_VolumeTex");
    private static readonly int Size      = Shader.PropertyToID("_Size");
    
    
    private ComputeBuffer colors;
    private RenderTexture tex, tex2;
    
    private int PaintLocalK, RaycastLocalK;
    
    private ComputeBuffer kernelBuffer;
    private Camera cam;
    
    private Matrix4x4 ToBox, FromBox;
    private static readonly int toBox = Shader.PropertyToID("_ToBox");


    [Serializable]
    public struct WaterColor
    {
        public Color color;
        public float density;

        public Vector4 Col(bool multi = false)
        {
            return new Vector4(color.r, color.g, color.b, density * (multi? 256f / cells : 1));
        }
    }


    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.depthTextureMode = DepthTextureMode.Depth;
        
        t = transform;
        
        PaintLocalK   = compute.FindKernel("PaintLocal");
        RaycastLocalK = compute.FindKernel("RaycastLocal");
        
        tex = new RenderTexture(cells, cells, 0)
        {
            filterMode        = FilterMode.Bilinear, 
            wrapMode          = TextureWrapMode.Clamp, 
            enableRandomWrite = true, 
            dimension         = UnityEngine.Rendering.TextureDimension.Tex3D, 
            volumeDepth       = cells,
        };
        tex.Create();
        
        tex2 = new RenderTexture(cells, cells, 0)
        {
            filterMode        = FilterMode.Bilinear, 
            wrapMode          = TextureWrapMode.Clamp, 
            enableRandomWrite = true, 
            dimension         = UnityEngine.Rendering.TextureDimension.Tex3D, 
            volumeDepth       = cells,
        };
        tex2.Create();
        
        Shader.SetGlobalTexture(VolumeTex, tex2);
        compute.SetTexture(PaintLocalK,   "_VolumeTex", tex);
        compute.SetTexture(RaycastLocalK, "_VolumeTex", tex);
        compute.SetTexture(RaycastLocalK, "_ResultTex", tex2);
        
       
    //  BoxCalc  //
        Ray ray = cam.ViewportPointToRay(new Vector3(1, 1, 0));
        Vector3 rayPoint = ray.direction * maxDist;
        Vector3 bSize  = new Vector3(Vector3.Dot(rayPoint, t.right) * 2, 
                                     Vector3.Dot(rayPoint, t.up) * 2, 
                                     maxDist);
            
        size = Mathf.Max(bSize.x, Mathf.Max(bSize.y, bSize.z));
        
        Bounds b = new Bounds(Vector3.forward * size * .45f, Vector3.one * size);
        
        cell     = size / cells;
        cellMin  = b.min;
        
        Debug.Log(b.min + " " + b.size);
        
        Shader.SetGlobalFloat(Size, size);
        Shader.SetGlobalVector(Min, cellMin);
        
        Ray r1 = cam.ViewportPointToRay(Vector3.zero);
        Ray r2 = cam.ViewportPointToRay(Vector3.right);
        Debug.Log("Angle " + Vector3.Angle(r1.direction, r2.direction));
        
        compute.SetInt("_Cells",      cells);
        compute.SetVector("_Min",     cellMin);
        compute.SetFloat("_Size",     size);
        compute.SetFloat("_CellSize", cell);
        
        CalculateSampleInfos();
    }
    
    
    private void LateUpdate()
    {
        Shader.SetGlobalVector(Fog, waterColors[2].Col());
        
        compute.SetVector("_ColorA", waterColors[0].Col(true));
        compute.SetVector("_ColorB", waterColors[1].Col(true));
        
        compute.SetFloat("_Time", Time.realtimeSinceStartup);
        compute.SetFloat("_DT",   Time.deltaTime);
        
        ToBox   = t.worldToLocalMatrix;
        FromBox = t.localToWorldMatrix;
        
        compute.SetMatrix("FromBox", FromBox);
        
        Shader.SetGlobalMatrix(toBox, ToBox);
        
        Profiler.BeginSample("PaintLocalK");
        compute.DispatchIndirect(PaintLocalK,   kernelBuffer);
        //compute.Dispatch(PaintLocalK, kernelSteps.x, kernelSteps.y, kernelSteps.z);
        Profiler.EndSample();
        Profiler.BeginSample("RaycastLocalK");
        compute.DispatchIndirect(RaycastLocalK, kernelBuffer);
        //compute.Dispatch(RaycastLocalK, kernelSteps.x, kernelSteps.y, kernelSteps.z);
        Profiler.EndSample();
    }


    private void CalculateSampleInfos()
    {
        bool[] vis = new bool[cellCount];
        int visible = 0;
        
        const float xMax = 92.62f * .5f;
        const float yMax = 72f * .5f;
        for (int i = 0; i < cellCount; i++)
        {
            Vector3Int cellPos = new Vector3Int(i % cells, i / cells % cells, i / cells / cells % cells);
            Vector3 boxPos = cellMin + ((Vector3)cellPos * 1f / (cells - 1)) * size;
            
            bool xVis = Mathf.Acos(Vector3.Dot(new Vector3(boxPos.x, 0, boxPos.z).normalized, Vector3.forward)) * Mathf.Rad2Deg < xMax;
            bool yVis = Mathf.Acos(Vector3.Dot(new Vector3(0, boxPos.y, boxPos.z).normalized, Vector3.forward)) * Mathf.Rad2Deg < yMax;
            bool isVis = xVis && yVis;
            vis[i] = isVis;
            
            if(isVis)
                visible++;
        }

        const int pad = 3;
        bool[] padVis = vis.Copy();
        for (int i = 0; i < cellCount; i++)
            if (vis[i])
            {
                bool isOnEdge = false;
                
                Vector3Int cellPos = new Vector3Int(i % cells, i / cells % cells, i / cells / cells % cells);
                for (int x = -1; x < 2; x++)
                for (int y = -1; y < 2; y++)
                for (int z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3Int checkPos = cellPos + new Vector3Int(x, y, z);

                    if (checkPos.x < 0 || checkPos.y < 0 || checkPos.z < 0 ||
                        checkPos.x >= cells || checkPos.y >= cells || checkPos.z >= cells)
                        continue;

                    int checkIndex = checkPos.x + checkPos.y * cells + checkPos.z * (cells * cells);
                    if (!vis[checkIndex])
                    {
                        isOnEdge = true;
                        goto Fuck;
                    }
                }
                
                Fuck:
                
                if(!isOnEdge)
                    continue;
                
                for (int x = -pad; x < pad + 1; x++)
                for (int y = -pad; y < pad + 1; y++)
                for (int z = -pad; z < pad + 1; z++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    Vector3Int checkPos = cellPos + new Vector3Int(x, y, z);

                    if (checkPos.x < 0 || checkPos.y < 0 || checkPos.z < 0 ||
                        checkPos.x >= cells || checkPos.y >= cells || checkPos.z >= cells)
                        continue;

                    int checkIndex = checkPos.x + checkPos.y * cells + checkPos.z * (cells * cells);
                    padVis[checkIndex] = true;
                }
            }
        
        vis = padVis.Copy();

        int visibleNow = 0;
        for (int i = 0; i < cellCount; i++)
            if(vis[i])
                visibleNow++;
           
        Debug.LogFormat("Of {0} cells {1} are visible. With Padding {2}", cellCount, visible, visibleNow);
        
        List<SampleInfo> map = new List<SampleInfo>();
        for (int i = 0; i < cellCount; i++)
            if (vis[i])
            {
                Vector3Int cellPos = new Vector3Int(i % cells, i / cells % cells, i / cells / cells % cells);
                Vector3 boxPos = cellMin + ((Vector3)cellPos * 1f / (cells - 1)) * size;
                map.Add(new SampleInfo(cellPos, GetSteps(boxPos.magnitude), boxPos.normalized));
            }
        
        ComputeBuffer mapBuffer = new ComputeBuffer(map.Count, 8 * 4);
        mapBuffer.SetData(map.ToArray());
        
        compute.SetBuffer(PaintLocalK, "map", mapBuffer);
        compute.SetBuffer(RaycastLocalK, "map", mapBuffer);
        compute.SetInt("MapSteps", map.Count);
        
        
        kernelBuffer = new ComputeBuffer(2, 16, ComputeBufferType.IndirectArguments);
        int[] kernelArgs = new int[4 * 2];
        kernelArgs[0] = Mathf.CeilToInt(map.Count * 1f / 256);
        kernelArgs[1] = 1;
        kernelArgs[2] = 1;
        kernelBuffer.SetData(kernelArgs);
        
        int check = 10000;
        for (int i = 0; i < map.Count; i++)
            check = Mathf.Min(check, map[i].maxSteps);
        
        Debug.Log(check);
    }


    private int GetSteps(float objDist)
    {
        const float stepp = 1.0f / cells;
        return Mathf.Min(Mathf.CeilToInt(objDist / (stepp * size)), cells);
    }
    
    [Serializable]
    private struct SampleInfo
    {
        Vector3Int id;
        public int maxSteps;
        Vector3 dir;
        int dummy;

        public SampleInfo(Vector3Int id, int maxSteps, Vector3 dir)
        {
            this.id = id;
            this.maxSteps = maxSteps;
            this.dir = dir;
            dummy = 0;
        }
    }
}
