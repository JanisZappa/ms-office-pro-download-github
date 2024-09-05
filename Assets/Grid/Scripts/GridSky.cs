using UnityEngine;


[ExecuteInEditMode]
public class GridSky : MonoBehaviour
{
    public Texture2D tex;
    public float texScale;

    private static readonly int SkyTex = Shader.PropertyToID("SkyTex");
    private static readonly int SkyScale = Shader.PropertyToID("SkyScale");

    
    private void OnEnable()
    {
        Shader.SetGlobalTexture(SkyTex, tex);
    }

    
    private void Update()
    {
        Shader.SetGlobalFloat(SkyScale, texScale);
    }
}
