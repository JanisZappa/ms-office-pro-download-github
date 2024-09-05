using UnityEngine;


public static class Rot
{
    public static readonly Quaternion Zero = Quaternion.identity;
    public static readonly Quaternion Y180 = Y(180);
    
    public static Quaternion Limit(this Quaternion a, float value)
    {
        return Quaternion.Slerp(Rot.Zero, a, value);
    }
    
    
//  Axis Rot  //
    public static Quaternion X(float angle)
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
            return Quaternion.AngleAxis(angle, Vector3.right);
        #else
            angle *= Mathf.Deg2Rad * .5f;
            return new Quaternion(Mathf.Sin(angle), 0, 0, Mathf.Cos(angle));
        #endif
    }
    
    
    public static Quaternion Y(float angle)
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
            return Quaternion.AngleAxis(angle, Vector3.up);   
        #else
            angle *= Mathf.Deg2Rad * .5f;
            return new Quaternion(0, Mathf.Sin(angle), 0, Mathf.Cos(angle));
        #endif
    }
    
    
    public static Quaternion Z(float angle)
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
            return Quaternion.AngleAxis(angle, Vector3.forward);   
        #else
            angle *= Mathf.Deg2Rad * .5f;
            return new Quaternion(0, 0, Mathf.Sin(angle), Mathf.Cos(angle));
        #endif
    }
}
