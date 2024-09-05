using UnityEngine;

//[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class PostFX : MonoBehaviour
{
    public float amount;
    public float yMulti;
    
    [Space]
    
    public Material material;

    private static readonly int xyAmount = Shader.PropertyToID("XYAmount");

    private void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        Shader.SetGlobalVector(xyAmount, new Vector2(amount, yMulti));
        Graphics.Blit (source, destination, material);
    }
}
