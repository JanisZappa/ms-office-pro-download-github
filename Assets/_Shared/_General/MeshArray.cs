using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



[CreateAssetMenu]
public class MeshArray : ScriptableObject
{
    [HideInInspector]public Mesh[] meshes;

    public int Length
    {
        get
        {
            return meshes.Length;
        }
    }
    
    public Mesh this[int index]
    {
        get
        {
            return meshes[index];
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(MeshArray))]
public class MeshArrayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshArray mA = target as MeshArray;

        Mesh valid  = null;
        bool rescan = false;
        if(mA.meshes != null)
        for (int i = 0; i < mA.meshes.Length; i++)
        {
            Mesh m = mA.meshes[i];
            if(m == null)
                rescan = true;
            else
                valid = m;
        }

        if (rescan && valid != null)
            FindMeshes(valid, mA);
        
        GUILayout.BeginHorizontal();
        
        Object dragHere = EditorGUILayout.ObjectField("Drag here: ", null, typeof(Object)); 
        
        if(dragHere != null)
           FindMeshes(dragHere, mA);
        
        if(GUILayout.Button("Refresh") && mA.meshes.Length > 0 && mA.meshes[0] != null)
            FindMeshes(mA.meshes[0], mA);
        
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        GUILayout.Label(mA.meshes.Length + " Meshes");
        GUILayout.Space(5);

        for (int i = 0; i <  mA.meshes.Length; i++)
            GUILayout.Label(mA.meshes[i].name);
    }


    private static void FindMeshes(Object initial,  MeshArray mA)
    {
        string path = AssetDatabase.GetAssetPath(initial);
        if (AssetDatabase.LoadAssetAtPath<Mesh>(path) != null)
        {
            path = path.Replace(".fbx", "");
            while (true)
            {
                if(int.TryParse(path[path.Length - 1].ToString(), out int whatever))
                    path = path.Substring(0, path.Length - 1);
                else
                    break;
            }
            
            string[] all = FindMatchingAssets(new []{ path });
            List<Mesh> meshList = new List<Mesh>();
            for (int i = 0; i < all.Length; i++)
            {
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(all[i]);
                
                if(mesh != null && !meshList.Contains(mesh))
                    meshList.Add(mesh);
            }
            
            mA.meshes = meshList.ToArray();
            EditorUtility.SetDirty(mA);
        }
    }
    
    
    private static string[] FindMatchingAssets(string[] matches)
    {
        List<string> assets = new List<string>();
        string[] guids = AssetDatabase.FindAssets("", null );
      
        for( int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );

            for (int e = 0; e < matches.Length; e++)
                if(!assetPath.Contains(matches[e]))
                    goto NoMatch;
            
            assets.Add(assetPath);
            continue;
            
            NoMatch:;
        }
        
        
        return assets.ToArray();
    }
}
#endif