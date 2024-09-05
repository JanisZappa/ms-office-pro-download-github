using System;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    private readonly List<ComputeBuffer> buffers = new List<ComputeBuffer>();
    
    private static Buff Inst;
    public static ComputeBuffer Add(ComputeBuffer buffer)
    {
        if (Inst == null)
            Inst = new GameObject("BufferCleaner").AddComponent<Buff>();
        
        return  Inst.buffers.GetAdd(buffer);;
    }


    public static ComputeBuffer New(Array array, int size, ComputeBufferType type = ComputeBufferType.Default)
    {
        return Add(new ComputeBuffer(array.Length, size, type).Init(array));
    }
    
    
    public static ComputeBuffer New(int length, int size, ComputeBufferType type = ComputeBufferType.Default)
    {
        return Add(new ComputeBuffer(length, size, type));
    }
    

    private void OnDisable()
    {
        buffers?.Dispose();
    }
}
