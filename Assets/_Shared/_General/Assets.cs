#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public static class Assets
{
    public static T[] FindAll<T>() where T : Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );

            Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath);

           T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if(asset != null)
                assets.Add(asset);
        }
        return assets.ToArray();
    }
    
    
    public static Mesh[] FindAllMeshes(string path, bool getAllInObject = false)
    {
        List<Mesh> assets = new List<Mesh>();
        string[] guids = AssetDatabase.FindAssets("", new[] { "Assets/" + path });
        List<string> used = new List<string>();
        
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            if(used.Contains(assetPath))
                continue;

            Object[] allObjects = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            for (int o = 0; o < allObjects.Length; o++)
                if (allObjects[o] is Mesh)
                {
                    assets.Add((Mesh)allObjects[o]);
                    used.Add(assetPath);
                    
                    if(!getAllInObject)
                        break;
                }
                    
        }
        return assets.ToArray();
    }


    public static T Get<T>(string name) where T : Object
    {
        string[] guids = AssetDatabase.FindAssets(name);

        if (guids.Length == 0)
        {
            Debug.Log("Couldn't find Asset with Name \"" + name + "\"");
            return null;
        }
        
        if (guids.Length > 1)
        {
            Debug.Log("Too many Assets with Name \"" + name + "\"");
            return null;
        }
        
        string guid = guids[0];
        T thing = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));

        if (thing == null)
            Debug.Log("Assets with Name \"" + name + "\" isn't the right Type");

        return thing;
    }
    
    
    public static Material[] FindAllMaterials(string path = "")
    {
        List<Material> assets = new List<Material>();
        string[] guids = AssetDatabase.FindAssets("t:material");
        
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            if(!assetPath.Contains(path))
                continue;

            Material asset = AssetDatabase.LoadAssetAtPath<Material>( assetPath );
            if(asset != null)
                assets.Add(asset);
                    
        }
        return assets.ToArray();
    }
    
    
    public static Texture2D[] FindAllTextures(string path = "")
    {
        List<Texture2D> assets = new List<Texture2D>();
        string[] guids = AssetDatabase.FindAssets("t:texture2D");
        
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            if(!assetPath.Contains(path))
                continue;

            Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>( assetPath );
            if(asset != null)
                assets.Add(asset);
                    
        }
        return assets.ToArray();
    }


    public static T[] FindAllAt<T>(string path = "") where T : Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(typeMap[typeof(T)]);
        
        for( int i = 0; i < guids.Length; i++ )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
            if(!assetPath.Contains(path))
                continue;

            T asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
            if(asset != null)
                assets.Add(asset);
                    
        }
        return assets.ToArray();
    }
    
    
    public static string[] FindMatchingAssets(string[] matches)
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


    private static readonly Dictionary<Type, string> typeMap;
    
    static Assets()
    {
        typeMap = new Dictionary<Type, string>()
        {
            {typeof(Texture2D), "t:texture2D"},
            {typeof(Material),  "t:material"},
            {typeof(TextAsset), "t:textasset"}
        };
    }
}

#endif
