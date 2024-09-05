using UnityEngine;


public class HoverDam : MonoBehaviour
{
    public Mesh rayMesh;
    public LayerMask mask;

    public float springstrength, springdamp;
    public float distance;

    private Vector3[] rays;
    private Rigidbody rB;
    private int rayCount;

    [Space] public float speed;
    
    
    private void Start()
    {
        rays = rayMesh.vertices;
        rayCount = rays.Length;
        rB = GetComponent<Rigidbody>();
    }

    
    private void FixedUpdate()
    {
        Vector3 steer = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        float steerAmount = Mathf.Min(steer.magnitude, 1);

        steer = steer.normalized;

        Vector3 weightSteer = Vector3.zero;
        int hits = 0;
        Vector3 p = rB.position;
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 d = rays[i];
            Vector3 u = -d;
            
            Vector3 currentVel = rB.GetPointVelocity(p + d * .5f);
            
            if (Physics.Raycast(new Ray(p, d), out RaycastHit hit, distance, mask))
            {
                float offset = distance - hit.distance;
                float vel    = Vector3.Dot(u, currentVel);
                float f      = offset * springstrength - vel * springdamp;
                float ndot   = Vector3.Dot(hit.normal, u);

                weightSteer += Vector3.ProjectOnPlane(steer, hit.normal).normalized * ndot;
                hits++;
                rB.AddForceAtPosition(u * f * ndot, p);
            }
            
            //rB.AddForceAtPosition(currentVel * -.01f, p);
        }
        
        rB.AddForce((hits == 0? steer : weightSteer).normalized * steerAmount * speed);
    }
}
