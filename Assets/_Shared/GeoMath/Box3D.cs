using UnityEngine;

public static class Box3D
{
    public static Vector3 ClosestPoint(Vector3 center, Vector3 size, Vector3 point)
    {
        size *= .5f;
        return new Vector3(Mathf.Clamp(point.x, center.x - size.x, center.x + size.x), 
                           Mathf.Clamp(point.y, center.y - size.y, center.y + size.y),
                           Mathf.Clamp(point.z, center.z - size.z, center.z + size.z));
    }
    
    
    public static bool SphereHit(Vector3 center, Vector3 size, Vector3 point, float radius)
    {
        Vector3 hit = ClosestPoint(center, size, point);
        
        return (hit - point).sqrMagnitude <= radius * radius;
    }
    
    
    public static bool SphereHit(Vector3 center, Vector3 size, Vector3 point, float radius, out Vector3 hit)
    {
        hit = ClosestPoint(center, size, point);
        
        return (hit - point).sqrMagnitude <= radius * radius;
    }
}
