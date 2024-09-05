using UnityEngine;


public class LookAtSpring : MonoBehaviour
{
    public Transform target;
    
    public float speed, damp;
    
    [Space]
    public bool animate;
    public float frequency, range;
    [Range(0, 1)]
    public float randomness;
    
    private Transform trans;
    private float t, next;
    
    private Quaternion look;
    private readonly QuaternionForce force = new QuaternionForce(200);
    
    
    private void Start()
    {
        trans = transform;
        look = Quaternion.LookRotation((target.position - trans.position).normalized.SetY(0), Vector3.up);
    }

    
    private void Update()
    {
        force.SetSpeed(speed).SetDamp(damp);

        if (animate)
        {
            t += Time.deltaTime;
            if (t >= next)
            {
                t -= next;
                next = frequency * (1f + Random.Range(-1, 1f) * randomness);
                
                Vector3 p1 = target.position, p2 = trans.position;
                float dist = (p2 - p1).magnitude;
                look = Quaternion.LookRotation((p1 + Random.insideUnitSphere * range * (1 + dist * 4) - p2).normalized.SetY(0), Vector3.up);
            }
        }
        else
            look = Quaternion.LookRotation((target.position - trans.position).normalized.SetY(0), Vector3.up);
        
        trans.rotation = force.Update(look, Time.deltaTime);
    }
}
