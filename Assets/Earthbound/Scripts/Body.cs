using UnityEngine;

public class Body : MonoBehaviour
{
    public float speed;
    
    [Space]
    public float legLength;
    public float torsoLength;
    public float armLength;
    public float headSize;
    public float hipWidth;
    public float shoulderWidth;
    
    private Vector3 pos;
    
    private Quaternion rot = QD, shoulderRot = QD, headRot = QD, torso = QD, bodyLeanRot = QI, leftLegRot = QD, rightLegRot = QD;

    private readonly Vector3Force    rootForce  = new Vector3Force(300, 40);
    private readonly QuaternionForce rotForce   = new QuaternionForce(400, 40).SetValue(QD);
    
    private readonly QuaternionForce shoulderRotForce = new QuaternionForce(200, 10).SetValue(QD);
    private readonly QuaternionForce headRotForce = new QuaternionForce(400, 16).SetValue(QD);
    
    private readonly Vector3Force    torsoForce    = new Vector3Force(200, 10);
    private readonly QuaternionForce torsoRotForce = new QuaternionForce(100, 10).SetValue(QD);
    
    private readonly QuaternionForce bodyLeanForce = new QuaternionForce(100, 12);
    
    
    private readonly QuaternionForce leftLegForce   = new QuaternionForce(220, 16).SetValue(QD);
    private readonly QuaternionForce rightLegForce  = new QuaternionForce(220, 16).SetValue(QD);
    
    
    private readonly FloatForce crouchForce = new FloatForce(120, 12);
    private Vector3 currentSteer = Vector3.back;

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
    
    private Stepper stepper;
    private float t;
    private const float runGate = 1.5f;
    private float RunRange  { get { return legLength * runGate; }}
    private float crouch;

    private float CrouchLeg
    {
        get
        {
            return (1 - crouch - .005f) * legLength;
        }
    }


    private float WalkMulti
    {
        get
        {
            return 1 - Mathf.Pow(1 - Mathf.Clamp01(rootForce.Value.magnitude / speed * 100), 10);
        }
    }


    private void Start()
    {
        stepper = new Stepper(200, StepUpdate);
        pos = transform.position;
    }

    
    private void Update()
    {
        stepper.Update(Time.deltaTime);
        
        Draw();
    }
    

