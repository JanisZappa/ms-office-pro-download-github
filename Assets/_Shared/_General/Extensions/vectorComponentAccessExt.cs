using UnityEngine;


public static class vectorComponentAccessExt
{
    public static Vector2 XY(this Vector4 value)
    {
        return new Vector2(value.x, value.y);
    }
    
    
    public static Vector2 ZW(this Vector4 value)
    {
        return new Vector2(value.z, value.w);
    }
    
    public static Vector3 XYZ(this Vector4 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }
}
