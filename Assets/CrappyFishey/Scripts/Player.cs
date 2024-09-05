using System.Text;
using UnityEngine;


public class Player : CharacterController
{
    [Header("Movement")]
    public float speed;
    public float acceleration, turnSpeed, turnAccel;
    
    [Header("Vertical Head")]
    public float lookAmount;
    public float lookAccel;
    [Space]
    public Transform head;
    public bool inverseY;
    
    [Header("Lean")]
    public float leanAmount;
    public float neckHeight;
    
    [Header("Jump")]
    public float jumpForce;
    public float jumpHold, gravityAccel;
    
    [Header("Raycast")]
    public Mesh rayMesh;
    public float rayDist, rayForce;
    public LayerMask walls, ground;
    
    
    private int dirCount;
    private Vector3[] dirs;
    private float[] dirMulti;
    
    private bool jumpEnabled, crouching, grounded;
    private float turn, smoothAngle, lookTurn, look, smoothLook, gravity, jumpH, smoothLookAhead, 
                  lookSpeedMulti = 1, walkTime, stable = 1, smoothStable = 1, crouchOffset;
        
    private Vector3 pos, smoothMove, jumpForward, neckPosition;
    
    public float crouch;
    private float exhaustion;

    
    private Transform trans;
    private Quaternion smoothLeanAway = Quaternion.identity, steerLean = Quaternion.identity, lookLean = Quaternion.identity;
    
    private readonly QuaternionForce wallForce   = new QuaternionForce(200).SetSpeed( 48).SetDamp( 6.2f);
    private readonly QuaternionForce hitForce    = new QuaternionForce(200).SetSpeed(70).SetDamp(4.75f);
    private readonly QuaternionForce steerForce  = new QuaternionForce(200).SetSpeed(100).SetDamp(9.75f);
    private readonly QuaternionForce lookForce   = new QuaternionForce(200).SetSpeed(80).SetDamp(11.75f);
    private readonly Vector3Force    NeckForce   = new    Vector3Force(200).SetSpeed(48).SetDamp(8.8f);
    private readonly FloatForce      VForce      = new      FloatForce(200).SetSpeed(111).SetDamp(11f);
    private readonly FloatForce      LightForce  = new      FloatForce(200).SetSpeed(2321).SetDamp(48f);
    private readonly FloatForce      CrouchForce = new      FloatForce(200).SetSpeed(45).SetDamp(8.5f);
    private readonly FloatForce  CrouchLookForce = new      FloatForce(200).SetSpeed(34).SetDamp(6.5f);
    
    private readonly QuaternionForce headRotForce = new QuaternionForce(200).SetSpeed(240 * .8f).SetDamp(13.75f * .8f);
    private readonly Vector3Force    headPosForce = new    Vector3Force(200).SetSpeed(258).SetDamp(17.8f);
    
    private float flashLight;
    private bool lightOn;
    private float turnMeasure;
    
    private float breath;
    
    private static readonly int playerRoot  = Shader.PropertyToID("PlayerRoot"), 
                                flashLight1 = Shader.PropertyToID("FlashLight");
    
    private float CrouchGoal { get { return crouching? .875f : 0; }}
    
    private readonly RotAxisStack eyeAxis    = new RotAxisStack(6), 
                                  weightAxis = new RotAxisStack(6);

    
    private void Start()
    {
        trans = transform;
        
        pos = trans.position;
        
        dirs = rayMesh.vertices;
        dirCount = dirs.Length;
        dirMulti = new float[dirCount];
        for (int i = 0; i < dirCount; i++)
            dirMulti[i] = 1f - Mathf.Pow(Mathf.Abs(Vector3.Dot(dirs[i], Vector3.up)), 4);
        
        Vector3 f = trans.forward;
        angle = Vector2.up.Angle_Sign(new Vector2(f.x, f.z));
        smoothAngle = angle;
        
        headRotForce.SetValue(head.rotation);
        headPosForce.SetValue(head.position);
    }


