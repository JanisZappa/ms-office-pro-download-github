using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoMath;
using UnityEngine;
using GridData;


public class GridLevel : MonoBehaviour
{
    public TextAsset posData;

    [Space] public int cellSize;
    
    private ScatterObject[] scatterObj;
    private int tileCount;


    private void Start()
    {
        using (MemoryStream m = new MemoryStream(posData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int count = r.ReadInt32();
            scatterObj = new ScatterObject[count];

            for (int i = 0; i < count; i++)
            {
                AddItem(new ScatterObject(
                    new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()), 
                    r.ReadInt32(), r.ReadSingle(), r.ReadSingle()), i);
            }

            for (int i = 0; i < count; i++)
                scatterObj[i].CreateCollider();
        }

        foreach (KeyValuePair<Vector2Int, List<int>> v in tiles)
        {
            int count = v.Value.Count;
            ScatterObject[] s = new ScatterObject[count];
            Bounds2D[] b = new Bounds2D[count];
            for (int i = 0; i < count; i++)
            {
                ScatterObject sO = scatterObj[v.Value[i]];
                s[i] = sO;
                b[i] = sO.GetBounds;
            }
                
            tileArrays.Add(v.Key, s);
            boundArrays.Add(v.Key, b);
        }
    }

    
    
    private void AddItem(ScatterObject scatterObject, int i)
    {
        Vector2Int tile = new Vector2Int(Mathf.FloorToInt(scatterObject.pos.x / cellSize), Mathf.FloorToInt(scatterObject.pos.z / cellSize));
        
        if (tiles.ContainsKey(tile))
            tiles[tile].Add(i);
        else
            tiles.Add(tile, new List<int>{i});

        scatterObj[i] = scatterObject;
    }


    private void OnEnable()
    {
        DynamicSprites.OnSpriteCollect += OnSpriteCollect;
        DebugUI.TL += DebugUiOnTl;
    }

    
    private void DebugUiOnTl(StringBuilder builder)
    {
        builder.AppendLine(tileCount.PrepString());
    }


    private void OnDisable()
    {
        DynamicSprites.OnSpriteCollect -= OnSpriteCollect;
    }
    

    private void OnSpriteCollect()
    {
        float multi = 1f / cellSize;
        int xmin   = Mathf.FloorToInt(GridCam.Bounds.minX * multi) - 1;
        int zmin   = Mathf.FloorToInt(GridCam.Bounds.minY * multi) - 1;
        int xcount = Mathf.FloorToInt(GridCam.Bounds.maxX * multi) + 2 - xmin;
        int zcount = Mathf.FloorToInt(GridCam.Bounds.maxY * multi) + 2 - zmin;

        for (int x = 0; x < xcount; x++)
        for (int z = 0; z < zcount; z++)
        {
            Vector2Int check = new Vector2Int(x + xmin, z + zmin);
            if (tileArrays.TryGetValue(check, out ScatterObject[] values))
                DynamicSprites.RenderThisTile(values, boundArrays[check]);
        }

        tileCount = xcount * zcount;
    }
    
    
    private static readonly Dictionary<Vector2Int,List<int>> tiles = new Dictionary<Vector2Int, List<int>>();
    private static readonly Dictionary<Vector2Int,ScatterObject[]> tileArrays = new Dictionary<Vector2Int, ScatterObject[]>();
    private static readonly Dictionary<Vector2Int,Bounds2D[]> boundArrays = new Dictionary<Vector2Int, Bounds2D[]>();
}
