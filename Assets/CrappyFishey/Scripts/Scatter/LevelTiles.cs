using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public class LevelTiles : MonoBehaviour
{
    public TextAsset tileData, dirData;
    public ComputeShader compute;
    public int editorDiv, buildDiv;
    
    [Space]
    public ScatterTest scatterTest;
    
    
    private static int[] tileState, stateRead;
    
    public static int SubDivisions;
    
    private class LevelBlock
    {
        private readonly int id;
        private readonly MeshFilter mF;
        private readonly MeshRenderer mR;
        private readonly Mesh[] meshes;
        private int pick = -1;
        private bool vis;

        
        public LevelBlock(int id, MeshFilter mF, Mesh[] meshes)
        {
            this.id = id;
            this.mF = mF;
            this.meshes = meshes;

            for (int i = 0; i < meshes.Length; i++)
                meshes[i].UploadMeshData(true);
            
            mR = mF.GetComponent<MeshRenderer>();
            mR.enabled = false;
        }
      

        public void TileUpdate()
        {
            int value = hideGeo? -1 : tileState[id];
       
            bool newVis = value >= 0;
            if (value != pick)
            {
                pick = value;
                if (vis != newVis)
                {
                    vis = newVis;
                    mR.enabled = vis;
                }
            
                if (vis)
                    mF.mesh = meshes[pick];
            }

            if (vis)
                visibleTiles++;
        }
    }
    
    private LevelBlock[] blocks;
    private int count;
    
    private Transform cam;
    
    private readonly Dictionary<Vector3Int, int> tileMap = new Dictionary<Vector3Int, int>();
    private static  int lastTile = -1, lastDir = -1;
    private static sbyte[] tileVis;
    private static Vector3Int[] coords;
    private static int tileCount;
    
    private Vector3[] dirs;
    private static int totalDirs, dirSteps, dirStepHalf, dirCount;
    private static bool[] dirVis;
    private static Vector3Int playerPos;
    
    private static int visibleTiles;
    
    private static bool hideGeo;
    
    private int TileCheck;
    private ComputeBuffer tileArgs, tileVisBuffer;
    

    private void Start()
    {
    //  Parse Data  //
        Check.It();
        
        int[] tileVisData;
        Vector2Int[] tileVisMap;
        using (MemoryStream m = new MemoryStream(tileData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            tileCount = r.ReadInt32();
            //Debug.Log(tileCount);
            tileVis = new sbyte[tileCount * tileCount];
            coords = new Vector3Int[tileCount];
            
            List<int> dataCollect = new List<int>();
            tileVisMap = new Vector2Int[tileCount];
            
            for (int i = 0; i < tileCount; i++)
            {
                Vector3Int coord = new Vector3Int(-r.ReadInt32(), r.ReadInt32(), r.ReadInt32());
                coords[i] = coord;
                tileMap.Add(coord, i * tileCount);

                int before = dataCollect.Count;
                for (int j = 0; j < tileCount; j++)
                {
                    sbyte value = r.ReadSByte();
                    tileVis[i * tileCount + j] = value;
                    if(value >= 0)
                        dataCollect.Add(j);
                }
                    
                tileVisMap[i] = new Vector2Int(before, dataCollect.Count - before);
            }
            
            tileVisData = dataCollect.ToArray();
        }
        
        
        if(Application.isEditor)
            Debug.LogFormat("Parsed VisData in {0} Seconds", Check.It());


        SubDivisions = Application.isEditor? editorDiv : buildDiv;
        
        cam = Camera.main.transform;
        
        Check.It();
        MeshFilter[] mF = GetComponentsInChildren<MeshFilter>();
        count  = mF.Length;
        blocks = new LevelBlock[count];
        
        
        for (int i = 0; i < count; i++)
        {
            MeshFilter m = mF[i];
            Mesh mesh = m.mesh;
            List<Mesh> meshes = MeshDivider.SubdivideVertMerge(mesh, SubDivisions);
            blocks[i] = new LevelBlock(int.Parse(m.name), m, meshes.ToArray());
        }
        
        Debug.LogFormat("Subdivided {0} times for {1} Seconds", SubDivisions, Check.It());
        
        
    //  Dirs  //
        int[] dirVisData;
        using (MemoryStream m = new MemoryStream(dirData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            dirSteps = r.ReadInt32();
            dirCount = r.ReadInt32();
            totalDirs = dirSteps * dirSteps * dirSteps;
            dirs = new Vector3[dirCount];
            for (int i = 0; i < dirCount; i++)
                dirs[i] = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
            
            int total = totalDirs * dirCount;
            dirStepHalf = (dirSteps - 1) / 2;
            dirVis = new bool[total];
            dirVisData = new int[total];

            for (int i = 0; i < total; i++)
            {
                sbyte value = r.ReadSByte();
                dirVis[i] = value == 1;
                dirVisData[i] = value;
            }
                
        }
        
        tileState = new int[tileCount];
        stateRead = new int[tileCount];
        
    //  Compute  //
        TileCheck = compute.FindKernel("TileCheck");
        
        tileVisBuffer = Buff.New(tileState.Length, 4);
        tileArgs      = Buff.Add(compute.SetupIndirect(tileCount, "tileX"));
        
        compute.SetBuffer(TileCheck, "tileVis", tileVisBuffer);
        compute.SetBuffer(TileCheck, "visData", Buff.New(tileVisData, 4));
        compute.SetBuffer(TileCheck, "mapData", Buff.New(tileVisMap, 8));
        compute.SetBuffer(TileCheck, "tilePos", Buff.New(coords, 3 * 4));
        compute.SetBuffer(TileCheck, "dirVisData", Buff.New(dirVisData, 4));
        
        compute.SetInt("dirStepHalf", dirStepHalf);
        compute.SetInt("dirSteps", dirSteps);
        compute.SetInt("tileCount", tileCount);
        compute.SetInt("SubDivisions", SubDivisions);
        
        scatterTest.ScatterSetup(tileVisBuffer);
    }


    private void LateUpdate()
    {
        Vector3 pos = cam.position;
        Vector3 f = cam.forward;
                f.x *= 1;
        
        bool shouldUpdate = false;
        
        playerPos = new Vector3Int(Mathf.RoundToInt(pos.x * .5f), 
                                   Mathf.RoundToInt(pos.y * .5f), 
                                   Mathf.RoundToInt(pos.z * .5f));
        
        int newTile = tileMap.ContainsKey(playerPos)? tileMap[playerPos] : lastTile;
        if (newTile != lastTile)
        {
            lastTile = newTile;
            shouldUpdate = true;
        }   
        
        
        int newDir = -2;
        int dircount = dirs.Length;
        float bestDot = -2;
        for (int i = 0; i < dircount; i++)
        {
            float d = Vector3.Dot(f, dirs[i]);
            if (d > bestDot)
            {
                bestDot = d;
                newDir = i;
            }
        }
        newDir *= totalDirs;

        
        if (lastDir != newDir)
        {
            lastDir = newDir;
            shouldUpdate = true;
        }

        if (Input.GetKeyDown(Keys.ü))
        {
            shouldUpdate = true;
            hideGeo = !hideGeo;
        }
            
        

        if (shouldUpdate)
        {
            compute.SetInt("playerTile", lastTile / tileCount);
            compute.SetInt("playerDir", lastDir);
            compute.SetInt("pPosX", playerPos.x);
            compute.SetInt("pPosY", playerPos.y);
            compute.SetInt("pPosZ", playerPos.z);
            
            compute.DispatchIndirect(TileCheck, tileArgs);

            scatterTest.ScatterUpdate();
            
            /*for (int i = 0; i < tileCount; i++)
            {
                int value = tileVis[lastTile + i];
                
                int checkValue = CoordValue(coords[i]);
                if(value != -1 && checkValue != value)
                    Debug.Log(value + " .. " + checkValue);
                
                if(value != -1 && !DirVisible(coords[i]))
                    value = -1;
                tileState[i] = value >= 0? Mathf.Max(SubDivisions - Mathf.Max(value - 1, 0), 0) : -1;
            }*/


            if (!hideGeo)
            {
                //Check.It();
                tileVisBuffer.GetData(stateRead);
                //Check.It("Vis Buffer Get");
                
                for (int i = 0; i < tileCount; i++)
                    tileState[i] = stateRead[i];
            }
           
            visibleTiles = 0;
            for (int i = 0; i < count; i++)
                blocks[i].TileUpdate();
        }
    }


    /*private static int CoordValue(Vector3Int tileCoords)
    {
        Vector3Int offset = tileCoords - playerPos;
        return Mathf.Max(Mathf.Abs(offset.x), Mathf.Max(Mathf.Abs(offset.y), Mathf.Abs(offset.z)));
    }


    private static bool DirVisible(Vector3Int tileCoords)
    {
        Vector3Int offset = tileCoords - playerPos;
        
        if(Mathf.Abs(offset.x) > dirStepHalf || 
           Mathf.Abs(offset.y) > dirStepHalf || 
           Mathf.Abs(offset.z) > dirStepHalf)
            return false;

        offset += new Vector3Int(dirStepHalf, dirStepHalf, dirStepHalf);
        int visIndex = offset.x  + offset.y * dirSteps + offset.z * (dirSteps * dirSteps);
        return dirVis[lastDir + visIndex];
    }*/


    public static Vector3 GetTilePos(int tile)
    {
        return coords[tile] * 2;
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
       return;
       builder.AppendLine(visibleTiles + " Tiles");
    }
    #endregion
}