    private void StepUpdate(float dt)
    {
        Vector3 steer = SteerVector;
        float steerMag = steer.magnitude;
        Vector3 steerN = steer / steerMag;
        bool isSteering = steerMag > .001f;
        
        currentSteer = isSteering? steerN : currentSteer;
        
        Vector3 oldMV   = rootForce.Value / speed;
        Vector3 leanDir = steer - oldMV + steer *.45f;
        float leanMag = leanDir.magnitude;
        
        bool isLeaning = leanMag > .001f;
        Quaternion bodyLean = isLeaning? 
            Quaternion.AngleAxis(Mathf.Pow(leanMag, 2) * speed * 2.5f, Vector3.Cross(Vector3.up, leanDir / leanMag).normalized) : 
            QI;
        
        bodyLeanRot = bodyLeanForce.Update(bodyLean, dt);
        Vector3 slip = bodyLeanRot * Vector3.up;
                slip.y = 0;
        
        
        //pos += slip * 1f * speed * dt * .5f;
        
        Vector3 oldPos = pos;
        pos += rootForce.Update(steer * speed, dt) * dt;
        t += ((pos - oldPos).magnitude / RunRange);
        
        float dip = (.5f - Mathf.Abs(.5f -Mathf.PingPong(t, 1))) * WalkMulti * 3 * (1 - Mathf.Pow(rootForce.Value.magnitude / speed, 2));
        crouch = crouchForce.Update(slip.magnitude * .25f + dip * .15f, dt);
        
        
        
        torsoForce.Update(pos, dt);
        
        
    //  SteerRot  //
        Vector3 forward = rot * Vector3.forward, right = rot * Vector3.right;
        
        float a = Vector3.Angle(forward, currentSteer) * Mathf.Sign(Vector3.Dot(currentSteer, right));
        Quaternion aR = Quaternion.AngleAxis(a * steerMag, Vector3.up);
        
        Vector3 shouderDir = shoulderRot * Vector3.forward;
        float b = Vector3.Angle(forward, shouderDir) * Mathf.Sign(Vector3.Dot(shouderDir, right));
        Quaternion bR = Quaternion.AngleAxis(a * steerMag - b, Vector3.up);
        
        Vector3 headDir = headRot * Vector3.forward;
        float c = Vector3.Angle(forward, headDir) * Mathf.Sign(Vector3.Dot(headDir, right));
        Quaternion cR = Quaternion.AngleAxis(a * steerMag - c, Vector3.up);
        
                rot = rotForce.Update(rot * aR, dt);
        shoulderRot = shoulderRotForce.Update(shoulderRot * bR, dt);
        headRot     = headRotForce.Update(headRot * cR, dt);
        
        torso = QI; //torsoRotForce.Update(torsoLean, dt);
        
        Vector3 leftLegDir = leftLegRot * Vector3.forward;
        float d = Vector3.Angle(forward, leftLegDir) * Mathf.Sign(Vector3.Dot(leftLegDir, right));
        Quaternion dR = Quaternion.AngleAxis(a * steerMag - d, Vector3.up);
        
        leftLegRot = leftLegForce.Update(leftLegRot * dR, dt);
        
        
        Vector3 rightLegDir = rightLegRot * Vector3.forward;
        float e = Vector3.Angle(forward, rightLegDir) * Mathf.Sign(Vector3.Dot(rightLegDir, right));
        Quaternion eR = Quaternion.AngleAxis(a * steerMag - e, Vector3.up);
        
        rightLegRot = rightLegForce.Update(rightLegRot * eR, dt);
        
    }
    
    
    public Vector3 Pos { get { return pos + Vector3.up * (legLength + torsoLength * .5f); }}
    
    private const bool lean = true;
    private void Draw()
    {
        
            DRAW.Vector(ToLeanPos(pos + Vector3.up * (CrouchLeg + torsoLength + headSize + .5f)), ToLeanDir(Vector3.up * .5f)).SetColor(Color.white.A(.6f));
        
            Vector3 p     = pos + (torsoForce.Value - pos) * -.15f;
            Vector3 tP    = p + Vector3.up * (CrouchLeg + torsoLength * .5f);
            Vector3 spine = torso * Vector3.up * torsoLength;
        
            DRAW.Vector(ToLeanPos(tP + torso * Vector3.down * torsoLength * .5f), ToLeanDir(spine)).SetColor(COLOR.red.tomato);
        
            //  Head  //
            Vector3 head = tP + spine * .5f + Vector3.up * headSize * .5f;
            DRAW.Sphere(ToLeanPos(head), headSize * .5f, ToLeanRot(headRot), 4 * 4).SetColor(Color.white.A(.1f));
        
            //  Hip  //
            Quaternion walkRot = rot * Quaternion.AngleAxis((25 + Mathf.PingPong(t, 1) * -50) * WalkMulti, Vector3.up);
            
            DRAW.Vector(ToLeanPos(tP - spine * .5f + walkRot * Vector3.left * .5f * hipWidth), ToLeanDir(walkRot * Vector3.right * hipWidth)).SetColor(Color.cyan);
            //  Shoulder  //
            Quaternion walkRot2 = shoulderRot * Quaternion.AngleAxis((25 + Mathf.PingPong(t + 1, 1) * -50) * WalkMulti, Vector3.up);
            DRAW.Vector(ToLeanPos(tP + spine * .5f + walkRot2 * Vector3.left * .5f * shoulderWidth), ToLeanDir(walkRot2 * Vector3.right * shoulderWidth)).SetColor(Color.green);
        
            //  Legs  //
            Vector3 l1 = tP - spine * .5f + walkRot * Vector3.left  * .5f * hipWidth;
            Vector3 r1 = tP - spine * .5f + walkRot * Vector3.right * .5f * hipWidth;
            
            Vector3 l2 = tP - spine * .5f + rot * Vector3.left  * .5f * hipWidth;
            Vector3 r2 = tP - spine * .5f + rot * Vector3.right * .5f * hipWidth;
            DrawIKLeg( .5f + t, l1, l2,  leftLegRot, true);
            DrawIKLeg(1.5f + t, r1, r2, rightLegRot, false);
        
            //DrawFootArc(tP - spine * .5f + rot * Vector3.left  * .5f * hipWidth + Vector3.down * legLength, rot).SetColor(Color.white.A(.1f));
            //DrawFootArc(tP - spine * .5f + rot * Vector3.right * .5f * hipWidth + Vector3.down * legLength, rot).SetColor(Color.white.A(.1f));
        
            //  Arms  //
            DRAW.Vector(ToLeanPos(tP + spine * .5f + walkRot2 * Vector3.left  * .5f * shoulderWidth), ToLeanDir(walkRot2 * Quaternion.AngleAxis(-30, Vector3.forward) * Vector3.down * armLength)).SetColor(Color.cyan);
            DRAW.Vector(ToLeanPos(tP + spine * .5f + walkRot2 * Vector3.right * .5f * shoulderWidth), ToLeanDir(walkRot2 * Quaternion.AngleAxis( 30, Vector3.forward) * Vector3.down * armLength)).SetColor(Color.cyan);
        
    }


