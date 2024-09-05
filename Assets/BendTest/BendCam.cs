using UnityEngine;


public class BendCam : MonoBehaviour
{
    private Transform trans;
    private Vector3 offset;
    private Quaternion lookRot, rot = Quaternion.identity;
    
    private readonly Vector3Force posForce = new Vector3Force(861, 81);
    private readonly QuaternionForce rotForce = new QuaternionForce(779, 78);
    private readonly FloatForce twistForce  = new FloatForce(40, 1f * 3 * .5f);
    private readonly FloatForce twistForce2 = new FloatForce(20, .8f * 5 * .5f);
    
    private readonly Vector3Force velocityForce  = new Vector3Force(40, 1f * 3 * .4f);
    private readonly Vector3Force velocityForce2 = new Vector3Force(20, .8f * 5 * .75f);
    
    private static readonly int force = Shader.PropertyToID("TwistForces");
    private static readonly int center = Shader.PropertyToID("Center");
    private float spin;
    
    private Vector3 pos, mV;

    private static float Steer
    {
        get
        {
            return Input.GetAxis("Horizontal") + Keys.Horizontal_Arrows;
        }
    }
    
    private static float Accel
    {
        get
        {
            return Input.GetAxis("Vertical") + Keys.Vertical_Arrows;
        }
    }
    
    private static float Strafe
    {
        get
        {
            return  Keys.Horizontal_AD;
        }
    }
    
    private Stepper stepper;
    private static readonly int velocity = Shader.PropertyToID("Velocity");
    
    private bool slowMo;


    private void Start()
    {
        trans   = transform;
        lookRot = trans.localRotation;
        offset  = trans.localPosition;
        
        posForce.SetValue(offset);
        rotForce.SetValue(lookRot);
        
        stepper = new Stepper(200, Step);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
            slowMo = !slowMo;
        
        stepper.Update(Time.deltaTime);
        
        trans.localPosition = posForce.Value;
        trans.localRotation = rotForce.Value;
        
        Shader.SetGlobalVector(force, 
            new Vector2(twistForce.Value, 
                        twistForce2.Value));
        
        trans.parent.position = pos;
        Shader.SetGlobalVector(center, pos);
        
        Vector3 a = velocityForce.Value;
        Vector3 b = velocityForce2.Value;
        Shader.SetGlobalVector(velocity, new Vector4(a.x, a.z, b.x, b.z));
    }

    private void Step(float dt)
    {
        Vector3 oldP = pos;
        float speed = slowMo? .45f : 1;
        spin = Mathf.Lerp(spin, Steer * 120 * speed, dt * 6);
        rot = rot * Quaternion.AngleAxis(spin * dt, Vector3.up);
        
        posForce.Update(rot * offset, dt);
        rotForce.Update(rot * lookRot, dt);
        
        Quaternion twist = rotForce.Force;
        Vector3 f = twist * Vector3.forward;
        f.y = 0;
        f = f.normalized;
        
        float a  = Vector3.SignedAngle(Vector3.forward, f, Vector3.up);
        float aF = a / dt * .01f * 5;
        
        twistForce.Update(aF, dt); 
        twistForce2.Update(aF, dt);
        
        mV *= 1 - dt * .975f;
        mV += rot * Vector3.forward * Accel * dt * 25 * speed;
        mV += rot * Vector3.right * Strafe * dt * 25 * speed;
        pos += mV * dt;
        
        
        Vector3 vel = (pos - oldP) / dt;
        velocityForce.Update(vel, dt);
        velocityForce2.Update(vel, dt);
    }

    private void OnDisable()
    {
        if(trans != null)
            Shader.SetGlobalVector(center, Vector4.zero); 
    }
}
