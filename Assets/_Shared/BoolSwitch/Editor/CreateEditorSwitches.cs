using LinkedBools;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class CreateEditorSwitches : Editor
{
    static CreateEditorSwitches()
    {
        Object getObject = Resources.Load("BoolSwitches") as Object;
        if ( getObject != null )
        {
            getObject = null;
            Resources.UnloadUnusedAssets();
            return;
        }

        string[] guids = AssetDatabase.FindAssets("");
        string path = "";
        for ( int i = 0; i < guids.Length; i++ )
        {
            string checkPath = AssetDatabase.GUIDToAssetPath(guids[i]);

            if ( checkPath.Contains("BoolSwitch.cs") )
                path = checkPath.Replace("BoolSwitch.cs", "Resources/BoolSwitches.asset");
        }


        _BoolSwitches inst = ScriptableObject.CreateInstance<_BoolSwitches>();

        AssetDatabase.CreateAsset(inst, path);
        AssetDatabase.SaveAssets();

        Debug.Log("Created Bool-Switches");
    }
}