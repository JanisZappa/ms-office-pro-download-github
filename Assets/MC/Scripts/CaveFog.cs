using UnityEngine;


[ExecuteInEditMode]
public class CaveFog : MonoBehaviour
{
    private Camera cam;
    private static readonly int caveFog    = Shader.PropertyToID("CaveFog");
    private static readonly int caveFogMax = Shader.PropertyToID("CaveFogMax");


    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    
    private void Update()
    {
        Shader.SetGlobalFloat(caveFog, 1f / 1000f);
        Shader.SetGlobalFloat(caveFogMax, cam.farClipPlane);
    }
}
