using UnityEngine;

public class CityMarker : MonoBehaviour
{
    public float angleFromCologne;
    public float distanceFromCologne;

    private Color drawColor;
    
    
    private void Start()
    {
        Vector3 pos = CoolCompass.GetPos(name);

        transform.position = pos;

        angleFromCologne = CoolCompass.AngleFromCologne(pos);
        distanceFromCologne = CoolCompass.GetDistanceFromCologne(pos);

        drawColor = GetDrawColor;
    }


    private void LateUpdate()
    {
        float a = Mathf.Clamp(.125f - distanceFromCologne * .00005f, .02f, .125f);
        float b = Mathf.Clamp(.25f + distanceFromCologne * .0015f, .25f, 1);
        DRAW.Arrow(Vector3.zero, (Compass.Rot * Quaternion.AngleAxis(angleFromCologne, Vector3.forward)) * Vector3.up * b, a).Fill(1).SetColor(drawColor);
    }


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
}