    private void LateUpdate()
    {
        Quaternion rotBefore = head.rotation;
        
        bool headClearance = movement.ClearanceCheck(1.6f);
        bool crouchOrder = CrappyInput.Crouch;
        if (crouching)
        {
            if((!crouchOrder || !movement.isGrounded) && headClearance)
                crouching = false;
        }
        else
        {
            if ((crouchOrder || !headClearance) && movement.isGrounded )
                crouching = true;
        }
        
        float dt = Time.deltaTime;
        
        crouch = CrouchForce.Update(CrouchGoal, dt);
        
        movement.SetCapsuleHeight(head.position.y - trans.position.y + .05f);
        
        stable = Mathf.Max(1, stable - dt * 1.35f);
        smoothStable = 1; //Mathf.Lerp(smoothStable, Mathf.Max(stable, 1 + turnMeasure * .0015f), dt * .65f);
        
        
        float vel = movement.velocity.SetY(0).magnitude;
        
        float edt = dt * 1.95f;
        exhaustion = 0;//Mathf.Clamp(exhaustion - edt * .111675f + 
                            //       Mathf.Max(Mathf.Abs(CrouchGoal - crouch) * .005f, Mathf.Max(vel * .0275f, turnMeasure *  .0006f) * edt * 1.1f), 0, 4);
            
        walkTime += Mathf.Max(vel, turnMeasure * .025f) * dt;
        
        Vector3 goal = movement.transform.position;
        Vector3 hPos = Vector3.Lerp(pos, goal, dt * 12);
        Vector3 vPos = Vector3.Lerp(pos, goal, dt * 12 * 2);
        
        pos         = new Vector3(hPos.x, vPos.y, hPos.z);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, dt * 12);
        smoothLook  = Mathf.Lerp(smoothLook,   look, dt * 12f * .65f);
        
        trans.position = pos;
        trans.rotation = Quaternion.AngleAxis(smoothAngle, Vector3.up);
        
        NeckUpdate();
        head.localPosition = neckPosition + Vector3.down * crouch;
        
        crouchOffset = CrouchLookForce.Update((CrouchGoal - crouch) * -13 + (headClearance? 0 : 9) - crouch * 6, dt);
        Quaternion headRot = Quaternion.AngleAxis(VForce.Update(smoothLook, dt) + crouchOffset, Vector3.right);
        head.localRotation = headRot;
        head.localRotation = LeanAwayRot() * (lookLean * steerLean) * headRot;
        
        
        Quaternion finalRot = headRotForce.Update( head.rotation * EyeSway(vel), dt);
        
        head.rotation = finalRot;
        
        head.position = headPosForce.Update(head.position + WeightSway(vel) + BreathOffset(), dt);
        
        turnMeasure = Mathf.Lerp(turnMeasure, Quaternion.Angle(rotBefore, finalRot) / dt, dt * 8);
        
        Shader.SetGlobalVector(playerRoot, pos + Vector3.up * .2f);


        if (CrappyInput.Light)
            lightOn = !lightOn;
        
