using System.IO;
using System.Text;
using UnityEngine;

public class ScatterTest : MonoBehaviour
{
    public TextAsset data;
    public Mesh[] meshes;
    public Material mat;
    
    [Space]
    public ComputeShader compute;
    
    [Space]
    public RenderThing[] things;
    
    private ComputeBuffer objArgs, ptsArgs, renderArgs, 
                         renderArgsBuffer, counterReadBuffer;
    
    private uint[] kernelArgs;
    
    private int CheckPts, CollectObj, ResetObj, RenderReset, RenderRead;
    private int typeCount;
   
    private static readonly int rend   = Shader.PropertyToID("Render"), 
                                map    = Shader.PropertyToID("Map"),
                                Offset = Shader.PropertyToID("_Offset");
    
    private bool stopIt, onlyImportant;
    private string debugTotal;
    private Vector3 solveTime, solves;
    
    private int maxValueLength;
    
    private Transform camT;
    private Camera cam;
    private readonly Plane[] planes = new Plane[6];
   
    private int[] counterRead;

    [System.Serializable]
    public class RenderThing
    {
        static RenderThing() { world = new Bounds(Vector3.zero, Vector3.one * 10000); }
        
        private static readonly Bounds world;
        
        public Mesh mesh;
        public Material mat;
        public int offset, total;
        private readonly int triangles;
        
        public readonly string debugName;
        
        public readonly bool important;


        public RenderThing(Mesh mesh, Material sourceMat, Vector2Int data, string debugName, int maxValueLength)
        {
            this.mesh  = mesh;
            total  = data.x;
            offset = data.y;
            
            mat = Instantiate(sourceMat);
            mat.SetInt(Offset, offset);
            
            this.debugName = " | " + total.ToString().PadLeft(maxValueLength)  + " | " + debugName;
            
            triangles = (int)mesh.GetIndexCount(0) / 3;
            
            important = debugName.Contains("Board");
        }


        public RenderThing ArgsSetup(uint[] args, int argsOffset)
        {
            mesh.AddRenderArgs(args, argsOffset);
            return this;
        }


        public RenderThing SetBuffer(ComputeBuffer allM, ComputeBuffer mapBuffer)
        {
            mat.SetBuffer(rend, allM);
            mat.SetBuffer(map, mapBuffer);
            
            return this;
        }
        

        public void Render(ComputeBuffer renderArgs, int argsOffset, bool onlyImportant)
        {
            if(!onlyImportant || important)
                Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, world, renderArgs, argsOffset * 20);
        }


