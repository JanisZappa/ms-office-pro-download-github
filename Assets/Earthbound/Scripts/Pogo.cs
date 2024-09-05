using UnityEngine;


public class Pogo : MonoBehaviour
{
    public float gravity, accel, bufferLength, stickLength, extra;
    
    [Space]
    public Transform camTrans;
    
    
    private Stepper stepper;
    
    private Vector3 pos, mV;
    private Quaternion rot = Quaternion.identity;
    private readonly QuaternionForce rotForce = new QuaternionForce(80, 7);
    
    private float force;
    private float dist;
    
    private Vector3 camOffset, camPos;
    
    
    private static Vector3 SteerVector
    {
        get
        {
            Vector3 steer = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            float mag = steer.magnitude;
            if (mag > 1)
                steer /= mag;
            
            return steer;
        }
    }
    
    
    private void Start()
    {
        stepper = new Stepper(200, StepUpdate);
        pos     = Vector3.up * bufferLength;
        
        if (Physics.Raycast(pos + Vector3.up * 100, Vector3.down, out RaycastHit hitInfo, 1000, 1 << 0))
            pos = hitInfo.point + Vector3.up * bufferLength;
        
        camOffset = camTrans.position;
    }

    
    private void StepUpdate(float dt)
    {
    //  Damping and Gravity  //
        mV *= 1 - dt * .75f;
        mV += Vector3.down * gravity * dt;
        
    //  Steer  //
        Vector3 steer = SteerVector;
            
        float leanMag = steer.magnitude;
            
        bool isLeaning = leanMag > .001f;
        Quaternion bodyLean = isLeaning? 
            Quaternion.AngleAxis(leanMag * 25, Vector3.Cross(Vector3.up, steer / leanMag).normalized) : 
            Quaternion.identity;
        
        rot = rotForce.Update(bodyLean, dt);
        
    //  Force  //
        Vector3 hitN = HitCheckN(out dist);
        bool applyForce = dist <= bufferLength;
        force = applyForce ? force + accel * dt : 0;

        if (applyForce)
        {
            float l = 1 - dist / bufferLength;
                  l = Mathf.Lerp(l, Mathf.Pow(l, 2), .5f);
              
            float multi = 1 + (Input.GetKey(KeyCode.Space)? extra : 0);
            
            Vector3 stickUp = rot * Vector3.up;
            Vector3 dir = Vector3.Slerp(stickUp, hitN, .45f);
            
            float amount = force * dt * (1 + l * 10f) * multi;
            mV += dir * amount;
            
            Quaternion impactRot = Quaternion.SlerpUnclamped(Quaternion.identity, Quaternion.FromToRotation(hitN, stickUp), amount * .001f);
            rotForce.AddForce(impactRot);
        }
        
    //  Pos  //
        pos += FloorAdjust(mV * dt);
    }


    private Vector3 HitCheckN(out float dist)
    {
        if (Physics.Raycast(pos + rot * Vector3.up * stickLength, rot * Vector3.down, out RaycastHit hitInfo, bufferLength + stickLength, 1 << 0))
        {
            dist = (pos - hitInfo.point).magnitude;
            return hitInfo.normal;
        }
        
        dist = float.MaxValue;
        return Vector3.up;
    }


    public Vector3 FloorAdjust(Vector3 mV)
    {
        float mag = mV.magnitude;
        
        if(mag < .0001f)
            return Vector3.zero;
        
        
        Vector3 dir = mV / mag;
        
        if (Physics.Raycast(pos, dir, out RaycastHit hitInfo, mag, 1 << 0))
            mag = Mathf.Min(mag, Mathf.Max(0, hitInfo.distance - .01f));
        
        return dir * mag;
    }
    
    
    private void Update()
    {
        stepper.Update(Time.deltaTime);
        
        
        DRAW.Vector(pos, rot * Vector3.up * stickLength).SetColor(COLOR.purple.magenta);
        DRAW.Zapp(pos, rot * Vector3.down * Mathf.Min(dist, bufferLength), 10, bufferLength * .1f, bufferLength).SetColor(COLOR.green.lime);
        
        camPos = Vector3.Lerp(camPos, pos.SetY(0), Time.deltaTime * 3);
        camTrans.position = camOffset + camPos;
    }
}
