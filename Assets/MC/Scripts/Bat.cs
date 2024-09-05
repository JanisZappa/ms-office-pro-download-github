using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class Bat : MonoBehaviour
{
    public float speed, drag;
    [Range(0, 1)]
    public float stiffness;
    
    [Space]
    public Mesh rayMesh;
    public float rayDist;
    public float rayForce;
    
    [Space]
    public LayerMask smoothCollider;
    
    [Space]
    public float searchDist;
    public int foundColliders;
    
    private Vector3 mv;
    
    private Transform cam;
    
    private Vector3[] dirs;
    
    private float angle, smoothAngle;
    private float smoothLook;
    
    private Quaternion camRot = Quaternion.identity;
    private Vector3 pos;
    
    private float time;
    
    private Stepper stepper;
    private BatInput input;
    
    private readonly QuaternionForce leanForce = new QuaternionForce().SetSpeed(99).SetDamp(20.7f);
    private readonly Vector3Force    flapForce = new Vector3Force().SetSpeed(208).SetDamp(17.7f);
    private float shotTime;
    private bool shooting;
    private float autoGoal;
    
    private float rand;
    private float flapTime;
    private float leanAway;
    
    private Vector3 smoothR, smoothR2;
    
    
    private void Start()
    {
        cam = transform.GetChild(0);
        
        dirs     = rayMesh.vertices;
        
        pos = transform.position;
        
        stepper = new Stepper(120 * 2, Step);
        
        input = GetComponent<BatInput>();
        
        camRot= Quaternion.identity;
        
        rand = Random.Range(0, 10000f);
    }

    
    private void Update()
    {
        float dt = Time.deltaTime;
        
        stepper.Update(dt);
        
        Quaternion rot = camRot * leanForce.Value;
        cam.rotation = rot;
        
        float lean = Mathf.Abs(Quaternion.Angle(Quaternion.identity, leanForce.Value)) * .035f;
        
        float fS = Mathf.PerlinNoise(rand + flapTime * 40, .1f) * .6f + .7f;
        flapTime += dt * (21 + mv.magnitude * .09f + Mathf.Abs(leanAway) * .1f) * fS;
        
        Vector3 up = flapForce.Update(Random.insideUnitSphere.AbsY().MultiY(4).MultiZ(.3f), dt).normalized;
        
        
        cam.localPosition = rot * up * ((1 - Mathf.Pow(Mathf.Sin(rand + flapTime) * .5f + .5f, 1.5f)) * 2 - 1) * .05f * (1f + lean * 2.25f + mv.magnitude * .1f) * .55f;
        
        transform.position = pos;
        
         Shooting();
        DebugRays();
    }


    private void Shooting()
    {
        autoGoal = Mathf.Lerp(autoGoal, Random.Range(0, 1f), Time.deltaTime * 20);
        bool shoot = input.auto ? autoGoal > .6f : Input.GetMouseButton(0);
        
        if (shoot)
        {
            if (shooting)
            {
                shotTime += Time.deltaTime;
                
                const float interval = .2f;
                if (shotTime >= interval)
                {
                    shotTime -= interval;
                    FireShot();
                }
            }
            else
            {
                FireShot();
                shotTime = 0;
                shooting = true;
            }
        }
        else
            shooting = false;
    }


    private void FireShot()
    {
        Ray r = input.CursorRay;
        Vector3 shotMV = r.direction * 70 + mv * .35f;
        
        HardWallEnable.BatBlockUpdate(r.origin, 2f);
            
        if(!Physics.SphereCast(new Ray(r.origin, shotMV), .25f, out _, 1.25f, MCLayers.Mask_Hard))
            MCProjectiles.FireShot(r.origin, shotMV);
    }


    private void Step(float dt)
    {
        float oldAngle = angle;
        angle += input.SteerH * dt * 225 * (1 + Mathf.Abs(input.SteerV) * .25f);
        
        smoothAngle = Mathf.Lerp(smoothAngle, angle, dt * 3);
        Quaternion rotRot = Quaternion.AngleAxis(smoothAngle, Vector3.up);
        
        smoothLook = Mathf.Lerp(smoothLook, input.SteerV * -78f, dt * 3);
      
        Quaternion oldRot = camRot;
        Quaternion lookRot = Quaternion.AngleAxis(smoothLook, Vector3.right);
        //camRot = rotRot * lookRot;
        //camRot = qf.Update(rotRot * lookRot, dt);
        camRot = Quaternion.Slerp(camRot, rotRot * lookRot, dt * 18.5f);
        
        Vector3 forward = camRot * Vector3.forward;
        Vector3 right   = camRot * Vector3.right;
        Vector3 up      = camRot * Vector3.up;
        
        
        
        Quaternion rotDiff = Quaternion.FromToRotation(oldRot * Vector3.forward, forward);
        
        mv = Vector3.Slerp(mv, rotDiff * mv, stiffness);
        
        
    //  Axis Dependent Drag
        Vector3 dragMV = forward * Vector3.Dot(mv, forward) * (1f - dt * drag)
                       + right   * Vector3.Dot(mv, right) * (1f - dt * drag * 2.75f)
                       + up      * Vector3.Dot(mv, up) * (1f - dt * drag * 2.75f);
        mv = dragMV;
        //mv *= 1f - dt * drag;
        
        
        Vector3 inputV = input.Vector;
        
    //  RotAxis Input  //
        float leanAmount = Mathf.Abs(Quaternion.Angle(Quaternion.identity, leanForce.Value)) + mv.magnitude * .1f;
        float rSpeed  = 1 + leanAmount;
        float rAmount = .5f + leanAmount * .045f;
        smoothR = Vector3.Lerp(smoothR, Random.insideUnitSphere * 3 * rAmount, dt * .5f * rSpeed);
        smoothR2 = Vector3.Lerp(smoothR2, Random.insideUnitSphere * .5f * rAmount, dt * 15f * rSpeed);
        Vector3 inputR = smoothR + smoothR2;
        inputR.z = 0;
        inputR *= .2f;
        
        float fb = Mathf.Max(0, inputV.z + inputR.z) + Mathf.Min(0, (inputV.z + inputR.z) * .5f);
        
        float extra = Vector3.Dot(forward * (fb >= 0? 1 : -1), Vector3.down) > 0? 1.4f : 1;
              
        mv += forward * (fb * extra * speed * dt);
        mv += Vector3.up * ((inputV.y < 0? inputV.y * 1.25f : inputV.y) + inputR.y) * .7f * speed * dt * 2.35f;
        mv += right * ((inputV.x + inputR.x) * .85f * speed * dt * 2.35f);

        
        float mag     = mv.magnitude * .25f;
        float maxDist = rayDist * 10;
        searchDist = Mathf.Max((1 + 1 * mag) * rayDist, maxDist);
        Bounds bnds = new Bounds(pos, Vector3.one * searchDist * 2);
        
        foundColliders = SmoothWallEnable.BatBlockUpdate(bnds, pos, searchDist);
        
        Vector3 bounceMV = Vector3.zero;
        if (foundColliders > 0)
        {
            Vector3 mVN      = mv.normalized;
            int   rayCount   = dirs.Length;
            
            for (int i = 0; i < rayCount; i++)
            {
                Vector3 d = camRot * dirs[i];
            
                float multi     = 1 + Mathf.Pow(Vector3.Dot(mVN, d) * .5f + .5f, 6) * mag;
                float multiDist = rayDist * multi;
                float castDist  = Mathf.Max(multiDist, maxDist);
            
                if (Physics.Raycast(pos, d, out RaycastHit hit, castDist, smoothCollider))
                {
                    float dist = hit.distance;
                    float push = (1f - Mathf.Min(dist, multiDist) / multiDist) * multi
                                 + Mathf.Pow(1f - Mathf.Min(dist, maxDist) / maxDist, 2) * .005f;
                              
                    bounceMV -= d * push * rayForce * dt;
                }
            }
            
            mv += bounceMV;
        }
        
        Vector3 move = mv * dt;
        float moveMag = move.magnitude;
        Vector3 movedPos = pos;
        while (moveMag > .0001f)
        {
            if (Physics.SphereCast(new Ray(pos, move), .45f, out RaycastHit h, moveMag, MCLayers.Mask_Smooth))
            {
                float amount = Mathf.Max(0, h.distance - .01f);
                movedPos += move.normalized * amount;
                moveMag -= amount;
                move = Vector3.Reflect(move.normalized, h.normal) * moveMag;
            } 
            else
            {
                movedPos += move;
                break;
            }
        }
       
        
        pos = movedPos;
        
        float lean = (angle - oldAngle) / dt * -.039f * (1 + mv.magnitude * .1f);
        
        leanAway = Mathf.Lerp(leanAway, Vector3.Dot(bounceMV, right) * -35 * (1 + mv.magnitude * .085f), dt * 2.45f);
              lean += leanAway;
        
        leanForce.Update(Quaternion.AngleAxis(lean * .65f, Vector3.forward), dt);

    }


    private void DebugRays()
    {
        return;
        float mag     = mv.magnitude * .25f;
        float maxDist = rayDist * 10;
        searchDist = Mathf.Max((1 + 1 * mag) * rayDist, maxDist);
        
        int hits = SmoothWallEnable.BatBlockUpdate(pos, searchDist);

        bool faster = false;
        if (hits > 0)
        {
            Vector3 mVN      = mv.normalized;
            Vector3 p        = transform.position;
            int   rayCount   = dirs.Length;
            
            for (int i = 0; i < rayCount; i++)
            {
                Vector3 d = camRot * dirs[i];
            
                float multi     = 1 + Mathf.Pow(Vector3.Dot(mVN, d) * .5f + .5f, 6) * mag;
                float multiDist = rayDist * multi;
                
                float castDist = Mathf.Max(multiDist, maxDist);
                
                if(multiDist > maxDist)
                    faster = true;
            
                if (Physics.Raycast(p, d, out RaycastHit hit, castDist, smoothCollider))
                {
                    DRAW.Vector(p, hit.point - p).SetColor(Color.magenta);
                }
                else
                {
                    DRAW.Vector(p, d * castDist).SetColor(Color.yellow);
                }
            }
        }
        
        if(faster)
            Debug.Log("Cool");
    }
}


public class RotAxis
{
    private static readonly System.Random r = new System.Random(DateTime.Now.Int());
    
    private readonly Vector3 dir;
    private readonly float speed;
    private float t;
 
    public RotAxis()
    {
        dir = new Vector3(r.Range(-1, 1f), 
                          r.Range(-1, 1f), 
                          r.Range(-1, 1f)).normalized;
             
        speed = r.Range(1, 3) * (r.Range(0, 2) == 0 ? -1 : 1);
        t     = r.Range(0, 10000f);
    }
 
 
    public Vector3 Update(float dt, float multi)
    {
        t += dt * speed * multi;
             
        return Quaternion.AngleAxis(t, dir) * Vector3.forward;
    }
}


public class RotAxisStack
{
    private readonly RotAxis[] axis;
    private readonly int count;
    private readonly float multi;

    public RotAxisStack(int count)
    {
        this.count = count;
        
        axis = new RotAxis[count];
        for (int i = 0; i < count; i++)
            axis[i] = new RotAxis();
        
        multi = 1f / count;
    }
    
    public Vector3 Update(float dt, float multi)
    {
        Vector3 value = Vector3.zero;

        for (int i = 0; i < count; i++)
            value += axis[i].Update(dt, multi);
             
        return value * multi;
    }
}