using UnityEngine;


public class Hover : MonoBehaviour
{
    public float distance;
    public float springstrength, springdamp;
    public LayerMask ground;
    
    private Ray[] rays;
    private Transform trans;
    private int rayCount;
    
    private Rigidbody rB;

    
    private void Start()
    {
        trans = transform;
        rB = GetComponent<Rigidbody>();
        
        const float c = .5f;
        if(true)
            rays = new []
            {
                new Ray(new Vector3(-c, -c, -c), new Vector3(-1, 0, 0)), 
                new Ray(new Vector3(-c, -c,  c), new Vector3(-1, 0, 0)), 
                new Ray(new Vector3(-c,  c,  c), new Vector3(-1, 0, 0)), 
                new Ray(new Vector3(-c,  c, -c), new Vector3(-1, 0, 0)), 
                
                new Ray(new Vector3( c, -c, -c), new Vector3( 1, 0, 0)), 
                new Ray(new Vector3( c, -c,  c), new Vector3( 1, 0, 0)), 
                new Ray(new Vector3( c,  c,  c), new Vector3( 1, 0, 0)), 
                new Ray(new Vector3( c,  c, -c), new Vector3( 1, 0, 0)), 
                
                new Ray(new Vector3(-c, -c, -c), new Vector3( 0, 0, -1)), 
                new Ray(new Vector3( c, -c, -c), new Vector3( 0, 0, -1)), 
                new Ray(new Vector3( c,  c, -c), new Vector3( 0, 0, -1)), 
                new Ray(new Vector3(-c,  c, -c), new Vector3( 0, 0, -1)), 
                
                new Ray(new Vector3(-c, -c,  c), new Vector3( 0, 0, 1)), 
                new Ray(new Vector3( c, -c,  c), new Vector3( 0, 0, 1)), 
                new Ray(new Vector3( c,  c,  c), new Vector3( 0, 0, 1)), 
                new Ray(new Vector3(-c,  c,  c), new Vector3( 0, 0, 1)), 
                
                new Ray(new Vector3(-c, -c, -c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3(-c, -c,  c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3( c, -c,  c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3( c, -c, -c), new Vector3( 0, -1, 0)), 
                
                new Ray(new Vector3(-c,  c, -c), new Vector3( 0, 1, 0)), 
                new Ray(new Vector3(-c,  c,  c), new Vector3( 0, 1, 0)), 
                new Ray(new Vector3( c,  c,  c), new Vector3( 0, 1, 0)), 
                new Ray(new Vector3( c,  c, -c), new Vector3( 0, 1, 0)), 
            };
        else
            rays = new []
            {
                new Ray(new Vector3(-c, -c, -c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3(-c, -c,  c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3( c, -c,  c), new Vector3( 0, -1, 0)), 
                new Ray(new Vector3( c, -c, -c), new Vector3( 0, -1, 0)), 
            };
        rayCount = rays.Length;
    }


    private void Update()
    {
        return;
        for (int i = 0; i < rayCount; i++)
        {
            Ray r = rays[i];
            Vector3 p = trans.TransformPoint(r.origin);
            Vector3 n = trans.TransformDirection(r.direction);
            
            DRAW.Vector(p, n * distance).SetColor(Color.magenta);
        }
    }


    private void FixedUpdate()
    {
        for (int i = 0; i < rayCount; i++)
        {
            Ray r = rays[i];
            Vector3 p = trans.TransformPoint(r.origin);
            Vector3 n = trans.TransformDirection(r.direction);
            Vector3 u = -n;
            
            Vector3 currentVel = rB.GetPointVelocity(p);
            
            if (Physics.Raycast(new Ray(p, n), out RaycastHit hit, distance, ground))
            {
                float offset       = distance - hit.distance;
                float vel          = Vector3.Dot(u, currentVel);
                float f            = offset * springstrength - vel * springdamp;
           
                rB.AddForceAtPosition(u * f, p);
            }
            
            //rB.AddForceAtPosition(currentVel * -.01f, p);
        }
    }
}
