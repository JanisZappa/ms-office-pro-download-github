using UnityEngine;


public static class V3Volume 
{
    public static Vector3 VolumeScaleX(this Vector3 vector, float scale)
    {
        float volume = vector.x * vector.y * vector.z;
        float factor = Mathf.Sqrt(volume / (vector.x * scale)) / Mathf.Sqrt(vector.y * vector.z);
    
        return new Vector3(vector.x * scale, vector.y  * factor, vector.z * factor);
    }
    
    
    public static Vector3 VolumeScaleY(this Vector3 vector, float scale)
    {
        float volume = vector.x * vector.y * vector.z;
        float factor = Mathf.Sqrt(volume / (vector.y * scale)) / Mathf.Sqrt(vector.x * vector.z);
  
        return new Vector3(vector.x * factor, vector.y  * scale, vector.z * factor);
    }
    
    
    public static Vector3 VolumeScaleZ(this Vector3 vector, float scale)
    {
        float volume = vector.x * vector.y * vector.z;
        float factor = Mathf.Sqrt(volume /  (vector.z * scale)) / Mathf.Sqrt(vector.x * vector.y);

        return new Vector3(vector.x * factor, vector.y  * factor, vector.z * scale);
    }


    public static Vector3 GetFactors(this Vector3 vector, Vector3 otherVector)
    {
        return new Vector3(otherVector.x / vector.x, otherVector.y / vector.y, otherVector.z / vector.z);
    }


    public static Vector3 ZeroDamp(this Vector3 vector)
    {
        return new Vector3(Mth.DampedRange(vector.x, 1, 0), Mth.DampedRange(vector.y, 1, 0), Mth.DampedRange(vector.z, 1, 0));
    }
}
