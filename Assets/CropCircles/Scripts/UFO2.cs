using UnityEngine;


public class UFO2 : MonoBehaviour
{
    public float speed;
    public float hoverHeight;
    
    [Space]
    public CropCanvas canvas;
    public CropCam cam;
    
    private readonly QuaternionForce leanForce = new QuaternionForce(200).SetSpeed(49).SetDamp(4.35f);
    private readonly Vector3Force    accelForce = new Vector3Force(200).SetSpeed(142).SetDamp(228);
    private readonly FloatForce        dipForce = new FloatForce(200).SetSpeed(15).SetDamp(1.4f);
    private Transform trans;
    private Vector3 pos, vel;
    
    
    private void Start()
    {
        trans = transform;
    }

    private void Update()
    {
        float dt = Time.deltaTime;
        
        Vector3 steer = new Vector3(UFOInput.P1_Horizontal, 0, UFOInput.P1_Vertical);
        steer = steer.normalized * Mathf.Min(1, steer.magnitude);
        Vector3 lean = steer + accelForce.Value;
        
        
        float leanMag = lean.magnitude;
        Quaternion leanGoal = leanMag > 0
            ? Quaternion.AngleAxis(leanMag * -13f, Vector3.Cross(lean.normalized, Vector3.up))
            : Quaternion.identity;
        
        trans.rotation = leanForce.Update(leanGoal, dt);
        Vector3 u = leanForce.Value * Vector3.up;
        Vector3 v = accelForce.Update(u.SetY(0), dt);
        pos += v * dt * speed;
        
        trans.position = pos + Vector3.up * hoverHeight * (dipForce.Update(Mathf.Pow(u.y, 4), dt) * .9f + .1f);

        cam.UFOUpdate(pos);
        canvas.SetTarget(pos + v * .105f * speed);
    }
}
