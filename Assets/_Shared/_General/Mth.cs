using UnityEngine;


public static class Mth
{
    public const float π         = 3.141593f;
    public const float EulerNum   = 2.718281f;
    public const float Pythagoras = 1.414214f;
    
    public const float FullRad = 2 * Mathf.PI;
    public const float HalfRad = Mathf.PI;
    public const float QuarterRad = Mathf.PI * .5f;
    
    
    private static float Damp(float x, float dampRange)
    {
        if ( x < 0 )
            return x;

        return dampRange * (1 - Mathf.Exp(-x / dampRange));
    }

    public static float DampedRange(float value, float remapMin, float remapMax)
    {
        float range = remapMax - remapMin;
        
        if ( range >= 0 )
            return  Damp(value - remapMin, Mathf.Abs(range)) + remapMin;
        
        return remapMin - Damp(remapMin - value, Mathf.Abs(range));
    }

    
    public static class Chord
    {
        public static float GetRadians(float chordLength, float radius)
        {
            return 2 * Mathf.Asin(chordLength / 2 / radius);
        }
            
            
        public static float GetAngle(float chordLength, float radius)
        {
            return 2 * Mathf.Asin(chordLength / 2 / radius) * Mathf.Rad2Deg;
        }

            
        public static float GetArcLength(float chordLength, float radius)
        {
            float radians = 2 * Mathf.Asin(chordLength / 2 / radius);
            return radians / 2 * π * radius;
        }
            
            
        public static float GetRadius(float chordLength, float arcLength)
        {
            float circleFactor = GetCircleFactor(chordLength, arcLength);
                
            return 1f / circleFactor * arcLength / π / 2;
        }
            
            
        public static float GetCircleFactor(float chordLength, float arcLength)
        {
            if (f.Same(arcLength, chordLength))
                return 0;
                
            float circleFactor = (arcLength - chordLength) / arcLength;

            const float power = π / EulerNum;
                
            circleFactor = Mathf.Sin(Mathf.Pow(circleFactor, power) * .5f * π);
              
            return circleFactor;
        }
            
            
        public static float GetRadiusFull(float chordLength, float arcLength)
        {
            return 1f / Mathf.Sin(Mathf.Pow((arcLength - chordLength) / arcLength, π / EulerNum) * .5f * π) * arcLength / π / 2;
        }
    }


    public static float Divide(this float value, float divider, int repeat)
    {
        for (int i = 0; i < repeat; i++)
            value = value / divider;

        return value;
    }
    
    
    public static float SmoothPP(float t)
    {
        const float start = Mathf.PI * -.5f;
        return Mathf.Sin(start + t * Mathf.PI);
    }
    
    
    public static float SmoothPP(float min, float max, float t)
    {
        const float start = Mathf.PI * -.5f;
        return min + (max - min) * (Mathf.Sin(start + t * Mathf.PI) * .5f + .5f);
    }
    
    
    public static float Repeat(float a, float b, float value)
    {
        float min = Mathf.Min(a, b);
        float max = Mathf.Max(a, b);
        
        float range  = max - min;
        float offset = -min;
        
        return Mod(value + offset, range) - offset;
    }
    
    
    //https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
    private static float Mod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }


    public static float IntPow(float value, int pow)
    {
        switch (pow)
        {
            case  2: return value * value;
            case  3: return value * value * value;
            case  4: return value * value * value * value;
            case  5: return value * value * value * value * value;
            case  6: return value * value * value * value * value * value;
            case  7: return value * value * value * value * value * value * value;
            case  8: return value * value * value * value * value * value * value * value;
            case  9: return value * value * value * value * value * value * value * value * value;
            case 10: return value * value * value * value * value * value * value * value * value * value;
            
            case 11: return value * value * value * value * value * value * value * value * value * value * value;
            case 12: return value * value * value * value * value * value * value * value * value * value * value * value;
            case 13: return value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 14: return value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 15: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 16: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 17: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 18: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 19: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 20: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            
            case 21: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 22: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 23: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 24: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 25: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 26: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 27: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 28: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 29: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
            case 30: return value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value * value;
        }

        return Mathf.Pow(value, pow);
    }


    public static float Sin01(float t)
    {
        return Mathf.Sin(t) * .5f + .5f;
    }
}
