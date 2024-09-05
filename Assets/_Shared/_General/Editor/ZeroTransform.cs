using UnityEditor;
using UnityEngine;


public static class ZeroTransform 
{
    [MenuItem("Edit/Zero Transform %0")]
    public static void ZeroIt()
    {
        if (Selection.activeGameObject != null)
        {
            Transform t = Selection.activeGameObject.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale    = Vector3.one;
        }
    }
}
