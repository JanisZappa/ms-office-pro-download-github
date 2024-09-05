using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;


public class MCLights : Singleton<MCLights>
{
    public TextAsset data;
    public ComputeShader compute;
    public TextAsset lightData;
    
    [Space]
    public int steps;
    private int count;
    private Vector3 min;
    public float size;
    
    [Space]
    public Color testColor;
    public float testRadius;
    
    [Space]
    public Color camSampleColor;
        
    
    private Texture3D tex;
    private RenderTexture writeTex;
    private static readonly int bakedLights    = Shader.PropertyToID("BakedLights");
    private static readonly int bakeLightsSize = Shader.PropertyToID("BakeLightsSize");
    private static readonly int bakeLightsMin  = Shader.PropertyToID("BakeLightsMin");
    private static readonly int dynamicLights  = Shader.PropertyToID("DynamicLights");
    
    private int AddKernel;
    private static MCLight[] Lights, BakeLights;
    private ComputeBuffer LightBuffer;
    private static readonly int dynamicMulti = Shader.PropertyToID("DynamicMulti");
    private ComputeBuffer kernelBuffer;
    
    private const int MaxLights = 256;
    private static int overwrite;
    
    private float darkness;
    private bool dark;
    private static readonly int caveDarkness = Shader.PropertyToID("CaveDarkness");
    
    private const float EffectMulti = .5f;
    private bool flicker;
    private int adjust;
    private static readonly int colorAdjust = Shader.PropertyToID("ColorAdjust");
    private static readonly int camBrightness = Shader.PropertyToID("CamBrightness");
    private float camB;
    private readonly FloatForce camBF = new FloatForce(200).SetSpeed(200).SetDamp(60);


