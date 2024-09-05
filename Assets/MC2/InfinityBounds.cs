using UnityEngine;


public class InfinityBounds : MonoBehaviour
{
    private void Start()
    {
        MeshFilter[] mf = GetComponentsInChildren<MeshFilter>();
        int count = mf.Length;
        for (int i = 0; i < count; i++)
        {
            Mesh m = mf[i].mesh;
            
            int vcount = m.vertices.Length;
            int[] indices = new int[vcount];
            for (int e = 0; e < vcount; e++)
                indices[e] = e;
 
            m.SetIndices(indices, MeshTopology.Points, 0);
        }
    }
}
