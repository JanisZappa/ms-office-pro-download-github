using System.IO;
using UnityEngine;


public static class serializeExt 
{
    public static void Write(this BinaryWriter writer, Vector2 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }
    
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        return new Vector2(reader.ReadSingle(), reader.ReadSingle());
    }
    
    
    public static BinaryWriter Write(this BinaryWriter w, Vector3 vector)
    {
        w.Write(vector.x);
        w.Write(vector.y);
        w.Write(vector.z);
        
        return w;
    }
    
    public static Vector3 ReadVector3(this BinaryReader r)
    {
        return new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
    }
    
    
    public static BinaryWriter Write(this BinaryWriter w, Quaternion rot)
    {
        w.Write(rot.x);
        w.Write(rot.y);
        w.Write(rot.z);
        w.Write(rot.w);
        
        return w;
    }
    
    public static Quaternion ReadQuaternion(this BinaryReader r)
    {
        return new Quaternion(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
    }
}
