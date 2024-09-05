using System.Collections.Generic;
using UnityEngine;


public static class keycodeExt
{
    static keycodeExt()
    {
        List<KeyCode> getKeys = new List<KeyCode>();
        foreach(KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            getKeys.Add(vKey);

        keyArray = getKeys.ToArray();
    }
    
    private static readonly KeyCode[] keyArray;
    
    
    public static KeyCode GetDownKey
    {
        get
        {
            int length = keyArray.Length;
            for (int i = 0; i < length; i++)
            {
                KeyCode code = keyArray[i];
                
                if (Input.GetKeyDown(code))
                    return code;
            }
           
            return KeyCode.None;
        }
    }
    
    
    
    public static string CharString(this KeyCode code)
    {
        return code.ToString().Replace("KeyCode.", "").Replace("Alpha", "").ToLower();
    }
}
