using UnityEngine;


public class FPS_Player : CharacterController
{
    [Header("Movement")]
    public float speed;
    public float sprintMulti, acceleration, turnSpeed, turnAccel;
    
    [Header("Vertical Head")]
    public float lookAmount;
    public float lookAccel;
    public float deadZone;
    public bool allowVertical;
    [Space]
    public Transform head;
    public bool inverseY;
    
    [Header("Jump")]
    public float jumpForce;
    public float jumpHold, gravityAccel;

    [Header("Tweaker")] 
    public ForceTweaker tweaker;
    
    [Space]
    public float lookSpeedMulti = 1, turnMeasure;
    
    
    private bool jumpEnabled, crouching, grounded;
    
    private float 
        turn, smoothAngle, lookTurn, look, smoothLook, gravity, 
        jumpH, smoothLookAhead, lookCheck, stable = 1;

    private Vector3 pos, smoothMove, jumpForward;

    private Transform trans;
    private LeanAwayForce leanForce;
    private FPS_PlayerAnim anim;
    
    private readonly QuaternionForce hitForce        = new QuaternionForce(300,  17.5f, 4.75f);
    private readonly FloatForce      VForce          = new      FloatForce(300, 184.4f,   19f);
    private readonly FloatForce      CrouchForce     = new      FloatForce(300,    45f,  8.5f);
    private readonly FloatForce      CrouchLookForce = new      FloatForce(300,    17f,  6.5f);
    private readonly QuaternionForce headRotForce    = new QuaternionForce(300,  244f,   21f);
    private readonly Vector3Force    headPosForce    = new    Vector3Force(300,  388f, 17.8f);
    private readonly FloatForce      headPosYForce   = new      FloatForce(300, 2528f, 63.8f);
    
    public CrouchInfo crouchInfo;

    
    private void Start()
    {
        trans = transform;
        
        pos = trans.position;
        if (Physics.SphereCast(new Ray(pos + Vector3.up, Vector3.down), .025f, out RaycastHit hit, 1000,
            CastleLayers.Mask_Floor))
        {
            pos = hit.point;
            movement.transform.position = pos;
        }
        
        smoothAngle = angle = Vector2.up.Angle_Sign(trans.forward.V2UseZ());
        
        headRotForce.SetValue(head.rotation);
        headPosForce.SetValue(head.position);
        headPosYForce.SetValue(head.position.y);

        leanForce = GetComponent<LeanAwayForce>();
        anim      = GetComponent<FPS_PlayerAnim>();
        
        GetTweakValues();
    }


    private void LateUpdate()
    {
        if(Application.isEditor)
            GetTweakValues();
        
        
        float dt = Time.deltaTime;
        
        Quaternion rotBefore = head.rotation;

        CrouchUpdate(dt);
        
        movement.SetCapsuleHeight(head.position.y - trans.position.y + .15f);
        
        stable = Mathf.Max(1, stable - dt * 1.35f);
        
        Vector3 goal = movement.transform.position;
        Vector3 hPos = Vector3.Lerp(pos, goal, dt * 12);
        Vector3 vPos = Vector3.Lerp(pos, goal, dt * 24);
        
        pos         = new Vector3(hPos.x, vPos.y, hPos.z);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, dt * 12);
        smoothLook  = Mathf.Lerp(smoothLook,   look, dt * 7.8f);
        
        trans.position = pos;
        trans.rotation = Quaternion.AngleAxis(smoothAngle, Vector3.up);
        
        Placement neckPlacement = anim.NeckUpdate(crouchInfo.Crouch, dt);
        head.localPosition = neckPlacement.pos + Vector3.down * crouchInfo.Crouch;
       
        Quaternion headRot = Quaternion.AngleAxis(VForce.Update(smoothLook, dt) + crouchInfo.CrouchOffset * .25f + (FPS_Input.Sprint? 1.25f : 0), Vector3.right);
        head.localRotation = headRot;
        head.localRotation = hitForce.Update(Quaternion.identity, dt) * leanForce.LeanAwayRot(dt) * neckPlacement.rot * headRot;
        
        
        float xzVel = movement.velocity.SetY(0).magnitude;
        Quaternion finalRot = headRotForce.Update(head.rotation * anim.EyeSway(xzVel * .2f), dt);
        