    private Vector3 ToLeanPos(Vector3 p)
    {
        return pos + bodyLeanRot * (p - pos);
    }
    
    
    private Vector3 ToLeanDir(Vector3 d)
    {
        return bodyLeanRot * d;
    }
    
    
    private Quaternion ToLeanRot(Quaternion r)
    {
        return bodyLeanRot * r;
    }


    private DRAW.Shape DrawFootArc(Vector3 pos, Quaternion rot)
    {
        const int div = 3, steps = div * 4 + 1;
        DRAW.Shape shape = DRAW.Shape.Get(steps);
        
        for (int i = 0; i < steps; i++)
        {
            Quaternion arc = Quaternion.AngleAxis(i * (360f / (steps - 1)), Vector3.right);
            Vector3 dir = arc * Vector3.back;
            dir.y *= Mathf.Pow(dir.z * .5f + .5f, 2) * .5f;
            dir *= RunRange * .5f;
            shape.Set(i, pos + rot  * dir);
        }
        
        return shape;
    }


    private Vector3 ArcPos(float lerp, Vector3 pos, Quaternion rot)
    {
        float lerp01 = Mathf.PingPong(lerp, 1);
        float z = lerp01 * 2 - 1;
        float y = Mathf.Sqrt(1 - Mathf.Pow(z, 2));
        y *= Mathf.FloorToInt(lerp) % 2 == 0? 1 : -1;
        y *= Mathf.Pow(z * .5f + .5f, 2) * .25f * (1 + crouch);
        
        return pos + rot * new Vector3(0, y, z) * RunRange * .5f * WalkMulti;
    }


    private void DrawIKLeg(float lerp, Vector3 root, Vector3 stiffRoot, Quaternion rot, bool left)
    {
        Color c = left? COLOR.red.tomato : COLOR.purple.orchid;
        
        root = ToLeanPos(root);
        Vector3 fRoot = stiffRoot + Vector3.down * CrouchLeg;
        Vector3 footDir = (ArcPos(lerp,  fRoot, rot)  - root).Clamp(legLength);
        
        float dirMag = footDir.magnitude;
        if (dirMag >= legLength - .001f)
            DRAW.Vector(root, footDir).SetColor(c);
        else
        {
            Vector3 kneeForward = Vector3.Cross(footDir / dirMag, rot * Vector3.right).normalized;
            Vector3 knee = root + footDir * .5f + kneeForward * Mathf.Sqrt(Mathf.Pow(legLength * .5f, 2) - Mathf.Pow(dirMag * .5f, 2));

            DRAW.Vector(root, knee - root).SetColor(c);
            DRAW.Vector(knee, root + footDir - knee).SetColor(c);
        }
    }
    
    
    private static readonly Quaternion QI = Quaternion.identity;
    private static readonly Quaternion QD = Quaternion.LookRotation(Vector3.back);
}
