using UnityEngine;
using Random = UnityEngine.Random;


public class MovieLook : MonoBehaviour
{
    public ComputeShader compute;
    public int fps;
    
    private Camera cam;
    
    private Camera dummy;
    private Material displayMat;
    
    private RenderTexture renderTex, resultTex, frameAtex, frameBtex;
    
    private int copyKernel, addKernel, frameASaveKernel, frameBSaveKernel, mergeKernel;
    private ComputeBuffer args;
    private float step;
    private float t;
    private bool frameA;
    
    private bool stop;
    private int frame;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        CamSetup();
        step = 1f / fps;
        t = 100;
        
        Shader.DisableKeyword( "NOISE_ON");
        Shader.EnableKeyword( "NOISE_OFF");
    }
    
    
    private void CamSetup()
    {
        CreateDummyCam();
        
        copyKernel       = compute.FindKernel("Copy");
        addKernel        = compute.FindKernel("AddToFrame");
        frameASaveKernel = compute.FindKernel("SaveFrameA");
        frameBSaveKernel = compute.FindKernel("SaveFrameB");
        mergeKernel      = compute.FindKernel("Merge");
        
        
        const int width  = 1280;
        const int height =  720;

        compute.SetInt("ResX", width);
        compute.SetInt("ResY", height);
    
        renderTex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Point, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
        renderTex.Create();
        resultTex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Trilinear, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
        resultTex.Create();
        frameAtex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Trilinear, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
        frameAtex.Create();
        frameBtex = new RenderTexture(width, height, 24) {filterMode = FilterMode.Trilinear, enableRandomWrite = true, format = RenderTextureFormat.ARGB64};
        frameBtex.Create();
    
        cam.targetTexture = renderTex;
    
        ComputeBuffer frameBuffer = new ComputeBuffer(width * height, 16);
        
        compute.SetTexture(copyKernel, "Source", renderTex);
        compute.SetTexture(copyKernel, "Result", resultTex);
        compute.SetTexture(addKernel, "Source", renderTex);
        
        compute.SetTexture(frameASaveKernel, "FrameAResult", frameAtex);
        compute.SetTexture(frameBSaveKernel, "FrameBResult", frameBtex);
        
        compute.SetBuffer(addKernel,        "FrameBuffer", frameBuffer);
        compute.SetBuffer(frameASaveKernel, "FrameBuffer", frameBuffer);
        compute.SetBuffer(frameBSaveKernel, "FrameBuffer", frameBuffer);
        
        compute.SetTexture(mergeKernel, "FrameAResult", frameAtex);
        compute.SetTexture(mergeKernel, "FrameBResult", frameBtex);
        compute.SetTexture(mergeKernel, "Result", resultTex);
        
        displayMat.mainTexture = resultTex;
    
        args = new ComputeBuffer(1, 16, ComputeBufferType.IndirectArguments);
        args.SetData(new[]{Mathf.CeilToInt(width / 16f), Mathf.CeilToInt(height / 16f), 1, 0});
        
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
        if(Input.GetKeyDown(KeyCode.F12))
            stop = !stop;
        
        if(stop)
            return;
        
        Transform camTrans = cam.transform;
        Quaternion camRot = camTrans.rotation;
        Vector3 camPos = camTrans.position;
        const float amount = .0004f;
        Vector3 a = Quaternion.AngleAxis(frame++ * 90, Vector3.forward) * new Vector3(amount, amount, 1).normalized;
        
        Quaternion mixRot = camRot * Quaternion.LookRotation(a, Vector3.up);;
        camTrans.rotation = mixRot;
        camTrans.position = camPos + camRot * Vector3.forward - mixRot * Vector3.forward;
        
        cam.Render();
        camTrans.rotation = camRot;
        camTrans.position = camPos;
        
        compute.SetVector("NoiseOffset", new Vector2(Random.Range(-1000f, 1000), Random.Range(-1000f, 1000)));
        
        t += Time.deltaTime;
        if (t >= step)
        {
            t = t % step;
            
            compute.SetFloat("Weight", (step - t) / step);
            compute.DispatchIndirect(addKernel, args);
            compute.DispatchIndirect(frameA ? frameASaveKernel : frameBSaveKernel, args);
            
            compute.SetFloat("Weight", (t / step) * (t / step));
            compute.DispatchIndirect(addKernel, args);
            
            frameA = !frameA;
        }
        else
        {
            compute.SetFloat("Weight", 1 * (t / step));
            compute.DispatchIndirect(addKernel, args);
        }
        
        float blend = Mathf.Clamp01(t * 2 / step);
        if(frameA)
            blend = 1f - blend;
        
        blend = 1f - blend;
        
        compute.SetFloat("Blend", blend);
        compute.SetVector("NoiseOffset", new Vector2(Random.Range(-1000f, 1000), Random.Range(-1000f, 1000)));
        compute.DispatchIndirect(mergeKernel, args);
    }
}
