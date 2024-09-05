using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


public static class NormCoreDisable
{
    [MenuItem("Fuck Normcore/Recompile while Playing #p")]
    private static void Recompile()
    {
        EditorPrefs.SetInt("ScriptCompilationDuringPlay", 0);
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}

#endif

public class FuckNormcore : MonoBehaviour
{
    private void OnDisable()
    {
        #if UNITY_EDITOR
            EditorPrefs.SetInt("ScriptCompilationDuringPlay", 0);
            //Debug.Log("Andy undid what Normcore want's the recompiling to be like. Fuck Normcore!");
        #endif
    }

}

