using UnityEngine;

public class GBAPixel : MonoBehaviour
{
    public Camera cam;
    public ComputeShader compute;
    public RenderTexture tex2, tex3;
    
    [Space]
    public bool direct;
    
    private Camera cam2;
    private Transform quad;
    //private const int XRes = 384, YRes = 190;
    private const int YRes = 288, XRes = 640;
    private int CopyIntoBiggerTex, Blur;
    private ComputeBuffer argsBuffer;
    private static readonly int squash = Shader.PropertyToID("Squash");
    private static readonly int xRes = Shader.PropertyToID("XRes");
    private static readonly int yRes = Shader.PropertyToID("YRes");

    private const int multi = 2;
    
    private void Start()
    {
        cam2 = GetComponent<Camera>();
        cam2.enabled = true;
        
        quad = transform.GetChild(0);
        
        RenderTexture tex = new RenderTexture(XRes, YRes, 0, RenderTextureFormat.ARGB32)
            {filterMode = FilterMode.Bilinear};
        
        cam.targetTexture = tex;
        cam.enabled = false;

        if(direct)
            quad.GetComponent<MeshRenderer>().material.mainTexture = tex;
        else
        {
            tex2 = new RenderTexture(XRes * multi, YRes * multi, 0, RenderTextureFormat.ARGB32)
                {filterMode = FilterMode.Trilinear, enableRandomWrite = true};
            tex2.Create();
    
            tex3 = new RenderTexture(XRes * multi, YRes * multi, 0, RenderTextureFormat.ARGB32)
                {filterMode = FilterMode.Trilinear, enableRandomWrite = true};
            tex3.Create();
    
            CopyIntoBiggerTex = compute.FindKernel("CopyIntoBiggerTex");
            Blur = compute.FindKernel("Blur");
           
        
            compute.SetTexture(CopyIntoBiggerTex, "Source", tex);
            compute.SetTexture(CopyIntoBiggerTex, "Result", tex2);
            compute.SetTexture(Blur, "Result", tex2);
            compute.SetTexture(Blur, "BlurResult", tex3);
        
            compute.SetInt("Multi",  multi);
            compute.SetInt("XRes", XRes * multi);
            compute.SetInt("YRes", YRes * multi);
            
            argsBuffer = Buff.New(
                new[]
                {
                    Mathf.CeilToInt(XRes * multi * 1f / 32), 
                    Mathf.CeilToInt(YRes * multi * 1f / 32), 1, 0
                }, 16, ComputeBufferType.IndirectArguments);
        
            quad.GetComponent<MeshRenderer>().material.mainTexture = tex3;
        }
        float toShader = (XRes * 1.0f /16 * 9) / YRes - 1;
        //Debug.Log(toShader);
        Shader.SetGlobalFloat(squash, toShader);
        Shader.SetGlobalFloat(xRes, XRes);
        Shader.SetGlobalFloat(yRes, YRes);
    }


    private void OnDisable()
    {
        Shader.SetGlobalFloat(squash, 0);
    }

    
    private void Update()
    {
        cam2.orthographicSize = Screen.height * .5f;
        quad.localScale       = new Vector3(Screen.width, Screen.height);
    }


    private void LateUpdate()
    {
        cam.enabled = true;
        cam.Render();
        cam.enabled = false;
        
        if(direct)
            return;
        
        compute.DispatchIndirect(CopyIntoBiggerTex, argsBuffer);
        compute.DispatchIndirect(Blur, argsBuffer);
        //compute.DispatchIndirect(Compose, argsBuffer);
    }
}
