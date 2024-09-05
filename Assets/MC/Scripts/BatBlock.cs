using System.IO;
using UnityEngine;


public class BatBlock : MonoBehaviour
{
    public bool draw;
    public TextAsset data;
    
    public delegate void BatBlockUpdate(Vector3Int block, Vector3Int colliderBlock);
    public event BatBlockUpdate BlockUpdate;
    
    public static float Blocks;
    private static Vector3 Offset;


    private void Start()
    {
        Offset = Vector3.zero;
        Blocks = 48;
        
        if(data != null)
            using (MemoryStream m = new MemoryStream(data.bytes))
            using (BinaryReader r = new BinaryReader(m))
            {
                Blocks = r.ReadInt32();
                //Debug.Log(Blocks);
                Offset = new Vector3(-r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) * 20;
            }
    }


    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        
        Vector3Int a = PosToBlock(pos);
        BlockUpdate?.Invoke(a, ColliderBlock(pos));
        
        
        if (draw)
        {
            Vector3Int b = a;
            Vector3 dir = pos - BlockCenter(a);
            b += new Vector3Int(dir.x > 0? -1 : 0, dir.y < 0? -1 : 0, dir.z < 0? -1 : 0);
            
            for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
            for (int z = 0; z < 2; z++)
                DrawBlock(b + new Vector3Int(x, y, z), Color.Lerp(Color.white, Color.yellow, Mathf.Max(x, Mathf.Max(y, z))).A(.5f));
        }    
    }


    private static void DrawBlock(Vector3Int a, Color color)
    {
        Vector3 p = BlockMin(a);
        float step = Blocks / 1;
        for (int x = 0; x < 1; x++)
        for (int y = 0; y < 1; y++)
        for (int z = 0; z < 1; z++)
        {
            Vector3 pp = p + new Vector3(x, y, z) * step;
            //DRAW.Box(pp, pp + Vector3.one * step).SetColor(color);
            DRAW.Box(pp + Vector3.one * .25f, pp + Vector3.one * (step - .25f)).SetColor(color);
        }
    }


    public static Vector3Int PosToBlock(Vector3 pos)
    {
        Vector3 p = (pos + Offset).MultiX(-1) + Vector3.one * .5f;
        return new Vector3Int(Mathf.FloorToInt(p.x / Blocks), Mathf.FloorToInt(p.y / Blocks), Mathf.FloorToInt(p.z / Blocks));
    }
    
    
    public static Vector3 BlockMin(Vector3Int pos)
    {
        pos.x = pos.x * -1 - 1;
        return new Vector3(pos.x * Blocks, pos.y * Blocks, pos.z * Blocks) - new Vector3(-.5f, .5f, .5f) - Offset;
    }
    
    
    public static Vector3 BlockCenter(Vector3Int pos)
    {
        return BlockMin(pos) + Vector3.one * Blocks * .5f;
    }


    public static Vector3Int ColliderBlock(Vector3 pos)
    {
        Vector3Int a = PosToBlock(pos);
        
        Vector3Int b = a;
        Vector3 dir = pos - BlockCenter(a);
        b += new Vector3Int(dir.x > 0? -1 : 0, dir.y < 0? -1 : 0, dir.z < 0? -1 : 0);
        
        return b;
    }
}
