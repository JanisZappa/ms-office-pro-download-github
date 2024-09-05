using UnityEditor;
using UnityEngine;


public class CrappyMeshImporter : AssetPostprocessor
{
    private void OnPostprocessModel(GameObject obj)
    {
        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < meshFilters.Length; i++)
            Collapse(meshFilters[i]);
    }
    
    
    private void CollapseF(MeshFilter m)
    {
        if(!assetImporter.assetPath.Contains("CrappyFishey"))
            return;
        
        Mesh mesh = m.sharedMesh;
        
        Vector2[] uva = mesh.uv, uvb = mesh.uv2;
        if(uva == null || uva.Length == 0 || uvb == null || uvb.Length == 0)
            return;
        
        Debug.Log(m.name);
        
        int count = uva.Length;
        Vector4[] merge = new Vector4[count];
        for (int i = 0; i < count; i++)
        {
            Vector2 a = uva[i], b = uvb[i];
            merge[i] = new Vector4(a.x, a.y, b.x, b.y);
        }
            
        mesh.SetUVs(0, merge);
        mesh.SetUVs(1, new Vector4[0]);
        EditorUtility.SetDirty(assetImporter);
    }
    
    
    private void Collapse(MeshFilter m)
    {
        if(!assetImporter.assetPath.Contains("CrappyFishey"))
            return;
        
        Mesh mesh = m.sharedMesh;
        
        int doneSomething = 0;

        Vector2[] uva, uvb;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                default:    uva = mesh.uv;    uvb = mesh.uv2;    break;
                case 1:     uva = mesh.uv3;    uvb = mesh.uv4;    break;
                case 2:     uva = mesh.uv5;    uvb = mesh.uv6;    break;
                case 3:     uva = mesh.uv7;    uvb = mesh.uv8;    break;
            }
            
            if(uva == null || uva.Length == 0 || uvb == null || uvb.Length == 0)
                continue;
        
            doneSomething++;
        
            int count = uva.Length;
            Vector4[] merge = new Vector4[count];
            for (int e = 0; e < count; e++)
            {
                Vector2 a = uva[e], b = uvb[e];
                merge[e] = new Vector4(a.x, a.y, b.x, b.y);
            }
            
            mesh.SetUVs(i, merge);

            switch (i)
            {
                default:    mesh.SetUVs(i * 2, new Vector4[0]);  
                            mesh.SetUVs(i * 2 + 1, new Vector4[0]);  
                            break;
                
                case 0:     mesh.SetUVs(1, new Vector4[0]);    break;
            }
        }

        if (doneSomething > 0)
        {
            Debug.LogFormat("Merged {0} for {1}", doneSomething, m.name);
            EditorUtility.SetDirty(assetImporter);
        }
    }
}
