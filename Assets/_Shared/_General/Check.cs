using UnityEngine;

public static class Check
{
    private static float t;

    public static float It()
    {
        float v = Time.realtimeSinceStartup - t;
        t = Time.realtimeSinceStartup;
        return v;
    }
    
    
    public static float It(MonoBehaviour mono)
    {
        float v = Time.realtimeSinceStartup - t;
        t = Time.realtimeSinceStartup;
        
        Debug.Log(mono.name + " " + v);
        
        return v;
    }
    
    
    public static float It(string message)
    {
        float v = Time.realtimeSinceStartup - t;
        t = Time.realtimeSinceStartup;
        
        Debug.Log(message + " : " + v);
        
        return v;
    }
}
