using UnityEngine;

public class GoodCar : MonoBehaviour
{
    public float speed, accel, stickLessByVelocity;
    public float steerAngle, steerAccel;
    
    [Space]
    public Vector2 wheelLayout;
    public WheelSettings wheel;
    public SpringSettings spring;

    [Space] 
    public GoodCarInput input;

    private Transform trans;
    private Rigidbody rb;

    private readonly Vector3[] wheelPositions = new Vector3[4];
    private readonly Vector3[] wheelDrawPts = new Vector3[30];
    
    
    public float steer, motor;
    
    
    private void Start()
    {
        Physics.gravity = new Vector3(0, -20, 0);
        
        trans = transform;
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = Vector3.up * .25f;
        
        int i = 0;
        for (int z = 0; z < 2; z++)
        for (int x = 0; x < 2; x++)
            wheelPositions[i++] = new Vector3(wheelLayout.x * (-1 + x * 2), spring.length * .5f, wheelLayout.y * (-1 + z * 2));
    }

    
    private void LateUpdate()
    {
        Vector3 pos = trans.position;
        Quaternion rot = trans.rotation;
        Vector3 down = rot * Vector3.down;
        
        float noiseAngle = 0;
        float wheelAngle = (steer + noiseAngle) * steerAngle;
        Quaternion wheelR = rot * Quaternion.AngleAxis(wheelAngle, Vector3.up);

        for (int i = 0; i < 4; i++)
            DrawWheel(pos + rot * wheelPositions[i], i < 2? rot : wheelR, down);

        //DRAW.Box(pos + rot * boxPos, boxSize, rot).SetColor(COLOR.turquois.bright);
    }


    private void DrawWheel(Vector3 pos, Quaternion rot, Vector3 down)
    {
        Vector3 end;
        WheelCheck check = SpringRaycastHit(pos, down);
        if (check.gounded)
            end = pos + down * (spring.length - check.springOffset);
        else
            end = pos + down * spring.length;


        Color c = COLOR.red.tomato;
        DRAW.Vector(pos, end - pos).SetColor(c);

        DrawWheelSide(end, rot, 1);
        DrawWheelSide(end, rot, -1);
    }


    private void DrawWheelSide(Vector3 end, Quaternion rot, float side)
    {
        Color c = COLOR.red.tomato;
        Vector3 u = new Vector3(wheel.thickness * side, wheel.radius, 0);

        for (int i = 0; i < 30; i++)
            wheelDrawPts[i] = end + rot * Quaternion.AngleAxis(12 * i, Vector3.right) * u;

        Vector3 a = wheelDrawPts[29];
        for (int i = 0; i < 30; i++)
        {
            Vector3 b = wheelDrawPts[i];
            DRAW.Vector(a, b - a).SetColor(c);

            a = b;
        }
    }
    

    private void FixedUpdate()
    {
        Vector2 inputSteerAccel = input != null? input.GetSteerAndAccel() : Vector2.zero;
        
        Vector3    pos  = trans.position;
        Quaternion rot  = trans.rotation;
        Vector3    down = rot * Vector3.down;

        Vector2 vel = rb.velocity;
        
        SteerUpdate(inputSteerAccel.x);
        MotorUpdate(inputSteerAccel.y);
       

        float noiseAngle = 0;
        float wheelAngle = (steer + noiseAngle) * steerAngle;
        Quaternion wheelR = rot * Quaternion.AngleAxis(wheelAngle, Vector3.up);
        
        
        for (int i = 0; i < 4; i++)
        {
            Vector3    p = pos + rot * wheelPositions[i];
            WheelCheck check = SpringRaycastHit(p, down);
            if(!check.gounded)
                continue;
            
            Quaternion r = i < 2 ? rot : wheelR;
            Vector3    v = rb.GetPointVelocity(p);
            rb.AddForceAtPosition(ApplyWheelDrag(r * Vector3.right, check.groundNormal, v) + ApplyWheelAccel(r * Vector3.right, check.groundNormal, i >= 2? 1 : 0) + WheelDamp(down, v, check.springOffset), p);
        }
    }
    
    
    private void MotorUpdate(float Acceleration)
    {
        float accStep = Time.fixedDeltaTime * accel;
        float acc = Acceleration * accStep;
        if (Mathf.Approximately(acc, 0))
            acc = Mathf.Min(Mathf.Abs(motor), accStep) * -Mathf.Sign(motor);
        motor = Mathf.Clamp(motor + acc, -.75f, 1);
    }


    private void SteerUpdate(float Steering)
    {
        float accStep = Time.fixedDeltaTime * steerAccel;
        float acc = Steering * accStep;
        if (Mathf.Approximately(acc, 0))
            acc = Mathf.Min(Mathf.Abs(steer), accStep) * -Mathf.Sign(steer);
        steer = Mathf.Clamp(steer + acc, -1, 1);
    }
    
    
    private Vector3 ApplyWheelDrag(Vector3 right, Vector3 normal, Vector3 vel)
    {
        Vector3 f = Vector3.Cross(right, normal).normalized;
        Vector3 r = Vector3.Cross(normal, f).normalized;

        float rDot = Vector3.Dot(vel, r);
        float slipMulti = 1f - Mathf.Clamp01(Mathf.Abs(rDot) * stickLessByVelocity);

        return (f * -(Vector3.Dot(vel, f) * wheel.drag.forward) + r * -(rDot * Mathf.Lerp(wheel.drag.forward, wheel.drag.sideways, slipMulti)));// / Time.fixedDeltaTime;
    }

    
    private Vector3 ApplyWheelAccel(Vector3 right, Vector3 normal, float factor)
    {
        return Vector3.Cross(right, normal).normalized * motor * speed * factor;
    }


    private Vector3 WheelDamp(Vector3 down, Vector3 pointVel, float springOffset)
    {
        float vel = Vector3.Dot(-down, pointVel);

        float force = springOffset * spring.strength - vel * spring.damping;
        
        return -down * force;
    }
    
    private readonly RaycastHit[] hits = new RaycastHit[100];
    
    private WheelCheck SpringRaycastHit(Vector3 pos, Vector3 down)
    {
        int hitCount = Physics.RaycastNonAlloc(new Ray(pos, down), hits, spring.length + wheel.radius);
        float hitDist = float.MaxValue;
        bool foundHit = false;
        Vector3 normal = Vector3.up;
        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.rigidbody != rb)
            {
                if (hitDist > hit.distance)
                {
                    hitDist = hit.distance;
                    normal  = hit.normal;
                }
               
                foundHit = true;
            }
        }

        return new WheelCheck(foundHit, foundHit ? spring.length + wheel.radius - hitDist : 0, normal);
    }
    
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        motor *= 1 - Mathf.Abs(Vector3.Dot(trans.rotation * Vector3.forward, other.contacts[0].normal)) * .5f;
    }


    private struct WheelCheck
    {
        public bool gounded;
        public float springOffset;
        public Vector3 groundNormal;

        public WheelCheck(bool gounded, float springOffset, Vector3 groundNormal)
        {
            this.gounded = gounded;
            this.springOffset = springOffset;
            this.groundNormal = groundNormal;
        }
    }
    
}