    private void Start()
    {
        AddKernel = compute.FindKernel("AddLight");
        
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            steps = r.ReadInt32();
            count = steps * steps * steps;
            Color32[] cols = new Color32[count];
            for (int i = 0; i < count; i++)
                cols[i] = new Color32(r.ReadByte(), r.ReadByte(), r.ReadByte(), 255);
            
        //  Textures  //
            tex = new Texture3D(steps, steps, steps, TextureFormat.ARGB32, true);
            tex.SetPixels32(cols);
            tex.Apply();
            Shader.SetGlobalTexture(bakedLights, tex);
            
            writeTex = new RenderTexture(steps, steps, 0)
            {
                filterMode        = FilterMode.Bilinear, 
                wrapMode          = TextureWrapMode.Clamp, 
                enableRandomWrite = true, 
                dimension         = UnityEngine.Rendering.TextureDimension.Tex3D, 
                volumeDepth       = steps,
            };
            writeTex.Create();
        
            Shader.SetGlobalTexture(dynamicLights, writeTex);
            compute.SetTexture(AddKernel, "Result", writeTex);
            
            
            min  = new Vector3(-r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) * 20;
            size = r.ReadSingle() * 20;
            
            Shader.SetGlobalVector(bakeLightsMin, min);
            Shader.SetGlobalFloat(bakeLightsSize, 1f / size);
            
            compute.SetVector("Min", min.MultiX(-1));
            compute.SetFloat("Step", size / (steps - 1));
            
            
        //  Lights //
            Lights = new MCLight[MaxLights];
            LightBuffer = new ComputeBuffer(MaxLights, Marshal.SizeOf(typeof(MCLight)));
            compute.SetBuffer(AddKernel, "Lights", LightBuffer);
            
            Shader.SetGlobalFloat(dynamicMulti, 1f);
            
            kernelBuffer = new ComputeBuffer(2, 16, ComputeBufferType.IndirectArguments);
            int[] kernelArgs = new int[4 * 2];
            int s = steps / 4;
            kernelArgs[0] = s;
            kernelArgs[1] = s;
            kernelArgs[2] = s;
            kernelBuffer.SetData(kernelArgs);
        }
        
        
        using (MemoryStream m = new MemoryStream(lightData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            BakeLights = new MCLight[MaxLights];
            for (int i = 0; i < MaxLights; i++)
                BakeLights[i] = 
                    new MCLight(
                        new Vector3(-r.ReadSingle(), r.ReadSingle(), r.ReadSingle()) * 20, 
                        new Color32( r.ReadByte(),   r.ReadByte(),   r.ReadByte(), 255), 
                        50f);
        }
        
        adjust = 10;
        Shader.SetGlobalFloat(colorAdjust, adjust * 1f / 10);
        
        Vector3 cam = (Camera.main.transform.position - min.MultiX(-1)) / size;
        camSampleColor = tex.GetPixelBilinear(cam.x, cam.y, cam.z);
        camB = .3f *  camSampleColor.r + .59f *  camSampleColor.g + .11f *  camSampleColor.b;
        camBF.SetValue(camB);
    }


    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
            dark = !dark;
        if(Input.GetKeyDown(KeyCode.Alpha9))
            flicker = !flicker;

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            adjust = (adjust + 1) % 11;
            Shader.SetGlobalFloat(colorAdjust, adjust * 1f / 10);
        }
        
        Vector3 cam = (Camera.main.transform.position - min.MultiX(-1)) / size;
        camSampleColor = tex.GetPixelBilinear(cam.x, cam.y, cam.z);
        //camB = camBF.Update(.3f *  camSampleColor.r + .59f *  camSampleColor.g + .11f *  camSampleColor.b, Time.deltaTime);
        camB = Mathf.Lerp(camB, .3f *  camSampleColor.r + .59f *  camSampleColor.g + .11f *  camSampleColor.b, Time.deltaTime * .75f);
        Shader.SetGlobalFloat(camBrightness, camB);
        
        darkness =  Mathf.Clamp01(darkness + Time.deltaTime * (dark? 1 : -1) * .25f);
        Shader.SetGlobalFloat(caveDarkness, Mathf.SmoothStep(0, .55f, Mathf.SmoothStep(0, 1, darkness)));
        
        
        for (int i = 0; i < MaxLights; i++)
            Lights[i] = BakeLights[i].Flicker(i, flicker);
        
        overwrite = 0;
        MCProjectiles.UpdateShots();
        
        LightBuffer.SetData(Lights);
        
        compute.SetFloat("Delta", Time.deltaTime);
        compute.DispatchIndirect(AddKernel, kernelBuffer);
    }


    public static void Shot(Vector3 pos)
    {
        Lights[overwrite++] = new MCLight(pos, Inst.testColor * EffectMulti, Inst.testRadius);
    }
    
    
    public static void Flash(Vector3 pos, float amount)
    {
        Lights[overwrite++] = new MCLight(pos, Color.white * amount * .7f * EffectMulti, Inst.testRadius * 1.15f);
    }
    
    
    private struct MCLight
    {
        Vector3 pos;
        Color   color;
        float   radius;

        public MCLight(Vector3 pos, Color color, float radius)
        {
            this.pos    = pos;
            this.color  = color;
            this.radius = 1f / radius;
        }


        public MCLight Flicker(int id, bool flicker)
        {
            float speed  = id * 324.011f % .2f + .8f;
            float value  = (Mathf.Sin(id * 315f    + Time.realtimeSinceStartup * speed * 1.1f) * .5f + .5f) * .25f + .75f;
                  value *= (Mathf.Sin(id * 712.2f  + Time.realtimeSinceStartup * speed * 4.123f) * .5f + .45f) * .25f + .75f;
                  value *= (Mathf.Sin(id * 1712.2f + Time.realtimeSinceStartup * speed * 6.1655f) * .5f + .45f) * .25f + .75f;
                  value  = Mathf.Pow(value, 20);
            float amount = value * .375f * 2.25f;
            amount *= flicker? .35f : 0;
            return new MCLight(pos, color * amount, 1f / radius);
        }
    }


    private void OnDisable()
    {
        Shader.SetGlobalFloat(dynamicMulti, 0);
    }
}
