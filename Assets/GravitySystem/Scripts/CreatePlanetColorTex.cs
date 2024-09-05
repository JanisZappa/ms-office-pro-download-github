using UnityEngine;

public class CreatePlanetColorTex : MonoBehaviour
{
    [Range(0, 1)]
    public float min, max;
    private static readonly int ColorTex = Shader.PropertyToID("_ColorTex");

    private const int steps = 8, total = steps * steps * steps;
    private readonly Color[] colors = new Color[total];
    private Texture3D tex;
    private static readonly int NoiseShift = Shader.PropertyToID("NoiseShift");

    private void Start()
    {
        tex = new Texture3D(steps, steps, steps, TextureFormat.ARGB32, false) { filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Repeat};
        NewColors();
        
        Shader.SetGlobalTexture(ColorTex, tex);
    }


    private void NewColors()
    {
        float mi = Mathf.Min(min, max);
        float ma = Mathf.Max(min, max);
        for (int i = 0; i < total; i++)
            colors[i] = new Color(Random.Range(mi, ma), Random.Range(mi, ma), Random.Range(mi, ma), 1);
        
        tex.SetPixels(colors);
        tex.Apply();
        
        Shader.SetGlobalFloat(NoiseShift, Random.Range(0, 1000f));
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
            NewColors();
    }
}
