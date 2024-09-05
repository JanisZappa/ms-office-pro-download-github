using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class DRAW_Execution : Editor
{
    static DRAW_Execution()
    {
        string scriptName = typeof(DRAW).Name;
        MonoScript[] scripts = MonoImporter.GetAllRuntimeMonoScripts();
        for (int i = 0; i < scripts.Length; i++)
        {
            MonoScript monoScript = scripts[i];
            if (monoScript.name == scriptName)
            {
                if (MonoImporter.GetExecutionOrder(monoScript) != 30000)
                {
                    MonoImporter.SetExecutionOrder(monoScript, 30000);
                    Debug.Log("Set DRAW's execution order to 30000");
                }

                break;
            }
        }
    }
}