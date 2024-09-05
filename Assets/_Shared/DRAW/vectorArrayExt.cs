using System.Collections.Generic;
using UnityEngine;


public static class vectorArrayExt 
{
    public static void SetX(this Vector3[] points, int start, int end, float x)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetX(x);
    }
    
    public static void SetX(this List<Vector3> points, int start, int end, float x)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetX(x);
    }
    
    
    
    
    public static void SetY(this Vector3[] points, int start, int end, float y)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetY(y);
    }
    
    public static void SetY(this List<Vector3> points, int start, int end, float y)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetY(y);
    }
    
    
    
    
    public static void SetZ(this Vector3[] points, int start, int end, float z)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetZ(z);
    }
    
    public static void SetZ(this List<Vector3> points, int start, int end, float z)
    {
        for (int i = start; i < end; i++)
            points[i] = points[i].SetZ(z);
    }
}