        head.rotation = finalRot;

        Vector3 hp = head.position + anim.WeightSway(xzVel);
        head.position = headPosForce.Update(hp, dt).SetY(headPosYForce.Update(hp.y, dt));
        
        turnMeasure = Mathf.Lerp(turnMeasure, Quaternion.Angle(rotBefore, finalRot) / dt, dt * 8);

        if (Input.GetKeyDown(KeyCode.P))
        {
            Vector3 roomPos = CastleManager.RandomRoomCenter + Vector3.up * .5f;
            movement.transform.position = roomPos;
            movement.cachedRigidbody.position = roomPos;
            Vector3 warpDir = roomPos - pos;
            pos += warpDir;
            trans.position = pos;
            
            headPosForce.SetValue(headPosForce.Value + warpDir);
            headPosForce.SetForce(Vector3.zero);
            head.position += warpDir;
        }
    }


    private void CrouchUpdate(float dt)
    {
        bool headClearance = movement.ClearanceCheck(1.6f);
        bool crouchOrder = FPS_Input.Crouch;
        
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
        
        float CrouchGoal   = crouching? 1.175f : 0;
        float Crouch       = CrouchForce.Update(CrouchGoal, dt);
        float CrouchOffset = CrouchLookForce.Update((CrouchGoal - Crouch) * -13 + (headClearance? 0 : 9) - Crouch * 6, dt);
        
        crouchInfo = new CrouchInfo(Crouch, CrouchOffset);
    }
    

    public void FixedUpdate()
    {
        float gravityNow = gravity;
        
        float dt = Time.fixedDeltaTime;

        float turnAxis = FPS_Input.H2,
            walkAxis = FPS_Input.V1,
            strafeAxis = FPS_Input.H1,
            lookAxis = FPS_Input.V2 * (allowVertical ? 1 : 0) * (inverseY ? -1 : 1);
            lookAxis = Mathf.Max(0, Mathf.Abs(lookAxis) - deadZone) * ((1f - deadZone) / 1f) * Mathf.Sign(lookAxis);
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
        
        smoothMove = Vector3.Lerp(smoothMove, move * speed * lookSpeedMulti * (1f - crouchInfo.Crouch * .55f) * (FPS_Input.Sprint? sprintMulti : 1), dt * acceleration);
        
        Vector3 jump = Vector3.up * GetVerticalSpeed(dt) + (movement.isGrounded? Vector3.zero : jumpForward);
        
        movement.Move(smoothMove + jump, speed * 10 * (FPS_Input.Sprint? sprintMulti : 1), false);
      
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

        if (movement.isGrounded && !grounded)
        {
            float amount = Mathf.Min(0, gravityNow) * -.00025f;
            amount = Mathf.Min(amount, .45f);
            CrouchForce.AddForce(amount);
        }
           
                
        grounded = movement.isGrounded;
    }


    private float GetVerticalSpeed(float time)
    {
        bool jumpInput = !crouching && FPS_Input.Jump;
        
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
        Quaternion hitRot = Quaternion.AngleAxis(relVel.magnitude * .056f, frwd) * Quaternion.AngleAxis(relVel.y * .14f, Vector3.right);
        hitForce.AddForce(hitRot);
        
        stable = Mathf.Max(stable, relVel.magnitude * .7f);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        movement.capsuleCollider.radius = .4f;
    }


    public struct CrouchInfo
    {
        public readonly float Crouch, CrouchOffset;

        public CrouchInfo(float crouch, float crouchOffset)
        {
            Crouch       = crouch;
            CrouchOffset = crouchOffset;
        }
    }


    private void GetTweakValues()
    {
        tweaker.Tweak("hitForce",        hitForce);
        tweaker.Tweak("VForce",          VForce);
        tweaker.Tweak("CrouchForce",     CrouchForce);
        tweaker.Tweak("CrouchLookForce", CrouchLookForce);
        tweaker.Tweak("headRotForce",    headRotForce);
        tweaker.Tweak("headPosForce",    headPosForce);
        tweaker.Tweak("headPosYForce",   headPosYForce);
    }
}
