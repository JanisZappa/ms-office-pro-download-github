using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ToDo : MonoBehaviour
{
    public KeyCode toggleKey;
    public Color listColor = Color.white;
    
    [HideInInspector]public string todo;
    private bool show;
    
    private Camera cam;


    private void Update()
    {
        if(Input.GetKeyDown(toggleKey))
            show = !show;
    }


    private void OnGUI()
    {
        if(Application.isPlaying && !show)
            return;
        
        if(cam == null)
            cam = Camera.main;
        
        float offset = cam.rect.x * Screen.width;
        
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUI.skin.label.fontSize = Mathf.FloorToInt(Mathf.Min(Screen.width, Screen.height) * 1f / 30);
        GUI.color = listColor;
        int margin = Mathf.FloorToInt(Mathf.Min(Screen.width, Screen.height) * 1f / 30);
        GUI.Label(new Rect(offset + margin, margin / 2, Screen.width - margin * 2, Screen.height - margin), todo);
        GUI.color = Color.white;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ToDo))]
public class ToDoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10);
        
        ToDo td = target as ToDo;
        
        string before = td.todo;
        td.todo = GUILayout.TextArea(td.todo);

        if (td.todo != before)
            EditorUtility.SetDirty(td);
    }
}
#endif
