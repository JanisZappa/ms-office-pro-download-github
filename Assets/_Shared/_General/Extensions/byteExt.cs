using System;
using System.IO;
using UnityEngine;


public static class byteExt
{
//  https://arkonica.wordpress.com/2012/06/06/chow-to-set-and-read-a-single-bit-from-a-byte/ //
    public static byte Set(this byte aByte, int pos, bool set)
    {
        if (set)
            return (byte)(aByte | (1 << pos));
    
        return (byte)(aByte & ~(1 << pos));
    }


    /// <param name="aByte"></param>
    /// <param name="valueA">Main</param>
    /// <param name="valueB">Contact</param>
    /// <param name="valueC">Occlusion</param>
    /// <param name="valueD">Floor</param>
    /// <param name="valueE"></param>
    /// <param name="valueF"></param>
    /// <param name="valueG"></param>
    /// <param name="valueH"></param>
    public static byte Set(this byte aByte, bool valueA, bool valueB = false, bool valueC = false, bool valueD = false, bool valueE = false, bool valueF = false, bool valueG = false,  bool valueH = false)
    {
        return aByte.Set(0, valueA).
                     Set(1, valueB).
                     Set(2, valueC).
                     Set(3, valueD).
                     Set(4, valueE).
                     Set(5, valueF).
                     Set(6, valueG).
                     Set(7, valueH);
    }


    public static bool Get(this byte aByte, int pos)
    {
        return (aByte & (1 << pos)) != 0;
    }


    public static string HoudiniString(this BinaryReader r)
    {
        int count = r.ReadInt32();
        string value = "";
        for (int i = 0; i < count; i++)
            value += Convert.ToChar(r.ReadSByte());
        
        return value;
    }
    
    
    /*public static void WriteHoudiniString(this BinaryWriter r, string value)
    {
        int count = value.Length;
        r.Write(count);
        
        for (int i = 0; i < count; i++)
            r.Write(value);
            value += Convert.ToChar(r.ReadSByte());
    }*/
    
    
    public static Vector3 HoudiniVector3(this BinaryReader r, bool mirrorX = false)
    {
        return new Vector3(r.ReadSingle() * (mirrorX? -1 : 1), r.ReadSingle(), r.ReadSingle());
    }
    
    public static Vector3 HoudiniVector2(this BinaryReader r, bool mirrorX = false)
    {
        return new Vector2(r.ReadSingle() * (mirrorX? -1 : 1), r.ReadSingle());
    }

    public static float ReadShortFloat(this BinaryReader r, int multi = 10)
    {
        float div = 1f / multi;
        return r.ReadInt16() * div;
    }
    
    public static float ReadByteFloat(this BinaryReader r)
    {
        const float multi = 1f / 255f;
        return r.ReadByte() * multi;
    }
}
