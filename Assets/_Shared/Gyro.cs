using UnityEngine;

public static class Gyro
{
    static Gyro()
    {
        Input.gyro.enabled = true;
    }
    
    
    private static Quaternion baseRot = Quaternion.identity;

    
    public static Quaternion GetRotation(bool reset = false)
    {
        if(reset)
            ResetGyro();
        
        return Quaternion.Inverse(baseRot) * Input.gyro.attitude;
    }


    public static float TiltAngle
    {
        get
        {
            Vector2 dir = GetRotation() * Vector3.up;
            return Vector2.up.RadAngle(dir.normalized);
        }
    }
    
    public static float YawAngle
    {
        get
        {
            Vector3 d = GetRotation() * Vector3.up;
            Vector2 dir = new Vector2(d.z, d.y);
            return Vector2.up.RadAngle(dir.normalized);
        }
    }


    public static void ResetGyro()
    {
        baseRot = baseRot = Input.gyro.attitude;
    }
}
