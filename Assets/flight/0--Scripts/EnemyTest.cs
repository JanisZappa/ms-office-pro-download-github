using System.Collections;
using UnityEngine;

public class EnemyTest : PoseRecFrames, iHitable, iPathInfo
{
    public float dist;
    [Space]
    public float lookAhead;
    public Vector3 localPos;
    
    private Placement bP, cP;
    
    private bool moving;
    private float hitTime;
    
    private readonly Vector3Force moveForce = new Vector3Force(200).SetSpeed(40).SetDamp(8);
    private float t, speed;
    private Vector3 animPos;
    
    private void Start()
    {
        StartCoroutine(LateSet());
    }


    private IEnumerator LateSet()
    {
        yield return null;
        localPos = new Vector3(
            Random.Range(FlightCore.Range.x * -.35f, FlightCore.Range.x * .35f),
            Random.Range(FlightCore.Range.y * -.35f, FlightCore.Range.y * .35f)) {z = Random.Range(-1f, 1) * 340};

    }


    private void Update()
    {
        float dt = Time.deltaTime;
        
        animPos = moveForce.Update(localPos, dt);
        
        bP = Placement.Lerp(bP, FlightCore.Sample(dist + animPos.z), dt * 5 * 4);
        cP = Placement.Lerp(cP, FlightCore.Sample(dist + animPos.z + lookAhead), dt * 5 * 4);
        
        animPos.z = 0;
        
        speed = Mathf.Lerp(speed, moving? 5 : 1, Time.deltaTime * 10);
        t += Time.deltaTime * speed;
        
        animPos += Vector3.up * Mathf.Sin(t * 8) * .55f;
        
        trans.position = bP.pos + bP.rot * animPos;
        trans.rotation = cP.rot * Quaternion.AngleAxis(Mathf.Sin(t * 4.15f) * 800, Vector3.up);
        
        animPos.z = moveForce.Value.z + dist;

        if (Input.GetKeyDown(KeyCode.M) && !moving)
        {
            StartCoroutine(MoveIt());
            hitTime = FlightCore.CurrentTime;
        }
    }

    
    public void Hit()
    {
        if(!moving)
            StartCoroutine(MoveIt());
        
        hitTime = FlightCore.CurrentTime;
    }
    
    
    private IEnumerator MoveIt()
    {
        moving = true;
        
        Vector3 start = localPos;
        Vector3 end   = start;
        
        while((start - end).sqrMagnitude < 1000)
            end = new Vector3(
            Random.Range(FlightCore.Range.x * -.45f, FlightCore.Range.x * .45f), 
            Random.Range(FlightCore.Range.y * -.35f, FlightCore.Range.y * .35f));
        
        end.z = Random.Range(-1f, 1) * 340;
        
        float d = (start - end).magnitude;
        float s = 500f / d;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * s;
            float l =  Mathf.SmoothStep(0, 1, t);
            localPos = Vector3.Lerp(start, end, l);
            
            yield return null;
        }
        
        moving = false;
    }


    protected override Vector2 Anim(float time)
    {
        return new Vector2(1 - Mathf.PingPong(Mathf.Clamp01((time - hitTime) * 2) * 5, 1), 0);
    }
    
    public Vector3 PathPos { get { return animPos; }}
}