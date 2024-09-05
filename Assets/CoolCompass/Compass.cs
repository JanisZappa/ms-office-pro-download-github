using TMPro;
using UnityEngine;

public class Compass : MonoBehaviour
{
    public TextMeshPro mobileTxt;
    
    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            Input.location.Start();
            Input.compass.enabled = true;
            Input.gyro.enabled = true;
        }
    }


    private static float Angle
    {
        get
        {
            return Application.isMobilePlatform ? Input.compass.trueHeading : Mathf.Sin(Time.unscaledTime * 4) * 15;
        }
    }


    private static readonly QuaternionForce f = new QuaternionForce(200).SetSpeed(70).SetDamp(20);
    private static int frame = -1;
    public static Quaternion Rot
    {
        get
        {
            if (Time.frameCount != frame)
            {
                f.Update(Quaternion.AngleAxis(Angle, Vector3.forward), Time.deltaTime);
                frame = Time.frameCount;
            }
            return f.Value;
        }
    }

    
    private void Update()
    {
        DRAW.Circle(Vector3.zero, 1.1f, 100).SetColor(Color.white.A(.35f));

        Heading(Rot *  Vector3.up, Color.red);
        Heading(Rot *  Vector3.right, Color.white);
        Heading(Rot * -Vector3.right, Color.white);
        Heading(Rot * -Vector3.up, Color.white);

        if (Application.isMobilePlatform)
        {
            mobileTxt.text = "True: " + Input.compass.trueHeading + "\nMagnetic: " + Input.compass.magneticHeading
                + "\n Gyro: " + Input.gyro.attitude.eulerAngles.y + "\n" + (Input.gyro.enabled? "ON" : "OFF");
        }
    }


    private static void Heading(Vector3 dir, Color c)
    {
        DRAW.Vector(dir * .65f, dir * (1.1f -.7f)).SetColor(c.A(.35f));
        DRAW.Circle(dir * 1.15f, .025f, 20).SetColor(c).Fill(.35f, true);
    }
}
