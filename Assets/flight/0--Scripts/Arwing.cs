using UnityEngine;


public class Arwing : PoseRecFrames, iHitable, iPathInfo
{
    [Space]
    public bool steer;
    
    [Space]
    public float lookAhead;
    
    [Space]
    public float moveAmount;
    private readonly Vector3Force moveForce = new Vector3Force(200).SetSpeed(300).SetDamp(30);
 
    public float rotAmount;
    private readonly QuaternionForce rotForce = new QuaternionForce(200).SetSpeed(160).SetDamp(20);
    
    public float steerAmount;
    private readonly  QuaternionForce steerForce = new QuaternionForce(200).SetSpeed(420).SetDamp(41);
    
    public float dist;
    public Vector3 localPos;
    
    private Placement bP, cP;
    
    private Quaternion leanRot;
    private float rF, tRand;
    private float hitTime;
    
    public float H
    {
        get
        {
            float h = Input.GetAxis("Horizontal") + (Input.GetKey(KeyCode.A)? -1 : Input.GetKey(KeyCode.D)? 1 : 0);
            return steer? h : 0;//h > .01f ? 1 : h < -.01f ? -1 : 0;
        }
    }

    public float V
    {
        get
        {
            float v = Input.GetAxis("Vertical") + (Input.GetKey(KeyCode.S)? -1 : Input.GetKey(KeyCode.W)? 1 : 0);;
            return steer? v * -1 : 0;//v > .01f ? 1 : v < -.01f ? -1 : 0;
        }
    }
    
    
    private float T { get { return tRand + Time.realtimeSinceStartup; }}


    private void Start()
    {
        rF = rotForce.GetSpeed;
        
        tRand = Random.Range(0, 100f);

        if (!steer)
        {
            localPos = new Vector3(
                Random.Range(FlightCore.Range.x * -.35f, FlightCore.Range.x * .35f), 
                Random.Range(FlightCore.Range.y * -.35f, FlightCore.Range.y * .35f));
        } 
    }

    
    private void Update()
    {
        if(FlightCore.Stop)
            return;
        
        float dt = Time.deltaTime;
        
        bP = Placement.Lerp(bP, FlightCore.Sample(dist), dt * 5 * 4);
        cP = Placement.Lerp(cP, FlightCore.Sample(dist + lookAhead), dt * 5 * 4);
        
        Vector3 pos = trans.position;
        Vector3 left = bP.rot * Vector3.left;

        float lookStep = 120 * (1f + Mathf.Sin(T) * .35f);
        
        
        Vector3 oldP = localPos;
        Vector3 inV  = new Vector3(H, V, 0);
        Vector3 mV   = moveForce.Update(inV * moveAmount, dt) * dt;
        Vector3 newP = oldP + mV;
        
        Vector3 clampedPos = new Vector3(Mathf.Max(FlightCore.Range.x * -.5f, 
                                         Mathf.Min(FlightCore.Range.x * .5f, newP.x)), 
                                         Mathf.Max(FlightCore.Range.y * -.5f, 
                                         Mathf.Min(FlightCore.Range.y * .5f, newP.y)));

        if ((clampedPos - newP).sqrMagnitude > .0001f)
        {
            newP = clampedPos;
            moveForce.SetValue((newP - oldP) / dt);
        }
        
        localPos = newP;
        
        float a = 0;
        for (int i = 0; i < 10; i++)
        {
            Placement ahead = FlightCore.Sample(dist + lookStep + i * lookStep);
            Vector3 dir = (ahead.pos + ahead.rot * localPos - pos).normalized;
            a += Vector3.Dot(dir, left);
        }
        
        Quaternion steerRot = steerForce.Update(Quaternion.Euler(V * -1f * steerAmount, H * 1f * steerAmount, 0), dt);
                   leanRot  = rotForce.SetSpeed(rF * (.5f + .5f * Mathf.Clamp01(inV.magnitude))).
            Update(Quaternion.AngleAxis(H * rotAmount + Mathf.Sin(T * 4) * 3.5f + a * 15, Vector3.forward), dt);
        
        
        trans.position = bP.pos + bP.rot * (localPos + Vector3.up * Mathf.Sin(T * 3) * .35f);
        trans.rotation = cP.rot * (steerRot * (leanRot * Quaternion.AngleAxis((Mathf.Sin(Time.realtimeSinceStartup * 20 + gameObject.GetInstanceID())) * 80 * (steer? 0 : 0), Vector3.forward)));
    }
    
    
    public Placement Get { get { return bP; }}
    
    
    public void Hit()
    {
        hitTime = FlightCore.CurrentTime;
    }
    
    
    protected override Vector2 Anim(float time)
    {
        return new Vector2(1 - Mathf.PingPong(Mathf.Clamp01((time - hitTime) * 2) * 5, 1), 0);
    }
    
    
    public Vector3 PathPos { get { return localPos.SetZ(dist); }}
}
