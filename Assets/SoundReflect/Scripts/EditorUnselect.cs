using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorUnselect : MonoBehaviour
{
    private void Start()
    {
        #if UNITY_EDITOR
        Selection.activeGameObject = null;
        #endif
    }
}
