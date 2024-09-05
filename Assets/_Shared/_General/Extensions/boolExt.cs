using UnityEngine;


public static class boolExt 
{
    public static bool KeySwitch(this bool value, KeyCode key, bool down = true)
    {
        return (down? Input.GetKeyDown(key) : Input.GetKeyUp(key))? !value : value;
    }


    public static int SignInt(this bool value, int iValue)
    {
        return value ? iValue : -iValue;
    }
    
    
    public static float SignFloat(this bool value, float fValue)
    {
        return value ? fValue : -fValue;
    }
}