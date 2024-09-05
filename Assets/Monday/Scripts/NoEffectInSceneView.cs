using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoEffectInSceneView : MonoBehaviour
{
    private readonly int _isSceneViewID = Shader.PropertyToID("_IsSceneView");
 
    public void OnEnable()
    {
        Camera.onPreRender += SetIfSceneViewCamera;
    }
 
    public void OnDisable()
    {
        Camera.onPreRender -= SetIfSceneViewCamera;
    }
 
    public void SetIfSceneViewCamera(Camera cam)
    {
        if (cam.gameObject.name == "SceneCamera")
        {
            Shader.EnableKeyword("SCENE_VIEW");
            Shader.SetGlobalFloat(_isSceneViewID, 1f);
        }
        else
        {
            Shader.DisableKeyword("SCENE_VIEW");
            Shader.SetGlobalFloat(_isSceneViewID, 0f);
        }
    }
}
