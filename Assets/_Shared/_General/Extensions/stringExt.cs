using UnityEngine;

public static class stringExt
{
    public static int PadLeft(this string[] list)
    {
        int count = list.Length;
        int maxLength = 0;
        for (int i = 0; i < count; i++)
            maxLength = Mathf.Max(maxLength, list[i].Length);
        
        for (int i = 0; i < count; i++)
            list[i] = list[i].PadLeft(maxLength);
        
        return maxLength;
    }
    
    public static int PadRight(this string[] list)
    {
        int count = list.Length;
        int maxLength = 0;
        for (int i = 0; i < count; i++)
            maxLength = Mathf.Max(maxLength, list[i].Length);
        
        for (int i = 0; i < count; i++)
            list[i] = list[i].PadRight(maxLength);
        
        return maxLength;
    }


    public static int ID(this string value)
    {
        return Shader.PropertyToID(value);
    }


    public static string Ordinal(this int num)
    {
        if( num <= 0 ) return num.ToString();

        switch(num % 100)
        {
            case 11:
            case 12:
            case 13:
                return num + "th";
        }
    
        switch(num % 10)
        {
            case 1:
                return num + "st";
            case 2:
                return num + "nd";
            case 3:
                return num + "rd";
            default:
                return num + "th";
        }
    }
}
