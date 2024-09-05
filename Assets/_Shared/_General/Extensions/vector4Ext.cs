using UnityEngine;


public static class vector4Ext
{
//  Set  //
    public static Vector4 Set(this Vector4 vec, Vector2 xy, float z, float w)
    {
        return new Vector4(xy.x, xy.y, z, w);
    }
    
    public static Vector4 Set(this Vector4 vec, float x, Vector2 yz, float w)
    {
        return new Vector4(x, yz.x, yz.y, w);
    }
    
    public static Vector4 Set(this Vector4 vec, float x, float y, Vector2 zw)
    {
        return new Vector4(x, y, zw.x, zw.y);
    }
    
    public static Vector4 Set(this Vector4 vec, Vector3 xyz, float w)
    {
        return new Vector4(xyz.x, xyz.y, xyz.z, w);
    }
    
    public static Vector4 Set(this Vector4 vec, float x, Vector3 yzw)
    {
        return new Vector4(x, yzw.x, yzw.y, yzw.z);
    }
}
