using UnityEngine;


namespace GeoMath
{
    public static class GJK
    {
        private static readonly Vector2[] simplex = new Vector2[100];
        private static int count;
        public static int maxCount;


        public static bool Intersection(IGJK s1, IGJK s2)
        {
            if(!s1.GetBounds.Intersects(s2.GetBounds))
                return false;
            
            Vector2 d = (s2.Center - s1.Center).normalized;

            simplex[0] = s1.GetSupport(d) - s2.GetSupport(-d);
            count = 1;

            d = -simplex[0].normalized;

            while (true)
            {
                Vector2 A = s1.GetSupport(d) - s2.GetSupport(-d);
                if (Vector2.Dot(A, d) < 0)
                    return false;

                simplex[count++] = A;

                if (count == 2 ? LineCase(ref d) : TriangleCase(ref d))
                    return true;
            }
        }
        
        
        public static GJKHit IntersectionHit(IGJK s1, IGJK s2)
        {
            Vector2 d = (s2.Center - s1.Center).normalized;

            simplex[0] = s1.GetSupport(d) - s2.GetSupport(-d);
            count = 1;

            d = -simplex[0].normalized;

            while (true)
            {
                Vector2 A = s1.GetSupport(d) - s2.GetSupport(-d);
                if (Vector2.Dot(A, d) < 0)
                    return GJKHit.None;

                simplex[count++] = A;

                if (count == 2 ? LineCase(ref d) : TriangleCase(ref d))
                    return GetHit(s1, s2);
            }
        }


        private static GJKHit GetHit(IGJK s1, IGJK s2)
        {
            int minIndex = 0;
            float minDistance = float.MaxValue;
            Vector2 minNormal = Vector3.zero;
            
            while (true)
            {
                for (int i = 0; i < count; i++)
                {
                    int j = (i + 1) % count;

                    Vector2 vertexI = simplex[i];
                    Vector2 vertexJ = simplex[j];

                    Vector2 ij = vertexJ - vertexI;

                    Vector2 normal = new Vector2(ij.y, -ij.x).normalized;
                    float distance = Vector2.Dot(vertexI, normal);

                    if (distance < 0)
                    {
                        distance *= -1;
                        normal *= -1;
                    }

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minNormal = normal;
                        minIndex = j;
                    }
                }

                Vector2 support = s1.GetSupport(minNormal) - s2.GetSupport(-minNormal);
                float sDistance = Vector2.Dot(support, minNormal);

                if(Mathf.Abs(sDistance - minDistance) > 0.001f) 
                {
                    minDistance = float.MaxValue;
                    
                //  Add it into simplex
                    if(minIndex == 0)
                        simplex[count] = support;
                    else
                    {
                        for (int i = count; i > minIndex; i--)
                            simplex[i] = simplex[i - 1];
                        
                        simplex[minIndex] = support;
                    }
                    
                    count++;
                }
                
                if(count == 100 | minDistance < float.MaxValue)
                    break;
            }

            maxCount = Mathf.Max(maxCount, count);

            return new GJKHit(true, s2.GetSupport(-minNormal), -minNormal, minDistance);
        }


        private static bool LineCase(ref Vector2 d)
        {
            Vector2 B = simplex[0], A = simplex[1];
            Vector2 AB = B - A, AO = -A;

            Vector2 ABperp = TripleProd(AB, AO, AB);
            d = ABperp.normalized;

            return false;
        }


        private static bool TriangleCase(ref Vector2 d)
        {
            Vector2 C = simplex[0], B = simplex[1], A = simplex[2];
            Vector2 AB = B - A, AC = C - A, AO = -A;

            Vector2 ABperp = TripleProd(AC, AB, AB);

            if (Vector2.Dot(ABperp, AO) > 0)
            {
                simplex[0] = simplex[1];
                simplex[1] = simplex[2];
                count = 2;
                d = ABperp.normalized;
                return false;
            }

            Vector2 ACperp = TripleProd(AB, AC, AC);
            if (Vector2.Dot(ACperp, AO) > 0)
            {
                simplex[1] = simplex[2];
                count = 2;
                d = ACperp.normalized;
                return false;
            }

            return true;
        }


        private static Vector2 TripleProd(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(Vector3.Cross(a, b), c);
        }
    }


    public struct GJKHit
    {
        public readonly bool hit;
        public readonly Vector2 pos, normal;
        public readonly float depth;

        public GJKHit(bool hit, Vector2 pos, Vector2 normal, float depth)
        {
            this.hit = hit;
            this.pos = pos;
            this.normal = normal;
            this.depth = depth;
        }
        
        public static readonly GJKHit None = new GJKHit(false, Vector2.zero, Vector2.zero, 0);

        public void DrawNormal()
        {
            DRAW.Vector(pos, normal * .2f).SetColor(COLOR.red.tomato);
        }
        
        public void DrawNormal(Vector2 shift)
        {
            DRAW.Vector(pos + shift, normal * .2f).SetColor(COLOR.red.tomato);
        }
    }
}