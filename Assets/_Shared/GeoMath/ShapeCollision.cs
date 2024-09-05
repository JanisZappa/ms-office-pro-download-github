using UnityEngine;


namespace GeoMath
{
    //https://www.youtube.com/watch?v=Ap5eBYKlGDo
    //https://gamedevelopment.tutsplus.com/tutorials/collision-detection-using-the-separating-axis-theorem--gamedev-169
    public static class ShapeCollision
    {
        public static bool Intersects(Quad quad, Quad quadB)
        {
            return !AxisProjectionsAreSeperate((quad.TR - quad.TL).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quad.BR - quad.TR).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quad.BL - quad.BR).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quad.TL - quad.BL).Rot90(), quad, quadB) &&

                   !AxisProjectionsAreSeperate((quadB.TR - quadB.TL).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quadB.BR - quadB.TR).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quadB.BL - quadB.BR).Rot90(), quad, quadB) &&
                   !AxisProjectionsAreSeperate((quadB.TL - quadB.BL).Rot90(), quad, quadB);
        }


        public static bool Intersects(Quad quad, Bounds2D bounds)
        {
            return !AxisProjectionsAreSeperate((quad.TR - quad.TL).Rot90(), quad, bounds) &&
                   !AxisProjectionsAreSeperate((quad.BR - quad.TR).Rot90(), quad, bounds) &&
                   !AxisProjectionsAreSeperate((quad.BL - quad.BR).Rot90(), quad, bounds) &&
                   !AxisProjectionsAreSeperate((quad.TL - quad.BL).Rot90(), quad, bounds) &&
                   
                   !AxisProjectionsAreSeperate(V2.right, quad, bounds) &&
                   !AxisProjectionsAreSeperate(V2.up,    quad, bounds);
        }
        
        

        
        private static bool AxisProjectionsAreSeperate(Vector2 axis, Quad quadA, Quad quadB)
        {
            float minQuadA, maxQuadA, minQuadB, maxQuadB;

            GetProjectionMinMax(axis, quadA, out minQuadA, out maxQuadA);
            GetProjectionMinMax(axis, quadB, out minQuadB, out maxQuadB);

            return minQuadA > maxQuadB || minQuadB > maxQuadA;
        }
        
        
        private static bool AxisProjectionsAreSeperate(Vector2 axis, Quad quadA, Bounds2D bounds)
        {
            float minQuad, maxQuad, minBounds, maxBounds;

            GetProjectionMinMax(axis, quadA, out minQuad, out maxQuad);
            GetProjectionMinMax(axis, bounds, out minBounds, out maxBounds);

            return minQuad > maxBounds || minBounds > maxQuad;
        }
        
        
        
        
        private static void GetProjectionMinMax(Vector2 axis, Quad quad, out float min, out float max)
        {
            float dot = Vector2.Dot(axis, quad.TR);
            min = max = dot;

            dot = Vector2.Dot(axis, quad.BR);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);

            dot = Vector2.Dot(axis, quad.BL);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);

            dot = Vector2.Dot(axis, quad.TL);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);
        }
        
        
        private static void GetProjectionMinMax(Vector2 axis, Bounds2D bounds, out float min, out float max)
        {
            float dot = Vector2.Dot(axis, bounds.TR);
            min = max = dot;

            dot = Vector2.Dot(axis, bounds.BR);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);

            dot = Vector2.Dot(axis, bounds.BL);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);

            dot = Vector2.Dot(axis, bounds.TL);
            max = Mathf.Max(dot, max);
            min = Mathf.Min(dot, min);
        }
    }
}
