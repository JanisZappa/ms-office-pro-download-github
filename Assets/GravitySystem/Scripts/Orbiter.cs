using UnityEngine;


public class Orbiter : MonoBehaviour
{ 
    public float radius;
    [Range(0, 1)]
    public float eccentricity;

    [Space]
    public float angle, speed;

    [Space] 
    public bool lit;

    [Space] 
    public Orbiter parent;

    private Transform planet;

    private Vector3 Pos => planet.position;
    public int chainID;
    
    
    private void Start()
    {
        planet = transform.GetChild(0);

        Orbiter nextUp = parent;
        while (nextUp != null)
        {
            chainID++;
            nextUp = nextUp.parent;
        }
        
        OrbitMaster.AddOrbiter(this);
    }

    
    public Vector4 OrbitUpdate(float t)
    {
        if (parent != null)
            transform.position = parent.Pos;
        
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        planet.localPosition = rot * Kepler.GetPosition(t * speed, eccentricity) * radius;

        return new Vector4().Set(Pos, planet.localScale.x);
    }
}
