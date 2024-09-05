using UnityEngine;


namespace GeoMath
{
    public class Quad
    {
        public Vector2 TR, BR, BL, TL;

        public Vector2 this[int index]
        {
            get
            {
                switch (index)
                {
                    default: return TR;
                    case 1:  return BR;
                    case 2:  return BL;
                    case 3:  return TL;
                }
            }
            set
            {
                switch (index)
                {
                    default: TR = value;    break;
                    case 1:  BR = value;    break;
                    case 2:  BL = value;    break;
                    case 3:  TL = value;    break;
                }
            }
        }

        public  Bounds2D bounds;
        private Bounds2D containedBounds;

        public void SetRect(Vector2 pos, Vector2 dimensions, float angle)
        {
            TR = pos + new Vector2(dimensions.x * .5f, dimensions.y * .5f).Rot(angle);
            BR = pos + new Vector2(dimensions.x * .5f, dimensions.y * -.5f).Rot(angle);
            BL = pos + new Vector2(dimensions.x * -.5f, dimensions.y * -.5f).Rot(angle);
            TL = pos + new Vector2(dimensions.x * -.5f, dimensions.y * .5f).Rot(angle);
            CalcBounds();
        }


        public void CalcBounds()
        {
            bounds = new Bounds2D(TR).Add(BR).Add(BL).Add(TL);

            float maxX = Mathf.Min(TR.x, BR.x);
            float minX = Mathf.Max(BL.x, TL.x);
            float maxY = Mathf.Min(TL.y, TR.y);
            float minY = Mathf.Max(BR.y, BL.y);

            containedBounds = new Bounds2D(new Vector2(maxX, maxY)).Add(new Vector2(maxX, minY))
                           .Add(new Vector2(minX, minY)).Add(new Vector2(minX, maxY));
        }


        public bool Contains(Vector2 point)
        {
            return Tri.Contains(TR, BR, BL, point) || Tri.Contains(BL, TL, TR, point);
        }


        public bool Intersects(Bounds2D checkBounds)
        {
            if (!bounds.Intersects(checkBounds))
                return false;

            if (containedBounds.Intersects(checkBounds))
                return true;

            if (Contains(checkBounds.TR) || Contains(checkBounds.BR) ||
                Contains(checkBounds.BL) || Contains(checkBounds.TL) ||
                checkBounds.Contains(TR) || checkBounds.Contains(BR) || 
                checkBounds.Contains(BL) || checkBounds.Contains(TL))
                return true;
            
            return ShapeCollision.Intersects(this, checkBounds);
        }


        public bool Intersects(Circle circle)
        {
            Bounds2D b = new Bounds2D(circle.center).Pad(circle.radius);
            
            if (!bounds.Intersects(b))
                return false;

            if (containedBounds.Intersects(b))
                return true;

            return Tri.Contains(TR, BR, BL, circle) || Tri.Contains(BL, TL, TR, circle);
        }


        public bool Intersects(Quad otherQuad)
        {
            return ShapeCollision.Intersects(this, otherQuad);
        }


        public void Extrude(float extrude)
        {
            Vector2 dir1 = new Vector2(BR.x - TR.x, BR.y - TR.y).normalized;
            Vector2 dir2 = new Vector2(BL.x - BR.x, BL.y - BR.y).normalized;
            Vector2 dir3 = new Vector2(TL.x - BL.x, TL.y - BL.y).normalized;
            Vector2 dir4 = new Vector2(TR.x - TL.x, TR.y - TL.y).normalized;

            TR = GetExtrudePoint(TR, dir4, dir1, extrude);
            BR = GetExtrudePoint(BR, dir1, dir2, extrude);
            BL = GetExtrudePoint(BL, dir2, dir3, extrude);
            TL = GetExtrudePoint(TL, dir3, dir4, extrude);

            CalcBounds();
        }

        
        private static Vector2 GetExtrudePoint(Vector2 point, Vector2 dir1, Vector2 dir2, float extrude)
        {
            Vector2 nP1 = point + new Vector2(-dir1.y, dir1.x) * extrude;
            Vector2 nP2 = point + new Vector2(-dir2.y, dir2.x) * extrude;
            float halfBetween = (nP2 - nP1).magnitude * .5f;

            float angle = Mathf.Acos(halfBetween / extrude);
            float distance = halfBetween / Mathf.Sin(angle);
            return nP1 + dir1 * distance;
        }
    }
}
