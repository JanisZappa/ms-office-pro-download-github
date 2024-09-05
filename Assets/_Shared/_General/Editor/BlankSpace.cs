using UnityEditor;
using UnityEngine;


public static class BlankSpace 
{
    [MenuItem("Assets/Create/BlankSpace Before %UP")]
    public static void CreateBefore()
    {
        Create(true);
    }
    
    [MenuItem("Assets/Create/BlankSpace After %DOWN")]
    public static void CreateAfter()
    {
        Create(false);
    }
    
    
    private static void Create(bool before)
    {
        if (Selection.activeGameObject == null)
            return;

        Transform t = Selection.activeGameObject.transform;
        int index = t.GetSiblingIndex() + (before? 0 : 1);
        
        GameObject blank = new GameObject("");
        blank.transform.SetParent(t.parent, false);
        blank.transform.SetSiblingIndex(index);
        blank.tag = "EditorOnly";
    }
}
