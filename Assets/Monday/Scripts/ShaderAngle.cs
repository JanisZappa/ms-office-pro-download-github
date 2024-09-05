using UnityEngine;

[ExecuteInEditMode]
public class ShaderAngle : Singleton<ShaderAngle>
{
    [Range(0, 1)]
    public float tilt;
    public float angle;
    
    public bool twice;
    
    private static readonly int Squash = Shader.PropertyToID("Squash");
    
    private readonly FloatForce force = new FloatForce(200).SetSpeed(360).SetDamp(48).SetValue(45);
    private int aPick;
    public static float TiltAngle;
    public static Vector3 Right;
    public static float squash;
    
    
    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            if(Input.GetButtonDown("Fire2"))
                aPick++;
            if(Input.GetButtonDown("Fire1"))
                aPick--;
            
            angle = force.Update(45 + aPick * 45 * (twice? 2 : 1), Time.deltaTime);
        }
       
        
        TiltAngle = 22.5f + 22.5f * tilt;
        transform.localRotation = Quaternion.Euler(new Vector3(TiltAngle, angle, 0));
        Right = transform.right;
        
        Vector3 f = transform.forward;
        Vector3 f2 = f.SetY(0).normalized;
        
        squash = 1f / Vector3.Dot(f, f2);
        Shader.SetGlobalFloat(Squash, squash);
    }
    
    
    public static Vector3 MoveDir
    {
        get
        {
            if(Mathf.Abs(squash) < .0001f)
                return Vector3.zero;
            
            Vector3 screenSteer = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            screenSteer.y /= squash;
            
            
            Transform trans = Inst.transform;
            Vector3 worldSteer = trans.TransformDirection(screenSteer);
            
            Vector3 worldX = Vector3.Dot(worldSteer, Vector3.right) * Vector3.right;
            Vector3 worldZ = Vector3.Dot(worldSteer, Vector3.forward) * Vector3.forward;
            
            return worldX + worldZ;
        }
    }
}
