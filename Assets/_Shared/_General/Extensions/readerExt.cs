using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class readerExt
{
    public static int GetInt32(this BinaryReader reader)
    {
        #if UNITY_EDITOR_OSX

        var data = reader.ReadBytes(4);
        //Array.Reverse(data);
        //return BitConverter.ToInt32(data, 0);
        return 0;
        #endif

        return reader.ReadInt32();
    }
}
