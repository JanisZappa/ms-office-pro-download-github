using System;
using System.Collections.Generic;
using UnityEngine;


public static class computebufferExt
{
    public static ComputeBuffer Init(this ComputeBuffer buffer, Array data)
    {
        buffer.SetData(data);
        return buffer;
    }


    public static void Dispose(this List<ComputeBuffer> list)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
            list[i]?.Dispose();
    }
}
