using UnityEngine;

public static class World
{
    public static Bounds Bounds;
    static World() { Bounds = new Bounds(Vector3.zero, Vector3.one * 10000); }
}
