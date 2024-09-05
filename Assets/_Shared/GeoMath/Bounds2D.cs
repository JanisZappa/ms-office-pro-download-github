using UnityEngine;


namespace GeoMath
{
    [System.Serializable]
    public partial struct Bounds2D
    {
        public float minX, maxX, minY, maxY;

        public Vector2 Center { get { return new Vector2(minX + (maxX - minX) * .5f, minY + (maxY - minY) * .5f); } }

        public Vector2 Size { get { return new Vector2(maxX - minX, maxY - minY); }}
        
        public Vector2 TR   { get { return new Vector2(maxX, maxY); } }
        public Vector2 BR   { get { return new Vector2(maxX, minY); } }
        public Vector2 BL   { get { return new Vector2(minX, minY); } }
        public Vector2 TL   { get { return new Vector2(minX, maxY); } }

        public float MaxSide { get { return Mathf.Max(maxX - minX,  maxY - minY); } }
        public float Aspect  { get { return (BR.x - BL.x) / (TL.y - BL.y); } }
        public float Area    { get { return (BR.x - BL.x) * (TL.y - BL.y); } }

        
    //  Constructor  //
        public Bounds2D (Vector2 point)
        {
            minX = maxX = point.x;
            minY = maxY = point.y;
        }

        public Bounds2D(float minX, float maxX, float minY, float maxY)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minY = minY;
            this.maxY = maxY;
        }

        public static Bounds2D GetViaString(string value)
        {
            string[] parts = value.Split('|');
            string[] cParts = parts[0].Replace("(", "").Replace(")", "").Split(',');
            string[] sParts = parts[1].Replace("(", "").Replace(")", "").Split(',');
            
            Vector2 c = new Vector2(float.Parse(cParts[0]), float.Parse(cParts[1]));
            Vector2 s = new Vector2(float.Parse(sParts[0]), float.Parse(sParts[1]));
            
            return new Bounds2D(c - s * .5f).Add(c + s * .5f);
        }
        

    //  Alter Bounds Shape  //
        public Bounds2D Add(Vector2 pos)
        {
           return new Bounds2D(Mathf.Min(minX, pos.x),
                               Mathf.Max(maxX, pos.x),
                               Mathf.Min(minY, pos.y),
                               Mathf.Max(maxY, pos.y));
        }
        
        public Bounds2D Add(Bounds2D otherBound)
        {
            return new Bounds2D(Mathf.Min(minX, otherBound.minX),
                                Mathf.Max(maxX, otherBound.maxX),
                                Mathf.Min(minY, otherBound.minY),
                                Mathf.Max(maxY, otherBound.maxY));
        }
        
        public Bounds2D Pad(float by)
        {
            return new Bounds2D(minX - by, maxX + by,
                                minY - by, maxY + by);
        }
        
        public Bounds2D Clamp(float minX, float maxX, float minY, float maxY)
        {
            return new Bounds2D(Mathf.Max(this.minX, minX), 
                                Mathf.Min(this.maxX, maxX),
                                Mathf.Max(this.minY, minY),
                                Mathf.Min(this.maxY, maxY));
        }

        public Bounds2D Fraction(float xMin, float xMax, float yMin, float yMax)
        {
            Vector2 s = Size;
            return new Bounds2D(new Vector2(minX + xMin * s.x, minY + yMin * s.y)).
                            Add(new Vector2(minX + xMax * s.x, minY + yMax * s.y));
        }
        
        public Bounds2D PadDir(Vector2 dir)
        {
            return new Bounds2D(dir.x >= 0? minX : minX + dir.x,
                                dir.x < 0? maxX : maxX + dir.x,
                                dir.y >= 0? minY : minY + dir.y,
                                dir.y < 0? maxY : maxY + dir.y);
        }
        
        
    //  Querys  //
        public bool Intersects(Bounds2D other)
        {
            return minX <= other.maxX && minY <= other.maxY && maxX >= other.minX && maxY >= other.minY;
        }

        public float IntersectLerp(Bounds2D other)
        {
           float x_overlap = Mathf.Max(0, Mathf.Min(maxX, other.maxX) - Mathf.Max(minX, other.minX));
           float y_overlap = Mathf.Max(0, Mathf.Min(maxY, other.maxY) - Mathf.Max(minY, other.minY));
           return (x_overlap * y_overlap) / Area;
        }
        
