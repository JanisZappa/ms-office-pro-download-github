using UnityEditor;


internal static class UsefulShortcuts
{
    [MenuItem("Edit/Clear Console _^")]
    private static void ClearConsole()
    {
        /*var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);*/
    }
}