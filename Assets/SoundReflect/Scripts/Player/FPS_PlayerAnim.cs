using UnityEngine;
using ECM.Components;


public class FPS_PlayerAnim : MonoBehaviour
{
    public float leanAmount;
    public float neckHeight;
    [Header("Tweaker")] 
    public ForceTweaker tweaker;
    
    private readonly RotAxisStack eyeAxis    = new RotAxisStack(6), 
                                  weightAxis = new RotAxisStack(6);

    private Transform trans;
    private CharacterMovement movement;

    private FPS_Player player;
    
    private readonly QuaternionForce lookForce  = new QuaternionForce(300, 120f, 28.75f);
    private readonly QuaternionForce steerForce = new QuaternionForce(300,  75f, 19.5f);
    private readonly Vector3Force    NeckForce  = new    Vector3Force(300,  12f, 8.8f);

    private float walkTime;
    

    private void Start()
    {
        player = GetComponent<FPS_Player>();

        movement = player.GetMovement;
        trans = transform;
        
        GetTweakValues();
    }


    public Quaternion EyeSway(float vel)
    {
        Vector3 v = eyeAxis.Update(Time.deltaTime * (40 + vel * 20), 1);
        
        v.z = 110;
        v.y *= .15f;
        v.x *= (1 + vel * .5f) * .3f;
        v.y *= .75f;
        v.y *= .25f;
        
        return Quaternion.LookRotation(v.normalized, Vector3.up);
    }
    
    
    public Vector3 WeightSway(float vel)
    {
        Vector3 v = weightAxis.Update(Time.deltaTime * (50 + vel * 15), 1);
        
        v *= (1 + vel * 1.5f) * .00095f;
        v.y *= .2f;
        v *= .5f;
        
        return v;
    }
    
    
    public Placement NeckUpdate(float crouch, float dt)
    {
        float xzVel = movement.velocity.SetY(0).magnitude;
        
        walkTime += Mathf.Max(xzVel, player.turnMeasure * .025f * .77777f) * dt * 2;
        
        float mag = movement.velocity.magnitude / dt * .025f;
        bool moving = mag > .0001f;
        
        float wS       = 6.5f * .8f * (1 + crouch * .3f) * 5f * 4* (FPS_Input.Sprint? 1.75f : 1);
        float walkNod  = 1f + Mathf.Pow(Mathf.Sin(walkTime * wS * 2) * .5f + .5f, 5) * mag * .155f * 3.25f * (movement.isGrounded? 1 : 0) * (1 + crouch * 1.15f);
        float walkSway = (Mathf.Sin(walkTime * wS * .5f) * .5f + .5f) * mag * .035f * (movement.isGrounded? 1 : 0) * (1 + crouch * 3.25f);
        
        walkNod  *= .1f   * 2.45f * (FPS_Input.Sprint? 1.75f : 1);
        walkSway *= .075f * 2.45f * (FPS_Input.Sprint? 1.75f : 1);
        
        float turnAxis = FPS_Input.H2, walkAxis = FPS_Input.V1, strafeAxis = FPS_Input.H1;
        
        Quaternion lookLean = lookForce.Update(Quaternion.AngleAxis(walkSway, Vector3.up) * Quaternion.AngleAxis(turnAxis * -(4 + mag * .5f * player.lookSpeedMulti) * .5f * .5f * walkNod , Vector3.forward), Time.deltaTime);
        
        float backWardsMulti = .8f + (walkAxis * .5f + .5f) * .2f;
        
        Vector3 moveStrafe = new Vector3(strafeAxis * .85f, 0, walkAxis * backWardsMulti);
        moveStrafe = moveStrafe.normalized * Mathf.Min(moveStrafe.magnitude, backWardsMulti);
        
        Vector3 move = Quaternion.AngleAxis(player.angle, Vector3.up) * moveStrafe;
                move += movement.velocity.SetY(0) * .2f;
                move *= player.lookSpeedMulti;
                
        Quaternion steerLean;
        Vector3 neckPosition;
        if(!moving)
        {
            neckPosition = NeckForce.Update(Vector3.zero, dt);
            steerLean = steerForce.Update(Quaternion.identity, dt);
            return new Placement(neckPosition, lookLean * steerLean);
        }
        
        move = trans.InverseTransformDirection(move);
        
        Vector3 right = Vector3.Cross(move.normalized, Vector3.up);
        Vector3 frwd  = Vector3.Cross(right, Vector3.up);
        Quaternion leanRot    = Quaternion.AngleAxis(Vector3.Dot(move, frwd) * leanAmount * walkNod * .05f, right);
        Quaternion leanRotPos = Quaternion.AngleAxis(Vector3.Dot(move, frwd) * leanAmount * walkNod, right);
        
        Vector3 goal = Vector3.down * neckHeight + Quaternion.Slerp(Quaternion.identity, leanRotPos, .5f) * Vector3.up * neckHeight;
    
        neckPosition = NeckForce.Update(goal, dt);
        
        steerLean = steerForce.Update(Quaternion.Slerp(Quaternion.identity, leanRot, .05f), dt);
        
        return new Placement(neckPosition, lookLean * steerLean);
    }


    private void LateUpdate()
    {
        if(Application.isEditor)
            GetTweakValues();
    }


    private void GetTweakValues()
    {
        tweaker.Tweak("lookForce",  lookForce);
        tweaker.Tweak("steerForce", steerForce);
        tweaker.Tweak("NeckForce",  NeckForce);
    }
}
