using UnityEditor;
using UnityEngine;


public class COLORWINDOW : EditorWindow
{
    private static Color color;
    private static COLORWINDOW window;
    private static bool csharp;
    private static string lastString;

    
    [MenuItem("Edit/Copy Color to C# &c")]
    private static void CopyWindowCS()
    {
        if ( window != null )
        {
            CloseWindow(csharp);
            if ( csharp )
                return;
        }

        csharp = true;
        window = GetWindow(typeof(COLORWINDOW), true, "Copy To C#", true) as COLORWINDOW;
        window.minSize = window.maxSize = new Vector2(100, 30);
    }

    
    [MenuItem("Edit/Copy Color to JS &x")]
    private static void CopyWindowJS()
    {
        if ( window != null )
        {
            CloseWindow(!csharp);
            if ( !csharp )
                return;
        }

        csharp = false;
        window = GetWindow(typeof(COLORWINDOW), true, "Copy To JS", true) as COLORWINDOW;
        window.minSize = window.maxSize = new Vector2(105, 30);
    }


    private void OnEnable()
    {
        window = this;
        color  = new Color(PlayerPrefs.GetFloat("r"), PlayerPrefs.GetFloat("g"), PlayerPrefs.GetFloat("b"), PlayerPrefs.GetFloat("a"));
    }

    
    private void OnDisable()
    {
        CreateCopyString(color);
    }


    private static void CloseWindow(bool copy)
    {
        if ( copy )
            CreateCopyString(color);

        PlayerPrefs.SetFloat("r", color.r);
        PlayerPrefs.SetFloat("g", color.g);
        PlayerPrefs.SetFloat("b", color.b);
        PlayerPrefs.SetFloat("a", color.a);

        window.Close();
    }


    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.FlexibleSpace();
        color = EditorGUILayout.ColorField(color);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }


    private static void CreateCopyString(Color color)
    {
        string s;
        if ( csharp )
            s = "new Color(" + ShortendFloat(color.r) + "f, " + ShortendFloat(color.g) + "f, " + ShortendFloat(color.b) + "f, " + ShortendFloat(color.a) + "f)";
        else
            s = "Color(" + ShortendFloat(color.r) + ", " + ShortendFloat(color.g) + ", " + ShortendFloat(color.b) + ", " + ShortendFloat(color.a) + ")";

        if ( s == lastString )
            return;

        EditorGUIUtility.systemCopyBuffer = s;
        lastString = s;
        Debug.Log((csharp ? "C#" : "JS") + " \"" + s + "\"");
    }


    //TODO
    private static string ShortendFloat(float value)
    {
        string returnString = value.ToString("F3");

        bool lastCharacterIsZero = true;
        while ( lastCharacterIsZero )
            if ( returnString.Length > 1 && (returnString[returnString.Length - 1] == '0' || returnString[returnString.Length - 1] == '.') )
                returnString = returnString.Remove(returnString.Length - 1);
            else
                lastCharacterIsZero = false;

        return returnString;
    }
}
