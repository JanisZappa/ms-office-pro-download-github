using UnityEngine;


public static class TriMath
{
    public static Vector3 GetBarycentricCoordinates(Vector3 f, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = b - a, v1 = c - a, v2 = f - a;
        float d00 = Vector3.Dot(v0, v0);
        float d01 = Vector3.Dot(v0, v1);
        float d11 = Vector3.Dot(v1, v1);
        float d20 = Vector3.Dot(v2, v0);
        float d21 = Vector3.Dot(v2, v1);
        float denom = 1f / (d00 * d11 - d01 * d01);
        float v = (d11 * d20 - d01 * d21) * denom;
        float w = (d00 * d21 - d01 * d20) * denom;
        float u = 1f - v - w;
        return new Vector3(u, v, w);
    }
    
    
    public static bool LinePlaneIntersection(Vector3 a, Vector3 b, Vector3 normal, Vector3 coord, out float x)
    {
        Vector3 ray = b - a;
        
        float nr = Vector3.Dot(normal, ray);
        if (Mathf.Abs(nr) < .00001f) 
        {
            x = 0;
            return false;
        }

        x = (Vector3.Dot(normal, coord) - Vector3.Dot(normal, a)) / nr;
        
        return x >= 0 && x <= 1;
    }


    public static bool TriPlaneIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 normal, Vector3 coord,
        out Vector3 uvw, out Vector3 uvw2)
    {
        bool hitA = LinePlaneIntersection(a, b, normal, coord, out float cA);
        bool hitB = LinePlaneIntersection(b, c, normal, coord, out float cB);
        bool hitC = LinePlaneIntersection(c, a, normal, coord, out float cC);

        if ((hitA ? 1 : 0) + (hitB ? 1 : 0) + (hitC ? 1 : 0) == 2)
        {
            uvw  = hitA ? new Vector3(1f - cA, cA, 0) : new Vector3(0, 1f - cB, cB);
            uvw2 = hitC ? new Vector3(cC, 0, 1f - cC) : new Vector3(0, 1f - cB, cB);
            return true;
        }
        
        uvw  = Vector3.zero;
        uvw2 = Vector3.zero;
        return false;
    }
    
    
    public static bool TriPlaneIntersection(Vector3 a, Vector3 b, Vector3 c, Vector3 normal, Vector3 coord,
        out EdgeHit outA, out EdgeHit outB)
    {
        bool hitA = LinePlaneIntersection(a, b, normal, coord, out float xA);
        bool hitB = LinePlaneIntersection(b, c, normal, coord, out float xB);
        bool hitC = LinePlaneIntersection(c, a, normal, coord, out float xC);

        if ((hitA ? 1 : 0) + (hitB ? 1 : 0) + (hitC ? 1 : 0) == 2)
        {
            outA = hitA ? new EdgeHit(0, xA) : new EdgeHit(1, xB);
            outB = hitC ? new EdgeHit(2, xC) : new EdgeHit(1, xB);
            return true;
        }
        
        outA = new EdgeHit();
        outB = new EdgeHit();
        return false;
    }
}


public struct EdgeHit
{
    public int id;
    public float lerp;

    public EdgeHit(int id, float lerp)
    {
        this.id = id;
        this.lerp = lerp;
    }
}
