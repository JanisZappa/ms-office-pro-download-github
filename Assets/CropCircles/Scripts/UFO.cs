using UnityEngine;


public class UFO : MonoBehaviour
{
    public float speed;
    public float hoverHeight;
    
    [Space]
    public CropCanvas canvas;
    public CropCam cam;

    private Vector3 pos, vel;
    private readonly Vector3Force    accelForce = new Vector3Force(200).SetSpeed(82).SetDamp(188);
    private readonly QuaternionForce  leanForce = new QuaternionForce(200).SetSpeed(41).SetDamp(4.35f);
    private readonly Vector3Force     jerkForce = new Vector3Force(200).SetSpeed(190).SetDamp(60);
    private readonly FloatForce        dipForce = new FloatForce(200).SetSpeed(12).SetDamp(5.5f);

    private Transform trans;
    private float hoverTime;


    private void Start()
    {
        trans = transform;
    }
    
    
    private void Update()
    {
        float dt = Time.deltaTime;
        
        Vector3 steer = new Vector3(UFOInput.P1_Horizontal, 0, UFOInput.P1_Vertical);
        steer = steer.normalized * Mathf.Min(1, steer.magnitude);
        
        Vector3 v = accelForce.Update(steer, dt);

        Vector3 lean = v - steer;
        float leanMag = lean.magnitude;
        Quaternion leanGoal = leanMag > 0
            ? Quaternion.AngleAxis(leanMag * speed * .0925f, Vector3.Cross(lean.normalized, Vector3.up))
            : Quaternion.identity;

        hoverTime += (2 + steer.magnitude * 4) * dt * 1.75f;

        Vector3 j = jerkForce.Update((leanGoal * Vector3.up).SetY(0) * .95f, dt);
        
        pos += (v + j) * dt * speed;
        trans.position = pos + Vector3.up * (hoverHeight + dipForce.Update(v.magnitude * 2+ leanMag * speed * -.0075f + Mathf.Sin(hoverTime) * .025f * (steer.magnitude * 2 + 1), dt));
        trans.rotation = leanForce.Update(leanGoal, dt);
            
        cam.UFOUpdate(pos);
        canvas.SetTarget(pos + v * .1f * speed);
    }
}
