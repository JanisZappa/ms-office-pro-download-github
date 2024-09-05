using UnityEngine;


public class Fingers : MonoBehaviour
{
    public Rigidbody2D finger;
    
    public float radius;
    
    public float frequency;
    public float damping;
    
    [Space]
    public float breakForce;
    
    private float kp, kd;
    private Vector2 pos;
    
    private Camera cam;
    
    
    private SpringJoint2D fingerJoint;


    private void Start()
    {
        cam = Camera.main;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = MousePos;
            Collider2D coll = Physics2D.OverlapCircle(mouse, radius);
            if (coll != null)
            {
                Rigidbody2D rb2 = coll.attachedRigidbody;

                if (rb2 != null)
                {
                    Vector2 root = finger.transform.position;
                    float dist = (root - mouse).magnitude;
                    
                    fingerJoint = finger.gameObject.AddComponent<SpringJoint2D>();
                    fingerJoint.autoConfigureDistance = false;
                    fingerJoint.autoConfigureConnectedAnchor = false;
                    fingerJoint.connectedBody   = rb2;
                    fingerJoint.distance        = dist;
                    fingerJoint.dampingRatio    = .5f;
                    fingerJoint.frequency       = 20;
                    fingerJoint.breakForce      = breakForce;
                       
                    fingerJoint.anchor = Vector2.zero;
                    fingerJoint.connectedAnchor = coll.transform.InverseTransformPoint(mouse);
                }
            }
           
        }
        
        if (Input.GetMouseButtonUp(0) && fingerJoint != null)
            Destroy(fingerJoint);
    }
    

    private void LateUpdate()
    {
        Vector3 root = finger.transform.position;
        DRAW.Circle(root, radius, 40).SetColor(COLOR.orange.coral);
    }


    private Vector2 MousePos
    {
        get
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            return r.origin;
        }
    }


    private void FixedUpdate()
    {
        CalcuateKPKD();
        
        Vector2 oldP = pos;
        Vector2 newP = MousePos;
        pos = newP;
        
        Vector3 vel = (newP - oldP) / Time.fixedDeltaTime;
        UpdatePos(newP, vel);
    }


    private void CalcuateKPKD()
    {
        kp = (6f*frequency)*(6f*frequency)* 0.25f;
        kd = 4.5f*frequency*damping;
    }


    private void UpdatePos(Vector2 Pdes, Vector2 Vdes)
    {
        float dt  = Time.fixedDeltaTime;
        float g   = 1 / (1 + kd * dt + kp * dt * dt);
        float ksg = kp * g;
        float kdg = (kd + kp * dt) * g;
        Vector2 Pt0 = finger.transform.position;
        Vector2 Vt0 = finger.velocity;
        
        Vector2 dir = Pdes - Pt0;
        Vector2 F = dir * ksg + (Vdes - Vt0) * kdg;
        finger.AddForce (F);
    }
}
