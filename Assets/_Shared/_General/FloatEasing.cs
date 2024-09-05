using UnityEngine;


public static class Ease  
{
    public static float FF(float value)
    {
        float mapped = Mathf.PI * .5f * (Mathf.Clamp01(value) * 2 - 1);
        return .5f + Mathf.Sin(mapped) * .5f;
    }
    
    public static float LF(float value)
    {
        return Mathf.Sin(Mathf.PI * .5f * Mathf.Clamp01(value));
    }
    
    public static float FL(float value)
    {
        return 1 + Mathf.Sin(Mathf.PI * .5f * ( Mathf.Clamp01(value) - 1 ));
    } 
}
