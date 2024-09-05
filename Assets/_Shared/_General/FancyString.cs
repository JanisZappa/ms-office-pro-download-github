using System.Text;
using UnityEngine;


public static class FancyString
{
    private static bool No { get { return !Application.isEditor; } }

    public static string B_Start(string color)
    {
        return "<b><color=" + color + ">";
    }

    public const string B_End = "</color></b>";

    public static string B(this string inputString)
    {
        return No? inputString : "<b>" + inputString + "</b>";
    }

    public static string B(this string inputString, int index)
    {
        switch (index)
        {
            default:  return inputString.B();
            case 0:   return inputString.B_Purple();
            case 1:   return inputString.B_Red();
            case 2:   return inputString.B_Orange();
            case 3:   return inputString.B_Yellow();
            case 4:   return inputString.B_Lime();
            case 5:   return inputString.B_Green();
            case 6:   return inputString.B_Teal();
            case 7:   return inputString.B_LightBlue();
            case 8:   return inputString.B_Blue();
            case 9:   return inputString.B_Pink();
            case 10:  return inputString.B_Salmon();
        }
    }

    public static string B_Frame(this string inputString)
    {
        return No? inputString : inputString.B(Time.frameCount % 11);
    }
    
    
    public static string B_Black(this string inputString)
    {
        return No? inputString : B_Hex(inputString, "#000000");
    }
    
    public static string B_Purple(this string inputString)
    {
        return No? inputString : B_Start("purple") + inputString + B_End;
    }

    public static string B_Red(this string inputString)
    {
        return No? inputString : B_Start("red") + inputString + B_End;
    }

    public static string B_Orange(this string inputString)
    {
        return No? inputString : B_Start("orange") + inputString + B_End;
    }

    public static string B_Yellow(this string inputString)
    {
        return No? inputString : B_Start("yellow") + inputString + B_End;
    }

    public static string B_Lime(this string inputString)
    {
        return No? inputString : B_Start("lime") + inputString + B_End;
    }

    public static string B_Green(this string inputString)
    {
        return No? inputString : B_Start("green") + inputString + B_End;
    }

    public static string B_Teal(this string inputString)
    {
        return No? inputString : B_Start("teal") + inputString + B_End;
    }

    public static string B_LightBlue(this string inputString)
    {
        return No? inputString : B_Start("aqua") + inputString + B_End;
    }

    public static string B_Blue(this string inputString)
    {
        return No? inputString : B_Start("blue") + inputString + B_End;
    }

    public static string B_Pink(this string inputString)
    {
        return No? inputString : B_Start("magenta") + inputString + B_End;
    }

    public static string B_Salmon(this string inputString)
    {
        return No? inputString : B_Start("#ff726b") + inputString + B_End;
    }
    
    public static string B_Color(this string inputString, Color color)
    {
        return No? inputString : B_Start(color.ToHex()) + inputString + B_End;
    }
    
    public static string B_Color32(this string inputString, Color32 color)
    {
        return No? inputString : B_Start(color.ToHex()) + inputString + B_End;
    }

    public static string B_Hex(this string inputString, string hex)
    {
        return No? inputString : B_Start(hex) + inputString + B_End;
    }

    public static string B_Switch(this string inputString, Color a, Color b, bool select)
    {
        return No? inputString : B_Start((select? b : a).ToHex()) + inputString + B_End;
    }

   
    private static readonly StringBuilder SB = new StringBuilder(100);
    private const string chars = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789";


    public static string Block = "█";
    public static string Arrow = "→";
    
    public static string GetRandom(int length)
    {
        SB.Length = 0;

        for (int i = 0; i < length; i++)
            SB.Append(chars[Random.Range(0, chars.Length)]);

        return SB.ToString();
    }
}
