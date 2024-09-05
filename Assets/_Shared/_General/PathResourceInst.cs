using UnityEngine;


public static class PathResourceInst 
{
    public static GameObject ResourceInst(this string path, Transform parent = null)
    {
        return Object.Instantiate(Resources.Load(path) as GameObject, parent);
    }
}
