using UnityEngine;


public class FilmBlur : MonoBehaviour
{
    public Camera cam;
    
    [Range(1, 8)]
    public int resDiv = 1;
    
    [Space]
    public int fps;
    public int blurSteps;
    
    public float offset;
    
    public bool enableBlur;
    
    [Space]
    public ComputeShader compute;
   
    
    public delegate void TimeSet(float time);
    public static event  TimeSet onTimeSet;
    
    public delegate void Trimm();
    public static event  Trimm onTrimm;
    
    private bool msaa;
    private RenderTexture renderTex, resultTex;
    private int clearKernel, addKernel, resultKernel;
    private ComputeBuffer args;
    private Material displayMat;
    private Camera dummy;
    private Vector3 localCamPos;
    
    
    private void Start()
    {
        clearKernel  = compute.FindKernel("Clear");
        addKernel    = compute.FindKernel("Add");
        resultKernel = compute.FindKernel("Finalize");
        
        msaa = cam.allowMSAA;
        CamSetup();
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            enableBlur = !enableBlur;
            CamSetup();
        }
    }


    private void CamSetup()
    {
        //cam.allowMSAA = !enableBlur && msaa;

        if (enableBlur)
        {
            CreateDummyCam();
            
            int width  = Mathf.RoundToInt(Screen.width  * 1f / resDiv);
            int height = Mathf.RoundToInt(Screen.height * 1f / resDiv);
        
            compute.SetInt("ResX", width);
            compute.SetInt("ResY", height);
            compute.SetFloat("Div", 1f / blurSteps);
        
            renderTex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Point, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
            renderTex.Create();
            resultTex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Trilinear, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
            resultTex.Create();
        
            cam.targetTexture = renderTex;
        
            ComputeBuffer buffer = new ComputeBuffer(width * height, 16);
            compute.SetBuffer(clearKernel,  "Math", buffer);
            compute.SetBuffer(addKernel,    "Math", buffer);
            compute.SetBuffer(resultKernel, "Math", buffer);
        
            compute.SetTexture(clearKernel,  "Result", resultTex);
            compute.SetTexture(addKernel,    "Result", resultTex);
            compute.SetTexture(resultKernel, "Result", resultTex);
        
            compute.SetTexture(addKernel, "Source", renderTex);
        
            displayMat.mainTexture = resultTex;
        
            args = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
            args.SetData(new[]{Mathf.CeilToInt(width / 16f), Mathf.CeilToInt(height / 16f), 1, 0});
        }
        else
        {
            Destroy(dummy);
            cam.targetTexture = null;
        }
    }
    
    
    private void CreateDummyCam()
    {
        dummy = new GameObject("Dummy").AddComponent<Camera>();
        dummy.cullingMask = 1 << 5;
        dummy.orthographic = true;
        dummy.orthographicSize = .5f;
        dummy.clearFlags = CameraClearFlags.Nothing;
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        Destroy(quad.GetComponent<Collider>());
        quad.layer = LayerMask.NameToLayer("UI"); 
        quad.transform.localPosition = Vector3.forward;
        quad.transform.localScale    = new Vector3(Screen.width * 1f / Screen.height, 1, 1);
        displayMat = Resources.Load<Material>("DisplayMat");
        quad.GetComponent<MeshRenderer>().material = displayMat;
        quad.transform.SetParent(dummy.transform, true);
    }



    private void LateUpdate()
    {
        if(enableBlur)
        {
            compute.DispatchIndirect(clearKernel, args);
            
            float step = 1f / fps / (blurSteps - 1);
            for (int i = blurSteps - 1; i > -1; i--)
            {
                onTimeSet?.Invoke(Time.unscaledTime - step * i);
                  
                cam.Render();
                
                compute.DispatchIndirect(addKernel, args);
            }
            
            compute.DispatchIndirect(resultKernel, args);
            
            onTrimm?.Invoke();
        }
    }
}
