using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Compass : Singleton<Compass>
{
    public TextMeshPro mobileTxt;
    public static List<LocationMarker> markers = new List<LocationMarker>();

    private static int markerPick;

    public static LocationMarker Marker => markers.Count == 0? null : markers[markerPick % markers.Count];

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = 60;
            Input.compass.enabled = true;
            Input.gyro.enabled = true;
            StartCoroutine(LocationUpdate());
        }
    }


    private static float Angle
    {
        get
        {
            return Application.isMobilePlatform ? Input.compass.magneticHeading : Mathf.Sin(Time.unscaledTime * 4) * 15;
        }
    }


    private static readonly QuaternionForce rotForce = new QuaternionForce(200).SetSpeed(100).SetDamp(25);
    private static int frame = -1;
    public static Quaternion Rot
    {
        get
        {
            if (Time.frameCount != frame)
            {
                rotForce.Update(Quaternion.AngleAxis(Angle, Vector3.forward), Time.deltaTime);
                frame = Time.frameCount;
            }
            return rotForce.Value;
        }
    }

    public const float maxRadius = 1.1f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            markerPick++;

        DRAW.Circle(Vector3.zero, maxRadius * .1f, 100).SetColor(Color.white.A(.35f));
        DRAW.Circle(Vector3.zero, maxRadius * .4f, 100).SetColor(Color.white.A(.35f));
        DRAW.Circle(Vector3.zero, maxRadius * .7f, 100).SetColor(Color.white.A(.35f));
        DRAW.Circle(Vector3.zero, maxRadius, 100).SetColor(Color.white.A(.35f));

        Heading(Rot * Vector3.up, Color.red);
        Heading(Rot * Vector3.right, Color.white);
        Heading(Rot * -Vector3.right, Color.white);
        Heading(Rot * -Vector3.up, Color.white);

        if (Application.isMobilePlatform)
            mobileTxt.text = Marker.Info();
    }


    private static void Heading(Vector3 dir, Color c)
    {
        DRAW.Vector(dir * .65f, dir * (1.1f - .7f)).SetColor(c.A(.35f));
        DRAW.Circle(dir * 1.15f, .025f, 20).SetColor(c).Fill(.35f, true);
    }


    private IEnumerator LocationUpdate()
    {
        Input.location.Start(10, 10);

        while (Input.location.status == LocationServiceStatus.Initializing)
            yield return new WaitForSeconds(.25f);

        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Running)
            {
                LocationInfo lF = Input.location.lastData;
                UserC = new Vector2(lF.latitude, lF.longitude);
                UserP = CoolCompass.ToPos(UserC.x, UserC.y);
                GotLocation = true;
            }
           
            yield return new WaitForSeconds(1f);
        }
    }

    private static Vector2 UserC;
    private static Vector3 UserP;
    private static bool GotLocation;

    private static Vector2 CologneCoords;
    private static Vector3 ColognePos;
    private static bool GotCologne;


    public static Vector3 UserPos
    {
        get
        {
            if (GotLocation)
                return UserP;

            if (!GotCologne)
            {
                GotCologne = true;
                CologneCoords = CoolCompass.GetCoords("Cologne");
                ColognePos    = CoolCompass.GetPos("Cologne");
            }

            return ColognePos;
        }
    }

    public static Vector2 UserCoords
    {
        get
        {
            if (GotLocation)
                return UserC;

            if (!GotCologne)
            {
                GotCologne = true;
                CologneCoords = CoolCompass.GetCoords("Cologne");
                ColognePos = CoolCompass.GetPos("Cologne");
            }

            return CologneCoords;
        }
    }
}