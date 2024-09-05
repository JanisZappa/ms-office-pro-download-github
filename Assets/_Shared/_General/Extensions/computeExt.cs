using System.Collections.Generic;
using UnityEngine;

public static class computeExt
{
    public static ComputeBuffer SetBuffers(this ComputeShader compute, string name, int length, int stride, 
        int a = -1, int b = -1, int c = -1, int d = -1, int e = -1, int f = -1,
        ComputeBufferType type = ComputeBufferType.Default)
    {
        ComputeBuffer buffer = new ComputeBuffer(length, stride, type);
        
        if(a != -1)    compute.SetBuffer(a, name, buffer);
        if(b != -1)    compute.SetBuffer(b, name, buffer);
        if(c != -1)    compute.SetBuffer(c, name, buffer);
        if(d != -1)    compute.SetBuffer(d, name, buffer);
        if(e != -1)    compute.SetBuffer(e, name, buffer);
        if(f != -1)    compute.SetBuffer(f, name, buffer);
        
        return buffer;
    }


    public static ComputeBuffer SetMultiBuffer(this ComputeShader compute,  string name, ComputeBuffer buffer,
        int a = -1, int b = -1, int c = -1, int d = -1, int e = -1, int f = -1)
    {
        if (a != -1) compute.SetBuffer(a, name, buffer);
        if (b != -1) compute.SetBuffer(b, name, buffer);
        if (c != -1) compute.SetBuffer(c, name, buffer);
        if (d != -1) compute.SetBuffer(d, name, buffer);
        if (e != -1) compute.SetBuffer(e, name, buffer);
        if (f != -1) compute.SetBuffer(f, name, buffer);

        return buffer;
    }

    
    public static Texture SetMultTexture(this ComputeShader compute,  string name, Texture texture,
        int a = -1, int b = -1, int c = -1, int d = -1, int e = -1, int f = -1)
    {
        if (a != -1) compute.SetTexture(a, name, texture);
        if (b != -1) compute.SetTexture(b, name, texture);
        if (c != -1) compute.SetTexture(c, name, texture);
        if (d != -1) compute.SetTexture(d, name, texture);
        if (e != -1) compute.SetTexture(e, name, texture);
        if (f != -1) compute.SetTexture(f, name, texture);

        return texture;
    }

    
    public static ComputeBuffer SetupIndirect(this ComputeShader compute, int count, string shaderX, int stride = 16)
    {
        Vector2Int ksteps = GetKernelSteps(count, stride);
        compute.SetInt(shaderX, ksteps.x * stride);
        return new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments).Init(new[]{ksteps.x, ksteps.y, 1, 0});
    }
    
    
    private static Vector2Int GetKernelSteps(int amount, int stride)
    {
        int x = Mathf.CeilToInt(Mathf.Sqrt(amount) / stride);
        int y = x;
        
        while (true)
        {
            if(y == 1)
                break;
            
            int less = y - 1;
            if (x * stride + less * stride < amount)
                break;
                
            y = less;
        }
        
        return new Vector2Int(x, y);
    }


    public static Dictionary<string, int> GetKernelDict(this ComputeShader compute, string[] names)
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        for (int i = 0; i < names.Length; i++)
            dict.Add(names[i], compute.FindKernel(names[i]));
        
        return dict;
    }
    
    
    public static int[] GetKernels(this ComputeShader compute, string[] names)
    {
        int[] kernels = new int[names.Length];
        for (int i = 0; i < names.Length; i++)
            kernels[i] = compute.FindKernel(names[i]);
        
        return kernels;
    }
}