        public int GetTriangles(int active)
        {
            return triangles * active;
        }
    }


    public void ScatterSetup(ComputeBuffer tileVisBuffer)
    {
        Check.It();
        
        cam = Camera.main;
        camT = cam.transform;
        
        Vector2Int[] allData;
        Matrix4x4[] allMats;
        Vector2Int[] scatterData;
        Vector4[] allPos;
        int[] scatterTypes;
        int objCount = 0;
        
        int ptCount;
        Vector2Int[] ptData;
        
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int scatterTypeCount = r.ReadInt32();
            scatterData = new Vector2Int[scatterTypeCount];
            scatterTypes = new int[scatterTypeCount];
            
            for (int i = 0; i < scatterTypeCount; i++)
            {
                int type  = r.ReadInt16();
                int count = r.ReadInt32();
                
                scatterData[i] = new Vector2Int(count, 0);
                
                objCount += count;
                
                scatterTypes[i] = type;
            }
            
            float[] typeRadii = new float[scatterTypeCount];
            for (int i = 0; i < scatterTypeCount; i++)
            {
                Bounds b = meshes[scatterTypes[i]].bounds;
                typeRadii[i] = (b.min - b.center).magnitude;
            }
            
            allPos = new Vector4[objCount];
                
            int index = 0;
            allData = new Vector2Int[objCount];
            allMats  = new Matrix4x4[objCount];
            int offset = 0;
            bool crunch = r.ReadByte() == 1;
            for (int i = 0; i < scatterTypeCount; i++)
            {
                Vector2Int tD = scatterData[i];
                int count = tD.x;
                float radius = -typeRadii[i];

                for (int e = 0; e < count; e++)
                {
                    int tile = r.ReadInt16();
                    allData[index] = new Vector2Int(i, offset);
                    
                    Matrix4x4 matrix;
                    if(crunch)
                        matrix = Matrix4x4.TRS(
                            LevelTiles.GetTilePos(tile) + Hou.Pos(r.ReadInt32()),
                            Hou.Rot(r.ReadInt32()),
                            Vector3.one);
                    else
                        matrix = Matrix4x4.TRS(
                            new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()), 
                            new Quaternion(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle()), 
                            //Quaternion.Euler(new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) * Mathf.Rad2Deg), 
                            Vector3.one);
                    
                    allMats[index] = matrix;
                    
                    Vector3 p = matrix.MultiplyPoint(Vector3.zero);
                    allPos[index] = new Vector4(p.x, p.y, p.z, radius);
                    
                    index++;
                }
                
                tD.y = offset;
                scatterData[i] = tD;
                offset += count;
            }
            
            ptCount = r.ReadInt32();
            ptData = new Vector2Int[ptCount];
            for (int i = 0; i < ptCount; i++)
                ptData[i] = new Vector2Int(r.ReadInt16(), r.ReadInt32());
        }
        
        ComputeBuffer allM      = Buff.New(allMats, 16 * 4);
        ComputeBuffer mapBuffer = Buff.New(objCount, 4);
        
    //  Collect Meshes  //
        typeCount = scatterTypes.Length;
        
        Mesh[] useMeshes = new Mesh[typeCount];
        for (int i = 0; i < typeCount; i++)
            useMeshes[i] = meshes[scatterTypes[i]];
        
    //  Names for Debugging  //
        string[] names = new string[typeCount];
        for (int i = 0; i < typeCount; i++)
            names[i] = useMeshes[i].name;
        
        int maxName = names.PadLeft();
        
        maxValueLength = objCount.ToString().Length;
        debugTotal = " | " + objCount.ToString().PadLeft(maxValueLength) + " | " + "Total".PadLeft(maxName);
        
        uint[] args = new uint[typeCount * 5];
        
        things = new RenderThing[typeCount];
        for (int i = 0; i < typeCount; i++)
            things[i] = new RenderThing(useMeshes[i], mat, scatterData[i], names[i], maxValueLength).
                SetBuffer(allM, mapBuffer).
                ArgsSetup(args, i);
        
        renderArgsBuffer = Buff.New(args, 4, ComputeBufferType.IndirectArguments);
       
        counterRead = new int[typeCount];
        counterReadBuffer = Buff.New(typeCount, 4);
        
        CheckPts    = compute.FindKernel("CheckPts");
        CollectObj  = compute.FindKernel("CollectObj");
        ResetObj    = compute.FindKernel("ResetObj");
        RenderReset = compute.FindKernel("RenderReset");
        RenderRead  = compute.FindKernel("RenderRead");
        
        compute.SetBuffer(CheckPts, "ptData",  Buff.New(ptData, 2 * 4));
        compute.SetBuffer(CheckPts, "tileVis", tileVisBuffer);
        compute.SetMultiBuffer("Found",      Buff.New(objCount, 4), CheckPts, CollectObj, ResetObj);
        compute.SetMultiBuffer("RenderArgs", renderArgsBuffer, CollectObj, RenderReset, RenderRead);
        compute.SetBuffer(CollectObj, "objData", Buff.New(allData, 2 * 4));
        compute.SetBuffer(CollectObj, "Map",     mapBuffer);
        compute.SetBuffer(CollectObj, "Pos",     Buff.New(allPos, 4 * 4));
        
        compute.SetBuffer(RenderRead, "RenderCounter", counterReadBuffer);
        
        compute.SetInt("objCount",  objCount);
        compute.SetInt("ptCount",   ptCount);
        compute.SetInt("typeCount", typeCount);
        compute.SetInt("SubDivisions", LevelTiles.SubDivisions);
        
        objArgs    = Buff.Add(compute.SetupIndirect(objCount,  "objX"));
        ptsArgs    = Buff.Add(compute.SetupIndirect(ptCount,   "ptX"));
        renderArgs = Buff.Add(compute.SetupIndirect(typeCount, "renderX"));
        
        Check.It("Scatter Setup");
    }

    
    public void ScatterUpdate()
    {
        if(stopIt)
            return;
        
        Check.It();
        
        compute.DispatchIndirect(ResetObj, objArgs);
        
        compute.DispatchIndirect(CheckPts, ptsArgs);
        
        solves.x++;
        solveTime.x += Check.It();
    }


    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.P))
            stopIt = !stopIt;
        
        if(Input.GetKeyDown(Keys.ö))
            onlyImportant = !onlyImportant;

        if (!stopIt)
        {
            Check.It();
            compute.SetVector("CamP", camT.position);
            compute.SetVector("CamF", camT.forward);
            
            GeometryUtility.CalculateFrustumPlanes(cam, planes);
            compute.SetVector("CamA", planes[0].normal);
            compute.SetVector("CamB", planes[1].normal);
            compute.SetVector("CamC", planes[2].normal);
            compute.SetVector("CamD", planes[3].normal);
            
            
            compute.DispatchIndirect(RenderReset, renderArgs);
            compute.DispatchIndirect(CollectObj,  objArgs);
            
            solves.y++;
            solveTime.y += Check.It();
            
            for (int i = 0; i < typeCount; i++)
                things[i].Render(renderArgsBuffer, i, onlyImportant);
            
            solves.z++;
            solveTime.z += Check.It();
        }
    }
    
    
    #region Debug
    
    protected void OnEnable()
    {
        DebugUI.BR += OnDebugUI;
    }
    protected void OnDisable()
    {
        DebugUI.BR -= OnDebugUI;
    }
    
    private void OnDebugUI(StringBuilder builder)
    {
        compute.DispatchIndirect(RenderRead, renderArgs);
        counterReadBuffer.GetData(counterRead);
        
        int active = 0;
        int triCount = 0;
        for (int i = 0; i < typeCount; i++)
        {
            RenderThing thing = things[i];
            int a = counterRead[i];
            
            if (a > 0 && (!onlyImportant || thing.important))
            {
                int tris = thing.GetTriangles(a);
                builder.AppendLine(tris + " | " + 
                                   (a > thing.total? "!!! " : "") 
                                   + a.ToString().PadLeft(maxValueLength) 
                                   + thing.debugName);
                
                active += a;
                triCount += tris;
            }
        }
        
        builder.AppendLine(triCount + " | " + active.ToString().PadLeft(maxValueLength)  + debugTotal);
        
        Vector3 s = solveTime.Divide(solves);
        builder.AppendLine(s.x.ToString("F5") + " - " + s.y.ToString("F5") + " - " + s.z.ToString("F5"));
    }
    #endregion
}