        public bool XAxisOverlap(Bounds2D other)
        {
            return minX <= other.maxX && maxX >= other.minX;
        }
        
        public bool YAxisOverlap(Bounds2D other)
        {
            return minY <= other.maxY && maxY >= other.minY;
        }
        
        public bool Contains(Bounds2D other)
        {
            return minX <= other.minX && minY <= other.minY && maxX >= other.maxX && maxY >= other.maxY;
        }

        public bool Contains(Vector2 point)
        {
            return point.x > minX && point.x < maxX && point.y > minY && point.y < maxY;
        }

        public Vector2 GetUV(Vector2 point)
        {
            return new Vector2((point.x - minX) / (maxX - minX), (point.y - minY) / (maxY - minY));
        }
        
        
        public float OverlapArea(Bounds2D other)
        {
            if (Intersects(other))
            {
                float x = Mathf.Min(maxX, other.maxX) - Mathf.Max(minX, other.minX);
                float y = Mathf.Min(maxY, other.maxY) - Mathf.Max(minY, other.minY);
                
                return x * y;
            }
            
            return 0;
        } 
        
        public float DistanceSqr(Vector2 point)
        {
            float x = 0;
            if (point.x < minX)
                x = minX - point.x;
            if (point.x > maxX)
                x = point.x - maxX;
            
            float y = 0;
            if (point.y < minY)
                y = minY - point.y;
            if (point.y > maxY)
                y = point.y - maxY;

            return x * x + y * y;
        }

        public float DistanceSqr(Bounds2D other)
        {
            float x = 0;
            if (other.maxX < minX)
                x = minX - other.maxX;
            if (other.minX > maxX)
                x = other.minX - maxX;

            float y = 0;
            if (other.maxY < minY)
                y = minY - other.maxY;
            if (other.minY > maxY)
                y = other.minY - maxY;
            
            return x * x + y * y;
        }
        
        public float DistanceSqr(Vector2 p1, Vector2 p2)
        {
            if (Contains(p1) || Contains(p2))
                return 0;

            Line line = new Line(p1, p2);

            return Mathf.Min(
                Mathf.Min(new Line(TL, TR).LineDistSqr(line), new Line(TR, BR).LineDistSqr(line)),
                Mathf.Min(new Line(BR, BL).LineDistSqr(line), new Line(BL, TL).LineDistSqr(line))
            );
        }
        
        public float InsideDistance(Bounds2D other)
        {
            float dist = maxX - other.maxX;
                  dist = Mathf.Min(dist, maxY - other.maxY);
                  dist = Mathf.Min(dist, other.minX - minX);
            
            return Mathf.Min(dist, other.minY - minY);
        }

        public bool ContainsLine(Vector2 p1, Vector2 p2)
        {
            if (Contains(p1) || Contains(p2))
                return true;

            Line line = new Line(p1, p2);
           
            return
            new Line(TL, TR).Contact(line) || 
            new Line(TR, BR).Contact(line) || 
            new Line(BR, BL).Contact(line) || 
            new Line(BL, TL).Contact(line);
        }

        public bool BorderIntersect(Quad quad)
        {
            return ContainsLine(quad.TL, quad.TR)
                || ContainsLine(quad.TR, quad.BR)
                || ContainsLine(quad.BR, quad.BL)
                || ContainsLine(quad.BL, quad.TL);
        }

        public string GetString()
        {
            return Center.ToString("F4") + "|" + Size.ToString("F4");
        }
        
        public bool SharedCorner(Bounds2D other, ref Vector2 corner)
        {
            if (TR == other.TL || TR == other.BR || TR == other.BL)
            {
                corner = TR;
                return true;
            }
            if (TL == other.TR || TL == other.BR || TL == other.BL)
            {
                corner = TL;
                return true;
            }
            if (BR == other.BL || BR == other.TR || BR == other.TL)
            {
                corner = BR;
                return true;
            }
            if (BL == other.BR || BL == other.TR || BL == other.TL)
            {
                corner = BL;
                return true;
            }
            
            return false;
        }

        public Bounds2D Move(Vector2 move)
        {
            return new Bounds2D(minX + move.x, maxX + move.x, minY + move.y, maxY + move.y);
        }
    }
}