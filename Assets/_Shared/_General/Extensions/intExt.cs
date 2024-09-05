using UnityEngine;


public static class intExt 
{
    static intExt()
    {
        numStrings = new string[maxStrings];
        
        for (int i = 0; i < maxStrings; i++)
            numStrings[i] = i.ToString();
    }

    private const int maxStrings = 10000;
    private static readonly string[] numStrings;

    
    public static string PrepString(this int value)
    {
        return value < maxStrings - 1 && value > -1? numStrings[value] : value.ToString();
    }
    
    public static string PrepString(this byte value)
    {
        return numStrings[value];
    }
    
    public static string RoundString(this float value)
    {
        return PrepString(Mathf.RoundToInt(value));
    }

    
    public static int Repeat(this int value, int max)
    {
        //return (int) Mathf.Repeat(value, max);
        
        
        while (value < 0)
            value += max;
        
        return value % max;
    }


    public static int IndexUp(this int index, int length)
    {
        if (index < length - 1)
            return index + 1;

        return 0;
    }
    
    
    public static int IndexDown(this int index, int length)
    {
        if (index > 0)
            return index - 1;

        return length - 1;
    }
    
    
    public static bool CloserThan(this int a, int b, int range)
    {
        return Mathf.Abs(a - b) <= range;
    }


    public static int NextPowerOfTwo(this int value)
    {
        if (value == 0)
            return 0;
        
        int power = 1;
        
        while(power < value)
            power *= 2;
        
        return power;
    }
    
    
    
}
