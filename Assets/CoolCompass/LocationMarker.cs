using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocationMarker : MonoBehaviour
{
    [HideInInspector] public float angle;
    [HideInInspector] public float distance;

    private Color drawColor;
    private Vector2 crd;
    private Vector3 pos;
    private bool gotPos;

    void Start()
    {
        drawColor = GetDrawColor;
        Compass.markers.Add(this);
    }


    private void LateUpdate()
    {
        if (!gotPos)
        {
            crd = GetCoords();
            pos = GetPos();
            transform.position = pos;
            gotPos = true;
        }


        angle = CoolCompass.AngleFromUser(pos, Compass.UserPos);
        float bearing = CoolCompass.GetCoordAngle(crd, Compass.UserCoords);
        if(Application.isEditor && false)
            Debug.Log(name + " ... " + angle.ToString("F4") + " ... " + bearing.ToString("F4"));
        angle = bearing;

        distance = CoolCompass.GetCoordDist(crd, Compass.UserCoords);

        //  I want 100 to be 10 => .1 * 100 = 10
        float d = Mathf.Max(0, Mathf.Log10(distance * 100));
              //d = Mathf.Max(0, d + Mathf.Sin(Time.unscaledTime) + 1);

        d = Mathf.Min(1, .1f + .9f * d * .3333333f) * Compass.maxRadius;

        DRAW.Shape s = DRAW.Circle((Compass.Rot * Quaternion.AngleAxis(angle, Vector3.forward)) * Vector3.up * d, .03f * (Compass.Marker == this ? 1.4f : 1), 20);
        if(Compass.Marker == this)
            s.Fill(1).SetColor(drawColor);
        else
            s.SetColor(drawColor);
        //DRAW.Arrow(Vector3.zero, (Compass.Rot * Quaternion.AngleAxis(angle, Vector3.forward)) * Vector3.up * d, Mathf.Min(d, .05f * (Compass.Marker == this? 2 : 1))).Fill(1).SetColor(drawColor);
    }

    protected abstract Vector2 GetCoords();
    protected abstract Vector3 GetPos();


    private static Color GetDrawColor
    {
        get
        {
            Color.RGBToHSV(COLOR.yellow.fresh, out float H, out float S, out float V);
            H = (H + (ColorPick++) * .05f) % 1;
            return Color.HSVToRGB(H, S, V);
        }
    }

    private static int ColorPick;

    //BoulderPlanet 50.94505695287149, 6.904911506276612
    //Wo Ist Tom 50.92164114665979, 6.920447846372373
    //Brewbees 50.92369904760403, 6.911705282303335

    public string Info()
    {
        return name + " " + distance.ToString("F4");
    }
}