        flashLight = LightForce.Update(lightOn? 1 : 0, dt);
        Shader.SetGlobalFloat(flashLight1, flashLight);
    }


    private Quaternion LeanAwayRot()
    {
        Quaternion currentCamRot = head.rotation;
        
        Vector3 camPos    = head.position;
            
        Vector3 bounceMV = Vector3.zero;
        
        for (int i = 0; i < dirCount; i++)
        {
            Vector3 d = currentCamRot * dirs[i];
            
            float multiDist = rayDist;
            float castDist  = Mathf.Max(multiDist, rayDist);
            
            if (Physics.Raycast(camPos, d, out RaycastHit hit, castDist, walls))
            {
                float dist = hit.distance;
                float push = (1f - Mathf.Min(dist, multiDist) / multiDist)
                             + Mathf.Pow(1f - Mathf.Min(dist, rayDist) / rayDist, 2) * .005f;
                              
                bounceMV -= d * push * rayForce * dirMulti[i];
            }
        }
        
        bounceMV = trans.InverseTransformDirection(bounceMV);
        bounceMV.x = Mathf.Pow(Mathf.Abs(bounceMV.x), 3) * Mathf.Sign(bounceMV.x);
        bounceMV.z = Mathf.Pow(Mathf.Abs(bounceMV.z), 3) * Mathf.Sign(bounceMV.z);
        
        Vector3 vel = movement.velocity;
        vel.y *= .66f;
        float velMulti = 1 + Mathf.Pow(vel.magnitude * .2125f, 7);
       
        smoothLeanAway = Quaternion.Slerp(smoothLeanAway, Quaternion.Euler(new Vector3(-bounceMV.z + LookAheadVector(), 0, -bounceMV.x) * velMulti), Time.deltaTime * 3.65f * velMulti);
        
        return hitForce.Update(Quaternion.identity, Time.deltaTime) * wallForce.Update(smoothLeanAway, Time.deltaTime);
    }
    
    
    private float LookAheadVector()
    {
        return 0;
        Vector3 f = trans.forward;
        
        Vector3 p = head.position;
        float charheight = 0;
        if(Physics.Raycast(new Ray(trans.position + Vector3.up * .5f, Vector3.down), out RaycastHit hit, 10000, ground))
            charheight = Mathf.Max(0, (hit.distance - .5f) * .25f);
        
        Vector3 u = Vector3.up * (1.07f + charheight);
        float weight = 1;
        const float stp = .15f;
        const int steps = 20;
        Vector3 mix = f;
        
        int maxSteps = Physics.Raycast(new Ray(p, f * steps * stp), out hit, steps * stp, walls)? 
            Mathf.Max(0, Mathf.FloorToInt(hit.distance / stp) - 1) : 
            steps;

        for (int i = 0; i < maxSteps; i++)
            if (Physics.Raycast(new Ray(p + f * (stp * (i + 1)), Vector3.down * 10000), out hit, 10000, ground))
            {
                float w = (steps - i) / (steps - 1f);
                mix += (hit.point + u - p).normalized * w;
                weight += w;
            }
        
        mix /= weight;
        
        smoothLookAhead = Mathf.Lerp(smoothLookAhead, -Mathf.Sign(mix.y) * Vector3.Angle(mix, f) * .5f + gravity * -.0185f, Time.deltaTime * 6.5f);
        return smoothLookAhead * .6f; //* 1.25f * 1.2f
    }


    private void NeckUpdate()
    {
        float mag = movement.velocity.magnitude;
        bool moving = mag > .0001f;
        
        float wS       = 6.5f * .8f * (1 + crouch * .3f);
        float walkNod  = 1f + Mathf.Pow(Mathf.Sin(walkTime * wS) * .5f + .5f, 5) * mag * .055f * 3.25f * (movement.isGrounded? 1 : 0) * smoothStable * (1 + crouch * 1.15f);
        float walkSway = (Mathf.Sin(walkTime * wS * .5f) * .5f + .5f) * mag * .235f * (movement.isGrounded? 1 : 0) * smoothStable * (1 + crouch * 3.25f);
        
        walkNod  *= .75f;
        walkSway *= .75f;
        
        float turnAxis = CrappyInput.H2, walkAxis = CrappyInput.V1, strafeAxis = CrappyInput.H1;
        
        lookLean = lookForce.Update(Quaternion.AngleAxis(walkSway, Vector3.up) * Quaternion.AngleAxis(turnAxis * -(4 + mag * .5f * lookSpeedMulti) * .5f * walkNod , Vector3.forward), Time.deltaTime);
        
        float backWardsMulti = .8f + (walkAxis * .5f + .5f) * .2f;
        
        Vector3 moveStrafe = new Vector3(strafeAxis * .85f, 0, walkAxis * backWardsMulti);
        moveStrafe = moveStrafe.normalized * Mathf.Min(moveStrafe.magnitude, backWardsMulti);
        
        Vector3 move = Quaternion.AngleAxis(angle, Vector3.up) * moveStrafe;
                move += movement.velocity.SetY(0) * .2f;
                move *= lookSpeedMulti;
                
        if(!moving)
        {
            neckPosition = NeckForce.Update(Vector3.zero, Time.deltaTime);
            steerLean = steerForce.Update(Quaternion.identity, Time.deltaTime);
            return;
        }
        
        move = trans.InverseTransformDirection(move);
        
        Vector3 right = Vector3.Cross(move.normalized, Vector3.up);
        Vector3 frwd  = Vector3.Cross(right, Vector3.up);
        Quaternion leanRot = Quaternion.AngleAxis(Vector3.Dot(move, frwd) * leanAmount * walkNod, right);
        
        Vector3 goal = Vector3.down * neckHeight + Quaternion.Slerp(Quaternion.identity, leanRot, .5f) * Vector3.up * neckHeight;
        //return goal;
        neckPosition = NeckForce.Update(goal, Time.deltaTime);
        
        steerLean = steerForce.Update(Quaternion.Slerp(Quaternion.identity, leanRot, .05f), Time.deltaTime);
    }


    private Quaternion EyeSway(float vel)
    {
        Vector3 v = eyeAxis.Update(Time.deltaTime * (40 + vel * 20) * (1 + exhaustion * .395f), 1);
        
        v *= 1 + exhaustion * .15f;
        v.z = 110;
        v.y *= .15f;
        v.x *= (1 + vel * .5f) * .3f;
        
        v *= .75f;
        
        return Quaternion.LookRotation(v.normalized, Vector3.up);
    }
    
    
    private Vector3 WeightSway(float vel)
    {
        Vector3 v = weightAxis.Update(Time.deltaTime * (50 + vel * 15) * (1 + exhaustion * .395f), 1);
        
        v *= 1 + exhaustion * .15f;
        v *= (1 + vel * 1.5f) * .00095f;
        v.y *= .2f;
        
        return v;
    }

    private Vector3 BreathOffset()
    {
        Vector3 r = trans.rotation * Vector3.right;
        
        const float down = .2f;
        
        float sp = .4f + exhaustion * .15f;
        float ap = (.4f + exhaustion * 1.4f) * (1f - crouch * 1f);
        float bp = (.4f + exhaustion * .55f) * (1f - crouch * 1f);
        
        breath += Time.deltaTime * 7 * sp;
        float s = Mathf.Sin(breath) * .5f + .3f;
              //s = .8f + .2f * Mathf.Pow(s, 2);
              
        Vector3 o = Quaternion.AngleAxis(s * .5f * ap, r) * Vector3.up * down;
        
        return Vector3.down * (down + s * .008f * bp) + o;
    }
    
    
    private float lookCheck;


    public void FixedUpdate()
    {
        float gravityNow = gravity;
        
        float dt = Time.fixedDeltaTime;
        
        float turnAxis   = CrappyInput.H2, 
              walkAxis   = CrappyInput.V1,
              strafeAxis = CrappyInput.H1,
              lookAxis   = CrappyInput.V2;
        
        lookAxis *= inverseY? -1 : 1;
        lookAxis = Mathf.Clamp(lookAxis, -1, .75f);
        
        float a = Mathf.Abs(lookCheck * .925f);
        float b = Mathf.Abs(lookAxis);
        lookCheck = a > b? lookCheck * .925f : lookAxis;
        lookAxis = lookCheck;
        
        turn = Mathf.Lerp(turn, turnAxis * turnSpeed, dt * turnAccel);
        angle += turn * dt;
        
        float backWardsMulti = .8f + (walkAxis * .5f + .5f) * .2f;
        
        Vector3 moveStrafe = new Vector3(strafeAxis * .85f, 0, walkAxis * backWardsMulti);
                moveStrafe = moveStrafe.normalized * Mathf.Min(moveStrafe.magnitude, backWardsMulti);
        
        Vector3 move = Quaternion.AngleAxis(angle, Vector3.up) * moveStrafe;
       
        lookSpeedMulti = Mathf.Lerp(lookSpeedMulti, 
            movement.isGrounded?
            1f + lookAxis * (lookAxis < 0? .75f : 1) * .25f * (1f - Mathf.Pow(1f - Mathf.Abs(walkAxis), 8)):
            1,
            dt * (movement.isGrounded? 5 : 1f));
        
        
        //Debug.Log(lookSpeedMulti);
        smoothMove = Vector3.Lerp(smoothMove, move * speed * lookSpeedMulti * (1f - crouch * .55f), dt * acceleration);
        
        Vector3 jump = Vector3.up * GetVerticalSpeed(dt) + (movement.isGrounded? Vector3.zero : jumpForward);
        
        movement.Move(smoothMove + jump, speed * 10, false);
      
        lookAxis = Mathf.Lerp(lookAxis, Mathf.Pow(lookAxis, 2) * Mathf.Sign(lookAxis), .5f);
        
        float lookDiff = lookAxis * lookAmount - look;
        float lookMove = Mathf.Min(Mathf.Abs(lookDiff), dt * lookAccel) * Mathf.Sign(lookDiff);
        look += lookMove;

        if (movement.isGrounded && movement.groundNormal.y < .5f)
        {
            gravity = 0;
            movement.DisableGrounding();
            jumpEnabled = true;
            jumpForward = Vector3.zero;
        }
        
        if(movement.isGrounded && !grounded)
            CrouchForce.AddForce(Mathf.Min(0, gravityNow) * -.000125f * 2);
                
        grounded = movement.isGrounded;
    }


    private float GetVerticalSpeed(float time)
    {
        bool jumpInput = !crouching && CrappyInput.Jump;
        
        if(!jumpInput)
            jumpEnabled = false;
        
        if(jumpEnabled)
            jumpH += time;
                
        gravity -= jumpEnabled && jumpInput && jumpH < jumpHold? 0 : time * gravityAccel;
        if (movement.isGrounded)
            gravity = 0;
        
        if (!jumpEnabled && movement.isGrounded && jumpInput)
        {
            gravity = jumpForce;
            jumpEnabled = true;
            jumpH = 0;
            movement.DisableGrounding();
            jumpForward = (movement.velocity.SetY(0) * .2f + trans.forward * .45f) * .5f;
        }
        
        return gravity * (movement.isGrounded ? 0 : 1) * time;
    }
    
    
    protected override void HandleCollision(Vector3 normal, Vector3 relVel)
    {
        if(gravity > 0 && normal.y < 0)
            gravity *= (1 - Mathf.Abs(normal.y)) * .75f + .25f;
        
        normal = trans.InverseTransformDirection(normal);
        
        Vector3    frwd   = Vector3.Cross(normal, Vector3.up);
        Quaternion hitRot = Quaternion.AngleAxis(relVel.magnitude * 14f * .004f, frwd) * Quaternion.AngleAxis(relVel.y * 35f * .004f, Vector3.right);
        hitForce.AddForce(hitRot);
        
        stable = Mathf.Max(stable, relVel.magnitude * .7f);
        exhaustion += relVel.magnitude * .02f;
        //LightForce.AddForce(-Mathf.Pow(relVel.magnitude * .013f, 2) * .22f * flashLight * .5f);
    }


    #region Debug
    protected override void OnEnable()
    {
        base.OnEnable();
        DebugUI.BL += OnDebugUI;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        DebugUI.BL -= OnDebugUI;
        Shader.SetGlobalFloat(flashLight1, 1);
    }
    private void OnDebugUI(StringBuilder builder)
    {
        if(movement == null)
            return;
      
        builder.AppendLine(movement.isGrounded? "Grounded" : "In Air").
                AppendLine("SmoothStable: " + smoothStable.ToString("F4")).
                AppendLine("TurnMEasure: " + turnMeasure.ToString("F4")).
                AppendLine("WalkTime: " + walkTime.ToString("F4")).
                AppendLine("Exhaustion: " + exhaustion.ToString("F4"));
    }
    #endregion
}
