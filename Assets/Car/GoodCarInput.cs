using UnityEngine;


public class GoodCarInput : MonoBehaviour
{
    public Camera cam;
    public Vector2 analogPos;
    public float analogSize;
    
    private float steer, accel;

    private bool touch;
    private float touchSteer, touchAccel;

    private float Steering =>
        Mathf.Clamp(
            (Input.GetKey(KeyCode.A)? -1 : 0) + (Input.GetKey(KeyCode.D)? 1 : 0) +
            touchSteer
            , -1, 1);

    private float Acceleration =>
        Mathf.Clamp(
            (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) ? 1 : 0) +
            (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) ? -1 : 0) +
            touchAccel +
            LR
            , -1, 1);


    private static float LR => Input.GetAxis("LR");

    public Vector2 GetSteerAndAccel()
    {
        //return AnalogAccel(carRot);
        //Debug.Log(LR);
        float dt = .02f / Time.fixedDeltaTime;

        float s = Mathf.Pow(Mathf.Abs(Steering), 1.666666f) * Mathf.Sign(Steering);
        float a = Acceleration;//Mathf.Pow(Mathf.Abs(Acceleration), 1.666666f) * Mathf.Sign(Acceleration);
        steer = Mathf.Lerp(steer, s, Time.fixedDeltaTime * 50 * dt);
        accel = Mathf.Lerp(accel, a, Time.fixedDeltaTime * 50 * dt);
        return new Vector2(steer, accel);
    }
    
    private void LateUpdate()
    {
        Vector2 center = cam.ViewportToScreenPoint(analogPos);
        float radius = cam.ViewportToScreenPoint(Vector2.up * analogSize).y;
        
        touch = false;
        if (Input.GetMouseButton(0))
        {
            touch = AnalogTouch(center, radius, Input.mousePosition);
        }

        if (!touch)
        {
            touchSteer = 0;
            touchAccel = 0;
        }
    }


    private Vector2 AnalogAccel(Quaternion carRot)
    {
        Vector2 analog = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float amount = Mathf.Min(analog.magnitude, 1);
                analog = analog.normalized;

        Vector2 carForward = carRot * Vector3.up;
        Vector2 carRight   = carRot * Vector3.right;
        
        float d  = Vector2.Dot(analog, carForward);
        float d2 = Vector2.Dot(analog, carRight);
        
        return new Vector2(d2 * (d >= 0? 1 : -1), d >= 0? amount : -amount);
        
         
         return Vector2.zero;
    }


    private bool AnalogTouch(Vector2 center, float radius, Vector2 pos)
    {
        Vector2 dir = pos - center;
        if(dir.magnitude > radius)
            return false;

        dir = dir.normalized;

        const float thresh = .15f, tMulti = 1f / (1f - thresh), pow = 2f;
        touchSteer = dir.x > thresh ? Mathf.Pow((dir.x - thresh) * tMulti, pow) : dir.x < -thresh ? -Mathf.Pow((-dir.x - thresh) * tMulti, pow) : 0;
        touchAccel = dir.y > thresh ? Mathf.Pow((dir.y - thresh) * tMulti, pow) : dir.y < -thresh ? -Mathf.Pow((-dir.y - thresh) * tMulti, pow) : 0;
        return true;
    }
}
