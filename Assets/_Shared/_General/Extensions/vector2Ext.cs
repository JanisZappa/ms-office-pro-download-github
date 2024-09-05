using UnityEngine;


public static class vector2Ext 
{
    public static float Angle_Sign(this Vector2 dir1, Vector2 dir2)
    {
        return Vector3.Angle(dir1, dir2) * dir1.Side_Sign(dir2);
    }

    
    public static int Side_Sign(this Vector2 vector1, Vector2 vector2)
    {
        return LeftOrRight(vector1, vector2) < 0 ? -1 : 1;
    }

    
    private static float LeftOrRight(Vector2 A, Vector2 B)
    {
        return -A.x * B.y + A.y * B.x;
    }
    

    public static Vector2 Rot90(this Vector2 v, bool counterClockwise = true)
    {
        return counterClockwise? new Vector2(-v.y, v.x) : new Vector2(v.y, -v.x);
    }
    
    
    public static Vector2 Rot90(this Vector2 v, bool counterClockwise, float scale)
    {
        return counterClockwise? new Vector2(-v.y * scale, v.x * scale) : new Vector2(v.y * scale, -v.x * scale);
    }
    
    
    public static Vector2 Rot90Step(this Vector2 v, int multi = 1)
    {
        multi = (multi + 4 * 1000) % 4;
        for (int i = 0; i < multi; i++)
            v = new Vector2(v.y, -v.x);
        
        return v;
    }
    
    
    public static Vector2 Rot(this Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x;
        float ty = v.y;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    
    public static Vector2 Rot(this Vector2 v, float degrees, float scale)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x * scale;
        float ty = v.y * scale;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    
    public static Vector2 RotRad(this Vector2 v, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
         
        float tx = v.x;
        float ty = v.y;
 
        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
    
    
    public static Vector3 V3(this Vector2 v2, float z = 0)
    {
        return new Vector3(v2.x,v2.y, z);
    }
    
    
//  Cross Product  //
    public static float Cross(this Vector2 a, Vector2 b )
    {
    //  Two crossed vectors return a scalar  //
        return a.x * b.y - a.y * b.x;
    }
    
    
    public static Vector2 Cross(this Vector2 a, float s)
    {
    //  More exotic (but necessary) forms of the cross product //
    //  with a vector a and scalar s, both returning a vector  //
        return new Vector2(s * a.y, -s * a.x);
    }

    
    public static Vector2 Cross(this float s, Vector2 a )
    {
        return new Vector2(-s * a.y, s * a.x);
    }


    public static Vector2 RangeChecked(this Vector2 vector, float min, float max)
    {
        float x      = Mathf.Clamp(vector.x, min, max);
        float y      = Mathf.Clamp(vector.y, min, max);
        float center = (x + y) * .5f;
        
        return new Vector2(Mathf.Min(center, x), Mathf.Max(center, y));
    }
    
    
    public static float RadAngle(this Vector2 dirA, Vector2 dirB)
    {
        float radA = Mathf.Atan2(dirA.y, dirA.x);
        float radB = Mathf.Atan2(dirB.y, dirB.x);

        float diff = radB - radA;

        if (diff > 0)
            return diff < Mathf.PI ? diff : -(Mathf.PI * 2 - diff);
		
        return diff > -Mathf.PI ? diff : Mathf.PI * 2 + diff;
    }


    public static Vector2 AimPos(this Vector2 a, Vector2 b, float distance)
    {
        float vX = b.x - a.x;
        float vY = b.y - a.y;
        float multi = 1f / Mathf.Sqrt(vX * vX + vY * vY) * distance;
        
        return new Vector2(a.x + vX * multi, a.y + vY * multi);
    }
    
    
    public static Vector2 SetLength(this Vector2 a, float length)
    {
        float multi = 1f / Mathf.Sqrt(a.x * a.x + a.y * a.y) * length;
        
        return new Vector2(a.x * multi, a.y * multi);
    }

    
    public static float ToRadian(this Vector2 vector)
    {
        vector = vector.normalized;

        return Mathf.Atan2(vector.y, vector.x).Wrap(0, 2 * Mathf.PI);
    }
    
    
    public static Vector2 ToRadDir(this float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }


    public static bool Same(this Vector2 a, Vector2 b)
    {
        float x = b.x - a.x, y = b.y - a.y;

        return x * x + y * y < .000001f;
    }
    
    
    public static bool IsNaN(this Vector2 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y);
    }

    
    public static Vector2 SinCos(this Vector2 v, float s, float c)
    {
        return new Vector2(c * v.x - s * v.y, s * v.x + c * v.y);
    }


    public static Vector2 AddRandomRange(this Vector2 v, float min, float max)
    {
        return new Vector2(v.x + Random.Range(min, max), v.y + Random.Range(min, max));
    }
    
    
    public static Vector2 AddRandomRange(this Vector2 v, float range)
    {
        range *= .5f;
        return new Vector2(v.x + Random.Range(-range, range), v.y + Random.Range(-range, range));
    }
    
    
//  MinMax  //
    public static Vector2 Min(this Vector2 vec, Vector2 other)
    {
        return new Vector2(Mathf.Min(vec.x, other.x), Mathf.Min(vec.y, other.y));
    }
    
    public static Vector2 Max(this Vector2 vec, Vector2 other)
    {
        return new Vector2(Mathf.Max(vec.x, other.x), Mathf.Max(vec.y, other.y));
    }

    
    public static Vector2 Multi(this Vector2 vec, float multi)
    {
        return vec * multi;
    }
    
    public static Vector2 MultiBy(this Vector2 vec, Vector2 other)
    {
        return new Vector2(vec.x * other.x, vec.y * other.y);
    }
    
    public static Vector2 MultiX(this Vector2 vec, float multi)
    {
        vec.x *= multi;
        return  vec;
    }
    public static Vector2 MultiY(this Vector2 vec, float multi)
    {
        vec.y *= multi;
        return  vec;
    }
    

//  Crazy  //
    public static float ValueLerp(this Vector2 v, float lerp)
    {
        return Mathf.Lerp(v.x, v.y, lerp);
    }
}
