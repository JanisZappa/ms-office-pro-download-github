using UnityEngine;

public static class Keys 
{
    public static float Axis(KeyCode a, KeyCode b, float min, float max)
    {
        float range  = (max - min) * .5f;
        float center = min + range;
        return center - (Input.GetKey(a)? range : 0) + (Input.GetKey(b)? range : 0);
    }

    public static float Horizontal_Arrows
    {
        get
        {
            return Axis(KeyCode.LeftArrow, KeyCode.RightArrow, -1, 1);
        }
    }
    
    public static float Vertical_Arrows
    {
        get
        {
            return Axis(KeyCode.DownArrow, KeyCode.UpArrow, -1, 1);
        }
    }
    
    public static float Horizontal_AD
    {
        get
        {
            return Axis(KeyCode.A, KeyCode.D, -1, 1);
        }
    }
    
    public static float Vertical_SW
    {
        get
        {
            return Axis(KeyCode.S, KeyCode.W, -1, 1);
        }
    }
    
    public const KeyCode ß = KeyCode.LeftBracket;
    public const KeyCode ä = KeyCode.Quote;
    public const KeyCode ö = KeyCode.BackQuote;
    public const KeyCode ü = KeyCode.Semicolon;
    public const KeyCode Hash = KeyCode.Slash;
    public const KeyCode Plus = KeyCode.Equals;
    public const KeyCode Axon = KeyCode.RightBracket;

    public static KeyCode Number(int value)
    {
        switch (Mathf.Clamp(value, 0, 9))
        {
            default: return KeyCode.Alpha0;
            case 1:  return KeyCode.Alpha1;
            case 2:  return KeyCode.Alpha2;
            case 3:  return KeyCode.Alpha3;
            case 4:  return KeyCode.Alpha4;
            case 5:  return KeyCode.Alpha5;
            case 6:  return KeyCode.Alpha6;
            case 7:  return KeyCode.Alpha7;
            case 8:  return KeyCode.Alpha8;
            case 9:  return KeyCode.Alpha9;
        }
    }
}
