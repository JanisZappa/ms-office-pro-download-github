using UnityEngine;

[ExecuteInEditMode]
public class LightDir : MonoBehaviour
{
    private static readonly int toLight = Shader.PropertyToID("toLight");
    private static readonly int toWorld = Shader.PropertyToID("toWorld");

    private static readonly int lightDir = Shader.PropertyToID("lightDir");

    private void Update()
    {
        Shader.SetGlobalMatrix(toLight, transform.worldToLocalMatrix);
        Shader.SetGlobalMatrix(toWorld, transform.localToWorldMatrix);
        
        Shader.SetGlobalVector(lightDir, transform.forward);
    }
}
