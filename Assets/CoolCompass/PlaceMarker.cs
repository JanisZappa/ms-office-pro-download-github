using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMarker : LocationMarker
{
    public Vector2 coords;

    protected override Vector2 GetCoords()
    {
        return coords;
    }

    protected override Vector3 GetPos()
    {
        return CoolCompass.ToPos(coords.x, coords.y);
    }
}
