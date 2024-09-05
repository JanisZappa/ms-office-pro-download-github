using UnityEngine;


public class CoasterCart : MonoBehaviour
{
    [Range(0, 1)]
    public float start;

    [Space] public int cartCount;
    private Transform[] carts;

    public float dist;
    public float vel;

    private CoasterTrack track;
    
    private Stepper stepper;
    
    
    private void Start()
    {
        track = FindObjectOfType<CoasterTrack>();

        dist = track.trackLength * start;
        
        stepper = new Stepper(200, Step);
        
        cartCount = transform.childCount;
        carts = new Transform[cartCount];
        for (int i = 0; i < cartCount; i++)
            carts[i] = transform.GetChild(i);
    }

    
    private void Step(float dt)
    {
        float newVel = 0;

        float dtdamp = track.damp * dt;
        float dtaccel = track.accel * dt * cartCount;
        for (int i = 0; i < cartCount; i++)
        {
            TrackPoint cartPt = track.GetTrackPoint(dist - track.cartStep * i);
            
            float addVel = vel * (1f - dtdamp);
            addVel += cartPt.gradient * dtaccel;

            newVel += addVel;
        }

        vel = newVel / cartCount;

        dist += vel * track.speed * dt;
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            start = Random.Range(0f, 1f);
            dist = track.trackLength * start;
            vel = 0;
        }
        
        stepper.Update(Time.deltaTime);
        
        for (int i = 0; i < cartCount; i++)
            track.GetTrackPoint(dist - track.cartStep * i).Placement.Apply(carts[i]);
    }
}
