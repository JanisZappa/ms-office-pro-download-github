using UnityEngine;


public static class randomExt 
{
    public static float Range(this System.Random rand, float min, float max)
    {
        return min + (max - min) * ((float) rand.Next() / int.MaxValue);
    }
    
    public static int Range(this System.Random rand, int min, int max)
    {
        return Mathf.FloorToInt(min + (max - min) * ((float) rand.Next() / int.MaxValue));
    }

    public static bool Chance(this System.Random rand, int chance, int max)
    {
        return rand.Range(0, max) < chance;
    }
}
