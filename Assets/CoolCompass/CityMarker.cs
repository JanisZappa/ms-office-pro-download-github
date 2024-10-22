using UnityEngine;

public class CityMarker : LocationMarker
{
    protected override Vector3 GetPos()
    {
        return CoolCompass.GetPos(name);
    }

    protected override Vector2 GetCoords()
    {
        return CoolCompass.GetCoords(name);
    }
}