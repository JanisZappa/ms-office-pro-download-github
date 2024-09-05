using UnityEngine;
using Random = System.Random;


public class MakeSomeNoise : MonoBehaviour
{
    public int seed;
    public bool liveEdit;
    
    public Texture2D tex;
    private static readonly int noiseTex = Shader.PropertyToID("NoiseTex");
    private int screenX, screenY;
    
    private void Start()
    {
        CreateTexture();
    }


    private void Update()
    {
        if((screenX != Screen.width  || screenY != Screen.height) || (liveEdit && Input.GetKeyDown(KeyCode.T)))
            CreateTexture();
    }


    private void CreateTexture()
    {
        Random r = new Random(seed);
        
        float aspect = Screen.width * 1f /  Screen.height;
        const int y = 530;  //450
        int x = Mathf.RoundToInt(y * (Screen.width * 1f / Screen.height));
        int count = x * y;
        Color[] pixels = new Color[count];
        
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = new Vector2((i % x - .5f) / x, (Mathf.Floor(i * 1f / x) - .5f) / y);
            //float box = Mathf.Min(Mathf.Min(pos.x, 1f - pos.x) * aspect, Mathf.Min(pos.y, 1f - pos.y));
                  //box = 1f - Mathf.Pow(1.0f- Mathf.Min(1, box * 3), 10);
                  //box = .8f + .2f * box;
            
            Vector2 dir = pos - Vector2.one * .5f;
            
                    dir.x *= aspect; 
            float mag  = dir.magnitude;
            float ring = Mathf.Clamp01(1f - Mathf.Pow(mag * .9225f * 1.01f, 2));
                 
            pixels[i] = new Color(r.Range(0, 1f), r.Range(0, 1f), ring, 0);
        }
           
        tex = new Texture2D(x, y, TextureFormat.RGBA32, false) {filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Repeat};
        
        tex.SetPixels(pixels);
        tex.Apply();
        
        Shader.SetGlobalTexture(noiseTex, tex);
        
        screenX = Screen.width;
        screenY = Screen.height;
    }
}
