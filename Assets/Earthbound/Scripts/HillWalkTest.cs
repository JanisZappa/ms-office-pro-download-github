using System;
using System.Collections;
using UnityEngine;

public class HillWalkTest : MonoBehaviour
{
    public float speed;
    public float legLength;
    
    private Stepper stepper;
    private readonly Vector3Force    rootForce  = new Vector3Force(300, 40);
    private readonly QuaternionForce rotForce   = new QuaternionForce(400, 40).SetValue(QD);
    private Vector3 pos, normal, footL, footR, currentSteer;
    private Quaternion rot = QD;
    private float stepL, stepR;
    
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

        RayCast();
        
        footL = RayCastFoot(rot * Vector3.left  * legLength * .25f);
        footR = RayCastFoot(rot * Vector3.right * legLength * .25f);
    }

    
    private void Update()
    {
        stepper.Update(Time.deltaTime);
        
        DRAW.Vector(pos + Vector3.up * legLength, Vector3.up).SetColor(Color.red);
        DRAW.Vector(pos + Vector3.up * (legLength + .5f), rot * Vector3.forward * .25f).SetColor(Color.red);
        DRAW.Vector(footL, Vector3.up * legLength).SetColor(Color.green);
        DRAW.Vector(footR, Vector3.up * legLength).SetColor(Color.magenta);

        Vector3 center = pos;
        
        //  Prediction  //
        SaveState();

        
        bool standL = Time.unscaledTime >= stepL;
        bool standR = Time.unscaledTime >= stepR;

        if (standL && standR)
        {
            const float step = 1f / 200;
        for (int i = 0; i < 400; i++)
        {
            StepUpdate(step);
            
            float checkT = Time.unscaledTime + i * step;
            bool canUseL = standL;//checkT >= stepL;
            bool canUseR = standR; //checkT >= stepR;
            if(!canUseL && !canUseR)
                continue;
            
            Vector3 posDir = pos - center;

            if (posDir.magnitude >= legLength * .35f)
            {
                Vector3 dirR = pos - footR;
                Vector3 dirL = pos - footL;
                
                float duration = Mathf.Min(checkT - Time.unscaledTime, .5f);
                float stepAny = Time.unscaledTime + duration;
                if (canUseL != canUseR)
                {
                    if (canUseL)
                    {
                        stepL = stepAny;
                        Debug.Log("StepL " + Time.frameCount);
                        Vector3 fL = RayCastFoot(pos + rot * Vector3.left  * legLength * .25f);
                        StartCoroutine(MoveFoot(footL, fL, duration, v => footL = v));
                        break;
                    }
                    
                    {
                        stepR = stepAny;
                        Debug.Log("StepR " + Time.frameCount);
                        Vector3 fR = RayCastFoot(pos + rot * Vector3.right  * legLength * .25f);
                        StartCoroutine(MoveFoot(footR, fR, duration, v => footR = v));
                        break;
                    }
                }
                
                {
                    Vector3 f = rot * Vector3.forward * (legLength * .1f + rootForce.Value.magnitude / speed * .1f);
                    
                    bool useLeftFoot = dirL.sqrMagnitude > dirR.sqrMagnitude;
                    if (useLeftFoot)
                    {
                        stepL = stepAny;
                        Debug.Log("StepL " + Time.frameCount);
                        Vector3 fL = RayCastFoot(pos + rot * Vector3.left  * legLength * .25f + f);
                        StartCoroutine(MoveFoot(footL, fL, duration, v => footL = v));
                        break;
                    }
                    
                    {
                        stepR = stepAny;
                        Debug.Log("StepR " + Time.frameCount);
                        Vector3 fR = RayCastFoot(pos + rot * Vector3.right  * legLength * .25f + f);
                        StartCoroutine(MoveFoot(footR, fR, duration, v => footR = v));
                        break;
                    }
                }
            }
        }
        }
        
        
        
        
        LoadState();
    }
    
    
    private void StepUpdate(float dt)
    {
        Vector3 steer = SteerVector;
        float steerMag = steer.magnitude;
        Vector3 steerN = steer / steerMag;
        bool isSteering = steerMag > .001f;
        
        currentSteer = isSteering? steerN : currentSteer;
        
        if(Mathf.Abs(steer.x) > .001f && Mathf.Abs(normal.x) > .001f && Mathf.Sign(steer.x) != Mathf.Sign(normal.x))
            steer.x = Mathf.Max(0, Mathf.Abs(steer.x) - Mathf.Abs(normal.x)) * Mathf.Sign(steer.x);
        
        if(Mathf.Abs(steer.z) > .001f && Mathf.Abs(normal.z) > .001f && Mathf.Sign(steer.z) != Mathf.Sign(normal.z))
            steer.z = Mathf.Max(0, Mathf.Abs(steer.z) - Mathf.Abs(normal.z)) * Mathf.Sign(steer.z);
        
        
        pos += rootForce.Update(steer * speed, dt) * dt;
        
        Vector3 forward = rot * Vector3.forward, right = rot * Vector3.right;
        
        float a = Vector3.Angle(forward, currentSteer) * Mathf.Sign(Vector3.Dot(currentSteer, right));
        Quaternion aR = Quaternion.AngleAxis(a * steerMag, Vector3.up);
        rot = rotForce.Update(rot * aR, dt);
        
        RayCast();
    }
    
    
    
    private void SaveState()
    {
        ValueSave.Writer.Write(pos);
        ValueSave.Writer.Write(normal);
        rootForce.Save(ValueSave.Writer);
        ValueSave.Writer.Write(rot);
        rotForce.Save(ValueSave.Writer);
        ValueSave.Writer.Write(currentSteer);
    }
    
    private void LoadState()
    {
        pos    = ValueSave.Reader.ReadVector3();
        normal = ValueSave.Reader.ReadVector3();
        rootForce.Load(ValueSave.Reader);
        rot    = ValueSave.Reader.ReadQuaternion();
        rotForce.Load(ValueSave.Reader);
        currentSteer = ValueSave.Reader.ReadVector3();
    }


    private void RayCast()
    {
        if (Physics.Raycast(pos + Vector3.up * 1000, Vector3.down * 10000, out RaycastHit hit))
        {
            pos    = hit.point;
            normal = hit.normal;
        } 
    }
    
    
    private static Vector3 RayCastFoot(Vector3 p)
    {
        return Physics.Raycast(p + Vector3.up * 1000, Vector3.down * 10000, out RaycastHit hit) ? hit.point : p;
    }


    private IEnumerator MoveFoot(Vector3 start, Vector3 end, float duration, Action<Vector3> footBack)
    {
        float s = 1f / duration;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * s;
            footBack.Invoke(Vector3.Lerp(start, end, t));
            yield return null;
        }
    }
    
    
    private static readonly Quaternion QI = Quaternion.identity;
    private static readonly Quaternion QD = Quaternion.LookRotation(Vector3.back);
}
