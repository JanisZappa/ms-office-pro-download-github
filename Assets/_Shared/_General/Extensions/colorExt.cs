using System.Collections.Generic;
//using GeoMath;
using UnityEngine;


public static class colorExt 
{
    public static Color A(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }


    public static Color Multi(this Color color, float multi)
    {
        return new Color(color.r * multi, color.g * multi, color.b * multi, color.a);
    }
    
    
    public static string ToHex(this Color color)
    {
        return "#" + ColorUtility.ToHtmlStringRGB(color);
    }
    
    
    public static string ToHex(this Color32 color32)
    {
        return "#" + color32.r.ToString("X2") + color32.g.ToString("X2") + color32.b.ToString("X2");
    }


    public static Color HexColor(this string value)
    {
        Debug.Log(value);
        Color color;
        if(value.Contains("#"))
            ColorUtility.TryParseHtmlString (value, out color);
        else
            ColorUtility.TryParseHtmlString ("#" +  value, out color);
        ColorUtility.TryParseHtmlString (value, out color);
        return color;
    }


    private static bool HasRGB(this List<Color> list, Color value)
    {
        int count = list.Count;
        for (int i = 0; i < count; i++)
            if (list[i].SameRGB(value, 5))
                return true;

        return false;
    }


    private static bool SameRGB(this Color a, Color b, int range)
    {
        return Mathf.RoundToInt(a.r * 255).CloserThan(Mathf.RoundToInt(b.r * 255), range) &&
               Mathf.RoundToInt(a.g * 255).CloserThan(Mathf.RoundToInt(b.g * 255), range) &&
               Mathf.RoundToInt(a.b * 255).CloserThan(Mathf.RoundToInt(b.b * 255), range);
    }
    
    
    public static void AddIfUniqueRGB(this List<Color> list, Color value)
    {
        if(!list.HasRGB(value))
            list.Add(value);
    }


    /*public static Color GamutTest(this Color color, float rT)
    {
        Vector2 p1 = Rot.Z(rT)       * Vector2.up;
        Vector2 p2 = Rot.Z(rT + 120) * Vector2.up;
        Vector2 p3 = Rot.Z(rT + 240) * Vector2.up;
		
        Line a = new Line(p1, p2);
        Line b = new Line(p2, p3);
        Line c = new Line(p3, p1);
        
        return color.ToHLS().MapGamut(a, b, c);
    }*/
    

    public static Color32 Multi(this Color32 color, Color32 other)
    {
        Color a = color;
        Color b = other;
        
        return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
    }
    
    
    public static Color32 SetA(this Color32 color, byte a)
    {
        return new Color(color.r, color.g , color.b, a);
    }


    public static float Dist(this Color color, Color other)
    {
        return Mathf.Abs(color.r - other.r) + 
               Mathf.Abs(color.g - other.g) + 
               Mathf.Abs(color.b - other.b);
    }
    
    
    public static float DistHSV(this Color color, Color other)
    {
        Vector3 c1 = HSV(color);
        Vector3 c2 = HSV(other);
        return Mathf.Abs(c1.x - c2.x) * 20 + 
               Mathf.Abs(c1.y - c2.y) + 
               Mathf.Abs(c1.z - c2.z) * 4;
    }


    private static Vector3 HSV(this Color c)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return new Vector3(h, s, v);
    }
    
    
    public static float DistWeighted(this Color color, Color other)
    {
        const int v = 1;
        return Mathf.Abs(color.r - other.r) * Mathf.Pow(1f - color.r, v) + 
               Mathf.Abs(color.g - other.g) * Mathf.Pow(1f - color.g, v) + 
               Mathf.Abs(color.b - other.b) * Mathf.Pow(1f - color.b, v);
    }
    
    
    /*public static float CheckDist(this Color color, Color other)
    {
        return Mathf.Sqrt(Mathf.Pow(color.r - other.r, 2) + 
                          Mathf.Pow(color.g - other.g, 2) + 
                          Mathf.Pow(color.b - other.b, 2));
    }*/
    
    
    /*public static float CheckDist(this Color color, Color other)
    {
        float y1 = .299f * color.r + .587f * color.g + .114f * color.b;
        float y2 = .299f * other.r + .587f * other.g + .114f * other.b;

        float pB1 = .168736f * color.r - .331264f * color.g + .5f * color.b;
        float pB2 = .168736f * other.r - .331264f * other.g + .5f * other.b;

        float pR1 = .5f * color.r - .418688f * color.g - .081312f * color.b;
        float pR2 = .5f * other.r - .418688f * other.g - .081312f * other.b;

        return Mathf.Sqrt(1.4f * Mathf.Pow(y1 - y2, 2) + .8f * Mathf.Sqrt(pB1 - pB2) + .8f * Mathf.Sqrt(pR1 - pR2));
    }*/


    public static int ClosestColorIndex(this Color color, List<Color> list)
    {
        int bestIndex = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < list.Count; i++)
        {
            Color newColor = list[i];
            float dist = newColor.Dist(color);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
    
    
    public static float ClosestColorDist(this Color color, List<Color> list)
    {
        float bestDist = float.MaxValue;

        for (int i = 0; i < list.Count; i++)
        {
            Color newColor = list[i];
            float dist = newColor.Dist(color);
            if (dist < bestDist)
                bestDist = dist;
        }

        return bestDist;
    }
    
    
    public static int ClosestColorIndex(this Color color, Color[] array)
    {
        int bestIndex = 0;
        float bestDist = float.MaxValue;
        for (int i = 0; i < array.Length; i++)
        {
            Color newColor = array[i];
            float dist = newColor.Dist(color);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
    
    
    public static float ClosestColorDist(this Color color, Color[] array)
    {
        float bestDist = float.MaxValue;

        for (int i = 0; i < array.Length; i++)
        {
            Color newColor = array[i];
            float dist = newColor.Dist(color);
            if (dist < bestDist)
                bestDist = dist;
        }

        return bestDist;
    }


    public static Color PerlinColor(this Color color, float x, float y, float min = 0)
    {
        float multi = 1 - min;
        return new Color(min + Mathf.PerlinNoise(x * 522.7f + 46, y * 865.312f + 1653)       * multi, 
                         min + Mathf.PerlinNoise(x * 6622.17f + 846, y * 865.312f + 31153)   * multi, 
                         min + Mathf.PerlinNoise(x * 2722.557f + 2246, y * 865.312f + 99553) * multi, 1);
    }


    public static Color To1(this Color color, float lerp)
    {
        return Color.Lerp(color, Color.white, lerp);
    }
    
    public static Color To0(this Color color, float lerp)
    {
        return Color.Lerp(color, Color.black, lerp);
    }

    public static Vector3 XYZ(this Color color)
    {
        return new Vector3(color.r, color.g, color.b);
    }


    public static Color ShiftHue(this Color color, float amount)
    {
        Color.RGBToHSV(color, out float r, out float g, out float b);
        r = (r + amount).Wrap(0, 1);
        return Color.HSVToRGB(r, g, b);
    }
}



