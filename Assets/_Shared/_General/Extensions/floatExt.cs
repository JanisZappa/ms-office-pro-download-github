using UnityEngine;


public static class floatExt 
{
    public static float SignAdd(this float value, float addAmount)
    {
        return value + Mathf.Sign(value) * addAmount;
    }
    
    
    public static float AxisΔ(this float value, string axis, float multi)
    {
        return value + Time.deltaTime * multi * Input.GetAxis(axis);
    }


    public static float Clamp(this float value, float min = 0, float max = 1)
    {
        return Mathf.Clamp(value, min, max);
    }


    public static float InvLerp(this float value, float min, float max)
    {
        return (value - min) / (max - min);
    }
    
    
    public static float RoundFrac(this float value, float fraction)
    {
        return Mathf.Round(value * (1/ fraction)) * fraction;
    }


    public static float NaNChk(this float value, float defaultValue = 0)
    {
        return float.IsNaN(value) ? defaultValue : value;
    }


    public static float Euler180(this float value)
    {
        return value > 180 ? -(360 - value) : value;
    }
    
    
//  Input Change  //
    public static float Wrap(this float value, float min, float max)
    {
        value -= min;
        float length = max - min;
        
        return value - Mathf.Floor(value / length) * length + min;
    }


    public static float KeyΔ(this float value, KeyCode key, float multi)
    {
        return Input.GetKey(key) ? value + Time.deltaTime * multi : value;
    }

    
    public static float KeySet(this float value, KeyCode key, float newValue, bool down = true)
    {
        return (down? Input.GetKeyDown(key) : Input.GetKeyUp(key)) ? newValue : value;
    }


    public static int IntSign(this float value)
    {
        return value >= 0 ? 1 : -1;
    }


    public static float Map01(this float value)
    {
        return value * .5f + .5f;
    }
}


public static class f
{
    public delegate bool FloatCompare(float valueA, float valueB);
    public static readonly FloatCompare Same = Mathf.Approximately;
}
