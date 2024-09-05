using UnityEngine;

public class CropCanvas : MonoBehaviour
{
    public int res;
    public float size;

    [Space] 
    public ComputeShader compute;
    
    [Space]
    public Texture2D stamp;

    public RenderTexture result;

    private static readonly int Stamp      = Shader.PropertyToID("_Stamp");
    private static readonly int CanvasSize = Shader.PropertyToID("CanvasSize");

    private int init, update, addstamp;

    private ComputeBuffer args;

    private bool adding;
    private Camera cam;
    
    private readonly Vector2Force force = new Vector2Force(300).SetSpeed(300).SetDamp(33);
    private Vector2 pos, target;


    public Vector3 Pos => new Vector3(pos.x * size - size * .5f, 0, pos.y * size - size * .5f);


    private void Start()
    {
        init   = compute.FindKernel("Init");
        update = compute.FindKernel("Update");
        addstamp = compute.FindKernel("AddStamp");
        
        
        RenderTexture goal = new RenderTexture(res, res, 8) {enableRandomWrite = true, };
        goal.Create();
        
        compute.SetTexture(init, "Goal", goal);
        compute.SetTexture(update, "Goal", goal);
        compute.SetTexture(addstamp, "Goal", goal);
        
        
        RenderTexture motion = new RenderTexture(res, res, 8) {enableRandomWrite = true, };
        motion.Create();
        
        compute.SetTexture(init, "Motion", motion);
        compute.SetTexture(update, "Motion", motion);
        
        
        result = new RenderTexture(res, res, 8) {enableRandomWrite = true, filterMode = FilterMode.Trilinear};
        result.Create();
        
        compute.SetTexture(init, "Result", result);
        compute.SetTexture(update, "Result", result);
        
        
        compute.SetTexture(update, "Stamp", stamp);
        compute.SetTexture(addstamp, "Stamp", stamp);
        
        
        Shader.SetGlobalTexture(Stamp, result);
        Shader.SetGlobalFloat(CanvasSize, size);

        args = Buff.Add(compute.SetupIndirect(res * res, "res", 8));
        compute.DispatchIndirect(init, args);

        cam = Camera.main;
    }


    public void SetTarget(Vector3 target)
    {
        this.target = new Vector2(target.x / size + .5f, target.z / size + .5f);
    }
    
    private void LateUpdate()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            adding = !adding;
            compute.DispatchIndirect(adding ? addstamp : init, args);
        }*/

        /*Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (new Plane(Vector3.down, Vector3.zero).Raycast(r, out float enter))
        {
            Vector3 p = r.origin + r.direction * enter;
            p = (p + new Vector3(size * .5f, 0, size * .5f)) / size;
            
        }*/
        
        pos = force.Update(new Vector2(target.x, target.y), Time.deltaTime);
        compute.SetVector("offset", pos);
        
        // compute.DispatchIndirect(addstamp, args);
        
        compute.DispatchIndirect(update, args);
        Shader.SetGlobalFloat(CanvasSize, size);
    }
}
