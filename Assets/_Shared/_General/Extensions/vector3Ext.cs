using UnityEngine;


public static class vector3Ext 
{
    public static Vector3 MultiBy(this Vector3 vector1, Vector3 vector2)
    {
        return new Vector3(vector1.x * vector2.x, vector1.y * vector2.y, vector1.z * vector2.z);
    }
    
    
    public static Vector3 Scale(this Vector3 vector, float x = 1, float y = 1, float z = 1 )
    {
        return new Vector3(vector.x * x, vector.y * y, vector.z * z);
    }
    
    
    public static float LeftOrRight(Vector3 A, Vector3 B)
    {
        return -A.x * B.y + A.y * B.x;
    }
    
    
    public static Vector2 V2(this Vector3 v3)
    {
        return new Vector2(v3.x,v3.y);
    }
    
    public static Vector2 V2UseZ(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }
    
    
    public static Vector3 Pow(this Vector3 vector, float power)
    {
        return new Vector3(Mathf.Pow(vector.x, power), Mathf.Pow(vector.y, power), Mathf.Pow(vector.z, power));
    }

    
//  Set a Float  //
    public static Vector3 SetX(this Vector3 vector, float value)
    {
        return new Vector3(value, vector.y, vector.z);
    }
    
    
    public static Vector3 SetY(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, value, vector.z);
    }
    
    
    public static Vector3 SetZ(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y, value);
    }

    
//  Add to Float  //
    public static Vector3 AddX(this Vector3 vector, float value)
    {
        return new Vector3(vector.x + value, vector.y, vector.z);
    }
    
    
    public static Vector3 AddY(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y + value, vector.z);
    }
    
    
    public static Vector3 AddZ(this Vector3 vector, float value)
    {
        return new Vector3(vector.x, vector.y, vector.z + value);
    }
    
    
//  Flip a Float  // 
    public static Vector3 FlipX(this Vector3 vector)
    {
        return new Vector3(-vector.x, vector.y, vector.z);
    }
    
    
    public static Vector3 FlipY(this Vector3 vector)
    {
        return new Vector3(vector.x, -vector.y, vector.z);
    }
    
    
    public static Vector3 FlipZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, -vector.z);
    }
    
    
//  Multi a Float //
    public static Vector3 MultiX(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x * multi, vector.y, vector.z);
    }
    
    
    public static Vector3 MultiY(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x, vector.y * multi, vector.z);
    }
    
    
    public static Vector3 MultiZ(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x, vector.y, vector.z * multi);
    }
    
    
    public static Vector3 MultiXY(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x * multi, vector.y * multi, vector.z);
    }
    
    
    public static Vector3 MultiXZ(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x * multi, vector.y, vector.z * multi);
    }
    
    
    public static Vector3 MultiYZ(this Vector3 vector, float multi)
    {
        return new Vector3(vector.x, vector.y * multi, vector.z * multi);
    }


    public static bool Closeish(this Vector3 a, Vector3 b)
    {
        return (a - b).sqrMagnitude < .01f;
    }
    
    
//  Abs a Channel
    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
    
    
    public static Vector3 AbsX(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), vector.y, vector.z);
    }
    
    public static Vector3 AbsY(this Vector3 vector)
    {
        return new Vector3(vector.x, Mathf.Abs(vector.y), vector.z);
    }
    
    public static Vector3 AbsZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, Mathf.Abs(vector.z));
    }

    
    
//  Rotate Around Axis  //
    public static Vector3 RotZ(this Vector3 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x;
        float ty = v.y;
 
        return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty, v.z);
    }
    
    public static Vector3 RotZ(this Vector3 v, float degrees, float multi)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x * multi;
        float ty = v.y * multi;
 
        return new Vector3(cos * tx - sin * ty, sin * tx + cos * ty, v.z * multi);
    }
    
    
    public static Vector3 RotY(this Vector3 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = -v.x;
        float ty = -v.z;
 
        return new Vector3(cos * tx - sin * ty, v.y, sin * tx + cos * ty);
    }
    
    public static Vector3 RotY(this Vector3 v, float degrees, float multi)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = -v.x * multi;
        float ty = -v.z * multi;
 
        return new Vector3(cos * tx - sin * ty, v.y * multi, sin * tx + cos * ty);
    }
    
    
    public static Vector3 RotX(this Vector3 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = -v.z;
        float ty = v.y;
 
        return new Vector3(v.x, sin * tx + cos * ty, -(cos * tx - sin * ty));
    }


    public static void PrefsSave(this Vector3 vector, string name)
    {
        PlayerPrefs.SetFloat(name + "X", vector.x);
        PlayerPrefs.SetFloat(name + "Y", vector.y);
        PlayerPrefs.SetFloat(name + "Z", vector.z);
    }
    
    public static Vector3 PrefsLoad(this string name)
    {
        return new Vector3(PlayerPrefs.GetFloat(name + "X"), PlayerPrefs.GetFloat(name + "Y"), PlayerPrefs.GetFloat(name + "Z"));
    }


    public static bool IsNaN(this Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }


    public static Vector3 V3YToZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }
    
    
//  Set  //
    public static Vector3 Set(this Vector3 vec, Vector2 xy, float z)
    {
        return new Vector3(xy.x, xy.y, z);
    }
    
    public static Vector3 Set(this Vector3 vec, float x, Vector2 yz)
    {
        return new Vector3(x, yz.x, yz.y);
    }
    

    public static Vector3 Min(this Vector3 vec, Vector3 other)
    {
        return new Vector3(Mathf.Min(vec.x, other.x), Mathf.Min(vec.y, other.y), Mathf.Min(vec.z, other.z));
    }
    
    public static Vector3 Max(this Vector3 vec, Vector3 other)
    {
        return new Vector3(Mathf.Max(vec.x, other.x), Mathf.Max(vec.y, other.y), Mathf.Max(vec.z, other.z));
    }
    
//  Floor Ceil Round  //    
    public static Vector3 Floor(this Vector3 vec)
    {
        return new Vector3(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z));
    }
    
    public static Vector3 Ceil(this Vector3 vec)
    {
        return new Vector3(Mathf.Ceil(vec.x), Mathf.Ceil(vec.y), Mathf.Ceil(vec.z));
    }
    
    public static Vector3 Round(this Vector3 vec)
    {
        return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y), Mathf.Round(vec.z));
    }
    
//  Multiply Divide  //
    public static Vector3 Multiply(this Vector3 vec, Vector3 other)
    {
        return new Vector3(vec.x * other.x, vec.y * other.y, vec.z * other.z);
    }
    
    public static Vector3 Divide(this Vector3 vec, Vector3 other)
    {
        return new Vector3(vec.x / other.x, vec.y / other.y, vec.z / other.z);
    }


    public static Vector3 Clamp(this Vector3 vec, float max)
    {
        return vec.normalized * Mathf.Min(max, vec.magnitude);
    }
    
    
//  Vector3Int  //
    public static Vector3Int Vector3Int(this Vector3 vec)
    {
        return new Vector3Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y), Mathf.FloorToInt(vec.z));
    }
}