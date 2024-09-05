using UnityEditor;
using UnityEngine;


public static class GroupSelection 
{
    [MenuItem("Edit/Group Items %g")]
    public static void GroupItems()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length == 0)
            return;

        Transform parent = selection[selection.Length - 1].transform.parent;
        GameObject group = new GameObject("Group");
        group.transform.SetParent(parent);
        
        for (int i = 0; i < selection.Length; i++)
            selection[i].transform.SetParent(group.transform, true);
    }
}
