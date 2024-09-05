using UnityEngine;


public static class gameobjectExt
{
    public static GameObject SetName(this GameObject gameObject, string name)
    {
        gameObject.name = name;
        return gameObject;
    }
    
    
    public static GameObject SetPos(this GameObject gameObject, Vector3 pos, bool local = false)
    {
        if(local)
            gameObject.transform.localPosition = pos;
        else
            gameObject.transform.position = pos;
        
        return gameObject;
    }
    
    
    public static GameObject SetRot(this GameObject gameObject, Quaternion rot, bool local = false)
    {
        if(local)
            gameObject.transform.localRotation = rot;
        else
            gameObject.transform.rotation = rot;
        
        return gameObject;
    }
    
    
    public static GameObject SetScale(this GameObject gameObject, float x, float y, float z)
    {
        gameObject.transform.localScale = new Vector3(x, y, z);

        return gameObject;
    }
    
    
    public static GameObject SetParent(this GameObject gameObject, Transform parent, bool setToZero = false)
    {
        gameObject.transform.SetParent(parent, !setToZero);
        
        return gameObject;
    }
    
    
    public static GameObject SetLayerToAll(this GameObject gameObject, LayerMask layermask)
    {
        gameObject.layer    = layermask;
        Transform transform = gameObject.transform;

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
            SetLayerToAll(transform.GetChild(i).gameObject, layermask);

        return gameObject;
    }


    public static GameObject CopyAndParent(this GameObject gameObject, bool onlyInEditor = false)
    {
        GameObject newGO = Object.Instantiate(gameObject);

        if (!onlyInEditor || Application.isEditor)
            newGO.transform.SetParent(gameObject.transform.parent);

        return newGO;
    }
    
    
    public static T GetOrAdd<T>(this GameObject gO) where T : Component
    {
        T t = gO.GetComponent<T>();
        if(t == null)
            t = gO.AddComponent<T>();
        
        return t;
    }


    public static GameObject CreateChild(this GameObject gO, string name = "")
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(gO.transform, false);
        return child;
    }
    
    
    public static GameObject AddTag(this GameObject gO, string name)
    {
        gO.tag = name;
        return gO;
    }
    
    
    public static GameObject SetLayer(this GameObject gO, int layer)
    {
        gO.layer = layer;
        return gO;
    }
    
}
