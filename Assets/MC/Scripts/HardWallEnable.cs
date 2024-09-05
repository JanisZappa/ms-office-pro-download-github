using System.Collections.Generic;
using UnityEngine;


public class HardWallEnable : MonoBehaviour
{
    private static Vector3Int pos = Vector3Int.one * -10000;
    
    private static readonly Dictionary<Vector3Int, GameObject> dict = new Dictionary<Vector3Int, GameObject>();
    private static readonly List<GameObject> active = new List<GameObject>();
    
    private static readonly Collider[] nonAlloc = new Collider[100];
    
    
    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gO = transform.GetChild(i).gameObject;
            string[] parts = gO.name.Replace("(","").Replace(")","").Split(',');
            Vector3Int v = new Vector3Int(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            dict.Add(v, gO);
            //gO.layer = MCLayers.Invisible;
        }
    }

    
    public static int BatBlockUpdate(Vector3 batPos, float radius)
    {
        Vector3Int colliderBlock = BatBlock.ColliderBlock(batPos);
        
        if(colliderBlock != pos)
        {
            int aCount = active.Count;
            for (int i = 0; i < aCount; i++)
                active[i].layer = 0;
            
            active.Clear();
            
            
            pos = colliderBlock;
            
            for (int x = 0; x < 2; x++)
            for (int y = 0; y < 2; y++)
            for (int z = 0; z < 2; z++)
            {
                Vector3Int checkPos = pos + new Vector3Int(x, y, z);
                if (dict.ContainsKey(checkPos))
                {
                    GameObject gO = dict[checkPos];
                    active.Add(gO);
                }
            }
        }

        {
            int aCount = active.Count;
            for (int i = 0; i < aCount; i++)
                active[i].layer = MCLayers.HardWall;
            int fCount = Physics.OverlapSphereNonAlloc(batPos, radius, nonAlloc, MCLayers.Mask_Hard);
            for (int i = 0; i < aCount; i++)
            {
                bool yes = false;
                GameObject gO = active[i];

                for (int e = 0; e < fCount; e++)
                    if (nonAlloc[e].gameObject == gO)
                    {
                        yes = true;
                        break;
                    }

                gO.layer = yes ? MCLayers.HardWall : 0;
            }
            
            return fCount;
        } 
    }


    private void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gO = transform.GetChild(i).gameObject;
            if(gO.GetComponent<MeshCollider>() != null)
                continue;
            
            MeshRenderer mR = gO.GetComponent<MeshRenderer>();
            MeshFilter   mF = gO.GetComponent<MeshFilter>();
            gO.AddComponent<MeshCollider>().sharedMesh = mF.sharedMesh;
            
            DestroyImmediate(mR);
            DestroyImmediate(mF);
        }
    }
}
