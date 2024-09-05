using UnityEngine;


namespace GeoMath
{
    public static class Tri
    {
        public static Vector2 HitPoint, HitNormal;
        public static Vector2 HitNormalInverse { get { return -HitNormal; } }
        
        
        public static Vector3 BaryWeight(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 p)
        {
            float d  = 1f / ((v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y));
            float f1 = p.x - v3.x, 
                  f2 = p.y - v3.y;
            
            float w1 = ((v2.y - v3.y) * f1 + (v3.x - v2.x) * f2) * d;
            float w2 = ((v3.y - v1.y) * f1 + (v1.x - v3.x) * f2) * d;
          
            return new Vector3(w1, w2, 1 - w1 - w2);
        }
        
        
        public static Vector2 BaryPos(Vector2 v1, Vector2 v2, Vector2 v3, Vector3 baryWeights)
        {
            return v1 * baryWeights.x + v2 * baryWeights.y + v3 * baryWeights.z;
        }


        public static bool Contains(Vector2 p1, Vector2 p2, Vector2 p3, Circle circle)
        {
            if (circle.Contains(p1))
            {
                HitPoint  = p1;
                HitNormal = (HitPoint - circle.center).normalized;
                return true;
            }
            if (circle.Contains(p2))
            {
                HitPoint  = p2;
                HitNormal = (HitPoint - circle.center).normalized;
                return true;
            }
            if (circle.Contains(p3))
            {
                HitPoint  = p3;
                HitNormal = (HitPoint - circle.center).normalized;
                return true;
            }

            if (Contains(p1, p2, p3, circle.center))
            {
                HitPoint = circle.center;
                return true;
            }

            return IntersectsLine(circle, p1, p2) || IntersectsLine(circle, p2, p3) || IntersectsLine(circle, p3, p1);
        }


        private static bool IntersectsLine(Circle circle, Vector2 pointA, Vector2 pointB)
        {
            Vector2 toPoint = new Vector2(circle.center.x - pointA.x, circle.center.y - pointA.y);
            Vector2 toB = new Vector2(pointB.x - pointA.x, pointB.y - pointA.y);

            float dot = Vector2.Dot(toPoint, toB);
            float distOnLine = dot / toB.sqrMagnitude;

            if (distOnLine < 0 || distOnLine > 1)
                return false;

            Vector2 pointOnLine = new Vector2(pointA.x + toB.x * distOnLine, pointA.y + toB.y * distOnLine);
            HitPoint = pointOnLine;
            HitNormal = (HitPoint - circle.center).normalized;
            return new Vector2(circle.center.x - pointOnLine.x, circle.center.y - pointOnLine.y).sqrMagnitude <= circle.radius * circle.radius;
        }
        
        
    //  From Triangulator  //
        public static bool Contains(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 p)
        {
            //  Based on Barycentric coordinates  //
            float d = (v2.y - v3.y) * (v1.x - v3.x) + (v3.x - v2.x) * (v1.y - v3.y);
            float f1 = p.x - v3.x, 
                  f2 = p.y - v3.y;
    
            float a = ((v2.y - v3.y) * f1 + (v3.x - v2.x) * f2) / d;
            float b = ((v3.y - v1.y) * f1 + (v1.x - v3.x) * f2) / d;
            float c = 1 - a - b;
    
            //  The point is within the triangle  //
            return a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f;
        }
        
        
        public static bool Contains(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 p, out Vector3 b)
        {
            b = BaryWeight(v1, v2, v3, p);
    
            //  The point is within the triangle  //
            return b.x > 0f && b.x < 1f && b.y > 0f && b.y < 1f && b.z > 0f && b.z < 1f;
        }
        
        
        public static bool IsClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return 0 >= p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;
        }
    }
}