using System.Collections.Generic;
using System.Linq;
using LinkedBools;
using UnityEditor;
using UnityEngine;


public class BoolSwitchWindow : EditorWindow
{
    private static BoolSwitchWindow window;
    
    private static Vector2 scrollPos;
    private static readonly List<string> menus = new List<string>();

    
    private static GUIStyle Style, Unfolded;
    

    private static int boolCount;
    
    
    [MenuItem("Window/QuickSwitches")]
    private static void OpenWindow()
    {
        window = GetWindow(typeof(BoolSwitchWindow), false, "QuickSwitch") as BoolSwitchWindow;
        window.minSize  = new Vector2(200, 250);
    }


    private static void CreateStyles()
    {
        Style = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState { textColor = Color.gray }
        };

        Unfolded = new GUIStyle()
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState { textColor = Color.Lerp(Color.gray, Color.black, .5f) }
        };
    }

    
    private void OnGUI()
    {
        if(Style == null)
            CreateStyles();

        
        const float buttonHeight = 18;
        const float labelHeight  = 18;
        const float bottomArea   = 50;
        
        //    Layout    //
        float windowHeight = Screen.height;
        float windowWidth  = EditorGUIUtility.currentViewWidth;

        List<BoolLink> displayList = BoolSwitch.links.OrderBy(x => x.SortName).ToList();

        if (boolCount != displayList.Count)
        {
            boolCount = displayList.Count;

            menus.Clear();
            for (int i = 0; i < displayList.Count; i++)
            {
                bool newMenu = true;
                for (int e = 0; e < menus.Count; e++)
                    if (menus[e] == displayList[i].menu)
                    {
                        newMenu = false;
                        break;
                    }

                if (newMenu)
                    menus.Add(displayList[i].menu);
            }
        }
        

        float listHeight = menus.Count * (labelHeight + 3);
        for (int i = 0; i < displayList.Count; i++)
            if (PlayerPrefs.GetInt("BoolsSwitchMenu_" + displayList[i].menu) == 0)
                listHeight += buttonHeight + 2;
        
        
        bool tooLong = listHeight > windowHeight - bottomArea;
        float buttonWidth = windowWidth - (tooLong ? 32 : 16) - 36;
        
        if (tooLong)
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Height(windowHeight - bottomArea));

        GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        int menu = -1;
        bool folded = false;
        for (int i = 0; i < displayList.Count; i++)
        {
            if (menu == -1 || displayList[i].menu != menus[menu])
            {
                menu++;

                folded = PlayerPrefs.GetInt("BoolsSwitchMenu_" + menus[menu]) == 1;

                EditorGUILayout.BeginHorizontal();

                Rect rect = GUILayoutUtility.GetRect(buttonWidth, labelHeight, Style);
                if (GUI.Button(rect, menus[menu], folded ? Style : Unfolded))
                {
                    folded = !folded;
                    PlayerPrefs.SetInt("BoolsSwitchMenu_" + menus[menu], folded ? 1 : 0);
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
            }

            if (folded)
                continue;

            string sign = displayList[i].defaultValue ? "▶" : "●";

            EditorGUILayout.BeginHorizontal();

            GUI.color = displayList[i].value ? new Color(0, 1, .494f, 1) : new Color(1f, .494f, .314f, 1);

            string boolLinks = Application.isPlaying ? " : " + displayList[i].ActiveLinks : "";
            if (GUILayout.Button(displayList[i].linkName + boolLinks, GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
                BoolSwitch.SetBool(displayList[i], !displayList[i].value);


            // Default and Delete Button
            GUI.color = displayList[i].value != displayList[i].defaultValue ? new Color(0, .961f, 1, 1) : Color.white;

            if (GUILayout.Button(sign, GUILayout.Height(buttonHeight), GUILayout.Width(18)))
                BoolSwitch.SetBool(displayList[i], displayList[i].defaultValue);

            GUI.color = Color.white;

            if (GUILayout.Button("X", GUILayout.Height(buttonHeight), GUILayout.Width(18)))
                BoolSwitch.RemoveBool(displayList[i]);

            EditorGUILayout.EndHorizontal();
        }
        
        
        if(tooLong)
            GUILayout.EndScrollView();

        if ( !tooLong )
            GUILayout.Space(windowHeight - listHeight- bottomArea);
        
        
        EditorGUILayout.BeginHorizontal();
        GUI.color = Color.white;
        if ( GUILayout.Button("Reset All") )
            for ( int i = 0; i < displayList.Count; i++ )
                if ( displayList[i].value != displayList[i].defaultValue )
                    BoolSwitch.SetBool(displayList[i], displayList[i].defaultValue);
        
        if(GUILayout.Button("Clear List"))
            BoolSwitch.links.Clear();
        
        EditorGUILayout.EndHorizontal();
    }
    
    
    public void OnInspectorUpdate()
    {
        if ( Application.isPlaying && BoolSwitch.RepaintAction == null)
            BoolSwitch.RepaintAction = Repaint;
    }
}