using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Castle))]
public class CastleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Castle castle = target as Castle;
        
        GUI.color = COLOR.red.tomato;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Generate"))
        {
            castle.Generate();
            EditorUtility.SetDirty(castle.gameObject);
        }
       
        GUILayout.EndHorizontal();
        GUI.color = Color.white;
        
        base.OnInspectorGUI();
        
        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Floors: " + castle.transform.childCount + " - Rooms: " + castle.rooms.Length + " - Doors: " + castle.doors.Length);
        //GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
