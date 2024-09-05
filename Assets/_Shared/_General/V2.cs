using UnityEngine;
using R = UnityEngine.Random;

public static class V2
{
    public static readonly Vector2 zero  = Vector2.zero;
    public static readonly Vector2 one   = Vector2.one;
    public static readonly Vector2 right = Vector2.right;
    public static readonly Vector2 left  = Vector2.left;
    public static readonly Vector2 up    = Vector2.up;
    public static readonly Vector2 down  = Vector2.down;
    public static readonly Vector2 away  = new Vector2(0, -1000);

    
    public static Vector2 Random(float range)
    {
        return new Vector2(R.Range(-range, range), R.Range(-range, range)) * .5f;
    }
    
    
    public static float InverseLerp(Vector2 a, Vector2 dir, Vector2 value)
    {
        Vector2 AV = value - a;
        return Vector2.Dot(AV, dir) / Vector2.Dot(dir, dir);
    }
}
