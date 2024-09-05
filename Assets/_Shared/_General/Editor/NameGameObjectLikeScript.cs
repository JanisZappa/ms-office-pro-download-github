using UnityEditor;
using UnityEngine;


public static class NameGameObjectLikeScript 
{
    [MenuItem("Edit/Name GameObject like Script %9")]
    private static void NameIt()
    {
        GameObject gO = Selection.activeGameObject;
        
        if(gO == null || !gO.name.Contains("GameObject"))
            return;

        Component[] allComponents = gO.GetComponents<Component>();
        for (int i = 0; i < allComponents.Length; i++)
        {
            string name = allComponents[i].GetType().ToString();
            
            if(!name.Contains("UnityEngine"))
            {
                gO.name = name;
                break;
            }
        }  
    }
}
