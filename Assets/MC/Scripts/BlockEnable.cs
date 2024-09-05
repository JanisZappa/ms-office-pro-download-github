using System.Collections.Generic;
using UnityEngine;


public class BlockEnable : Singleton<BlockEnable>
{
    [Range(1, 6)]
    public int range;
    
    public bool likeCollider;
    
    private static Vector3Int pos = Vector3Int.one * -10000;
    
    private static readonly Dictionary<Vector3Int, GameObject> dict = new Dictionary<Vector3Int, GameObject>();
    private static readonly List<GameObject> active = new List<GameObject>(), 
                                                add = new List<GameObject>(), 
                                               keep = new List<GameObject>();
    
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gO = transform.GetChild(i).gameObject;
            string[] parts = gO.name.Replace("(","").Replace(")","").Split(',');
            Vector3Int v = new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            dict.Add(v, gO);
            gO.layer = MCLayers.Invisible;
        }
        
        FindObjectOfType<BatBlock>().BlockUpdate += BatBlockUpdate;
    }
    
    
    private static void BatBlockUpdate(Vector3Int block, Vector3Int colliderBlock)
    {
        if(Inst.likeCollider)
            block = colliderBlock;
        
        if(block != pos)
        {
            pos = block;
            
            add.Clear();
            keep.Clear();

            if (Inst.likeCollider)
            {
                for (int x = 0; x < 2; x++)
                for (int y = 0; y < 2; y++)
                for (int z = 0; z < 2; z++)
                {
                    Vector3Int checkPos = pos + new Vector3Int(x, y, z);
                    if (dict.ContainsKey(checkPos))
                    {
                        GameObject gO = dict[checkPos];
                        
                        if (active.Contains(gO))
                        {
                            keep.Add(gO);
                            active.Remove(gO);
                        }
                    
                        add.Add(gO);
                    }
                }
            }
            else
            {
                for (int x = -Inst.range; x < Inst.range + 1; x++)
                for (int y = -Inst.range; y < Inst.range + 1; y++)
                for (int z = -Inst.range; z < Inst.range + 1; z++)
                {
                    Vector3Int checkPos = pos + new Vector3Int(x, y, z);
                    if (dict.ContainsKey(checkPos))
                    {
                        GameObject gO = dict[checkPos];
                    
                        if (active.Contains(gO))
                        {
                            keep.Add(gO);
                            active.Remove(gO);
                        }
                    
                        add.Add(gO);
                    }
                }
            }
            
             
            int count = active.Count;
            for (int i = 0; i < count; i++)
                active[i].layer = MCLayers.Invisible;
            
            active.Clear();
            
            count = keep.Count;
            for (int i = 0; i < count; i++)
                active.Add(keep[i]);
            
            count = add.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject gO = add[i];
                gO.layer = 0;
                active.Add(gO);
            }
        }
    }
}
