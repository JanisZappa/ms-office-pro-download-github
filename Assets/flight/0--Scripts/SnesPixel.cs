using UnityEngine;

public class SnesPixel : MonoBehaviour
{
    public Camera cam;
    public ComputeShader colorMix;
    public Texture2D palette;
    private Texture2D current;
    
    [Space]
    [Range(0, 1)]
    public float bars;
    
    [Range(0, 1)]
    public float colorCorrect;
    
    [Space]
    public Texture2D palleteTex;
    
    private Camera cam2;
    private Transform quad;
    private RenderTexture tex, resultTex, storeTex;

    private const int XRes = 384, YRes = 190, XP = 5, YP = 4, XF = 6, YF = 5;
    public const int FrameCount = XF * YF;
    
    private Material pixelMat;
    private static readonly int Amount  = Shader.PropertyToID("_Amount");
    private static readonly int xRes    = Shader.PropertyToID("XRes");
    private static readonly int yRes    = Shader.PropertyToID("YRes");
    private static readonly int mainTex = Shader.PropertyToID("_MainTex");
    
    private int mixK, gradeK, createK;
    private ComputeBuffer argsBuffer, paletteBuffer;
    
    private int amount;
    private bool barClosed, hard, stop, showAll;
    
    private int frame;
    private static readonly int paletteTex = Shader.PropertyToID("PaletteTex");


    private void Start()
    {
        mixK     = colorMix.FindKernel("Mix");
        gradeK   = colorMix.FindKernel("Grade");
        createK  = colorMix.FindKernel("CreateColors");
            
        cam2 = GetComponent<Camera>();
        quad = transform.GetChild(0);
        
        tex = new RenderTexture(XRes * XF, YRes * YF, 8, RenderTextureFormat.ARGB32)
            {filterMode = FilterMode.Point, useMipMap = false};
        
        storeTex = new RenderTexture(XRes * 4, YRes * 4, 8, RenderTextureFormat.ARGB32) 
            {filterMode = FilterMode.Point, enableRandomWrite = true};
        
        storeTex.Create();
        
        resultTex = new RenderTexture(XRes, YRes, 8, RenderTextureFormat.ARGB32) 
            {filterMode = FilterMode.Point, enableRandomWrite = true};
        
        resultTex.Create();
        
        
        
        
        colorMix.SetMultTexture("Store", storeTex, mixK, gradeK);
        colorMix.SetMultTexture("Result", resultTex, gradeK);
        colorMix.SetTexture(mixK, "Frame", tex);
        
        paletteBuffer = Buff.New(256 * 256 * 256, 4 * 4);
        colorMix.SetMultiBuffer("Palette", paletteBuffer, gradeK, createK);
        
        colorMix.SetInt("xRes",   XRes);
        colorMix.SetInt("yRes",   YRes);
        colorMix.SetInt("pixels", XRes * YRes);
        
        argsBuffer = Buff.New(
            new[]
            {
                Mathf.CeilToInt(XRes * 1f / 32), Mathf.CeilToInt(YRes * 1f / 32), 1, 0
            }, 16, ComputeBufferType.IndirectArguments);
        
        
        cam.targetTexture = tex;
        
        pixelMat = quad.GetComponent<MeshRenderer>().material;
        pixelMat.SetTexture(mainTex, resultTex);
        Shader.SetGlobalInt(xRes, XRes);
        Shader.SetGlobalInt(yRes, YRes);
        
        Debug.LogFormat("SNES-Res: {0} : {1} : {2}", XRes, YRes, XRes * YRes);
        
        int frameWeight = 0;
        for (int i = 0; i < FrameCount; i++)
            frameWeight += FrameCount - i;
        
        colorMix.SetFloat("FrameWeight", 1f / frameWeight);
        
        amount = 1;
        pixelMat.SetFloat(Amount, amount % 4);
    }

    
    private void Update()
    {
        if (palette != current)
        {
            Color[] pixels = palette.GetPixels();
           
            colorMix.SetBuffer(createK, "SourceColors", Buff.New(pixels, 4 * 4));
            colorMix.SetInt("sourceCount", pixels.Length);
            colorMix.Dispatch(createK, 32, 32, 32);
            current = palette;
            
            Color[] result = new Color[256 * 256 * 256];
            paletteBuffer.GetData(result);
            palleteTex = new Texture2D(4096, 4096){ filterMode = FilterMode.Point };
            palleteTex.SetPixels(result);
            palleteTex.Apply();
            
            colorMix.SetTexture(gradeK, "PaletteTex", palleteTex);
            
            Shader.SetGlobalTexture(paletteTex, palleteTex);
        }
        
        if(FlightCore.PauseToggle)
            stop = !stop;
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            hard = !hard;
            cam.allowMSAA = !hard;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            amount++;
            pixelMat.SetFloat(Amount, amount % 4);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha8))
        {
            showAll = !showAll;
            pixelMat.SetTexture(mainTex, showAll? tex : resultTex);
        }
        
        if(Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.JoystickButton6))
            colorCorrect = colorCorrect < .5f? 1 : 0;
        
        float aspect = Screen.width * 1f / Screen.height;
        const float yScale = XRes * XP * 1f / (YRes * YP * 1f);
        float yFactor = aspect / yScale;
        
        cam2.orthographicSize = Screen.height * .5f;
        quad.localScale       = new Vector3(Screen.width, Screen.height * yFactor);
    }


    private void LateUpdate()
    {
        if(stop)
            return;
        
        if(Input.GetKeyDown(KeyCode.B))
            barClosed = !barClosed;
        
        bars = Mathf.Clamp01(bars + Time.deltaTime * (barClosed? 1 : -1) * (1f / YRes) * 8);
        
        colorMix.SetFloat("Bars", bars);
        colorMix.SetFloat("Correction", colorCorrect);
        colorMix.SetInt("F", frame++);
        colorMix.DispatchIndirect(mixK, argsBuffer);
        colorMix.DispatchIndirect(gradeK, argsBuffer);
        
    }
}
