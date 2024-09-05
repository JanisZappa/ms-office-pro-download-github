using System.IO;
using UnityEngine;


public class CaveWater : MonoBehaviour
{
    public Color color;
    public TextAsset waterData;
    public Texture2D waterTex;
    
    [Space]
    public Vector3 min;
    public float size;
    
    
    private static readonly int waterLevel = Shader.PropertyToID("WaterLevel");
    private static readonly int waterColor = Shader.PropertyToID("WaterColor");
    private static readonly int tex        = Shader.PropertyToID("WaterTex");
    private static readonly int waterSize  = Shader.PropertyToID("WaterSize");
    private static readonly int waterMin   = Shader.PropertyToID("WaterMin");


    private void Start()
    {
        Shader.SetGlobalFloat(waterLevel, transform.position.y);
        Shader.SetGlobalColor(waterColor, color);
        Shader.SetGlobalTexture(tex, waterTex);
        
        using (MemoryStream m = new MemoryStream(waterData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            min  = new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) * 20;
            size = r.ReadSingle() * 20;
           
            Shader.SetGlobalFloat(waterSize, 1.0f / size);
            Shader.SetGlobalVector(waterMin, min);
        }
    }

    
    private void Update()
    {
        Shader.SetGlobalColor(waterColor, color);
    }
}
