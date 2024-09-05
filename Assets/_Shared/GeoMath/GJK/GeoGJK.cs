using UnityEngine;


namespace GeoMath
{
    public interface IGJK : IDRAW
    {
        Vector2 GetSupport(Vector2 dir);
        Vector2 Center { get; }
        Bounds2D GetBounds { get; }
        IGJK Shift(Vector2 shift);
    }
    
    
    public partial struct Box : IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            float dotA = Vector2.Dot(a, dir);
            float dotB = Vector2.Dot(b, dir);
            float dotC = Vector2.Dot(c, dir);
            float dotD = Vector2.Dot(d, dir);

            if (dotA > dotB)
            {
                if (dotA > dotC)
                    return dotA > dotD? a : d;
                
                return dotC > dotD? c : d;
            }
            
            {
                if (dotB > dotC)
                    return dotB > dotD? b : d;
                
                return dotC > dotD? c : d;
            }
        }

        public Vector2 Center
        {
            get
            {
                return a * .25f + b * .25f + c * .25f + d * .25f;
            }
        }
        
        public Bounds2D GetBounds
        {
            get
            {
                return new Bounds2D(a).Add(b).Add(c).Add(d);
            }
        }

        public IGJK Shift(Vector2 shift)
        {
            return new Box(a + shift, b + shift, c + shift, d + shift);
        }
        
        private Box(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }


    public partial struct Circle : IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            return center + dir * radius;
        }

        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        public Bounds2D GetBounds
        {
            get
            {
                return new Bounds2D(center).Pad(radius);
            }
        }
        
        public IGJK Shift(Vector2 shift)
        {
            return new Circle(center + shift, radius);
        }
    }


    public partial struct Line : IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            Vector2 l2 = l1 + dir;
            float dotA = Vector2.Dot(l1, dir);
            float dotB = Vector2.Dot(l2, dir);
            
            return dotA > dotB? l1 : l2;
        }

        public Vector2 Center
        {
            get
            {
                return l1 + dir * .5f;
            }
        }
        
        public IGJK Shift(Vector2 shift)
        {
            return new Line(l1 + shift, l1 + dir + shift);
        }
    }
    
    
    public partial struct Bounds2D : IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            return dir.x > 0? dir.y > 0? TR : BR : dir.y > 0? TL : BL;
        }
        
        public Bounds2D GetBounds { get { return this; } }
        
        public IGJK Shift(Vector2 shift)
        {
            return new Bounds2D(minX + shift.x, maxX + shift.x, minY + shift.y, maxY + shift.y);
        }
    }
    
    
    public partial struct Triangle: IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            float dotA = Vector2.Dot(a, dir);
            float dotB = Vector2.Dot(b, dir);
            float dotC = Vector2.Dot(c, dir);

            if (dotA > dotB)
                return dotA > dotC? a : c;
            
            return dotB > dotC? b : c;
        }

        public Vector2 Center
        {
            get
            {
                const float multi = 1f / 3;
                return a * multi + b * multi + c * multi;
            }
        }
        
        public Bounds2D GetBounds
        {
            get
            {
                return new Bounds2D(a).Add(b).Add(c);
            }
        }
        
        public IGJK Shift(Vector2 shift)
        {
            return new Triangle(a + shift, b + shift, c + shift);
        }
    }
    
    
    public partial struct Capsule : IGJK
    {
        public Vector2 GetSupport(Vector2 dir)
        {
            Vector2 a = l1 + dir * radius;
            Vector2 b = new Box(l1 + this.dir * .5f, angle, new Vector2(radius * 2, this.dir.magnitude)).GetSupport(dir);
            Vector2 c = l1 + this.dir + dir * radius;
            
            float dotA = Vector2.Dot(a, dir);
            float dotB = Vector2.Dot(b, dir);
            float dotC = Vector2.Dot(c, dir);

            if (dotA > dotB)
                return dotA > dotC? a : c;
            
            return dotB > dotC? b : c;
        }

        public Vector2 Center
        {
            get
            {
                return l1 + dir * .5f;
            }
        }
        
        public Bounds2D GetBounds
        {
            get
            {
                return new Bounds2D(l1).Add(l1 + dir).Pad(radius);
            }
        }
        
        public IGJK Shift(Vector2 shift)
        {
            return new Capsule(l1 + shift, dir, radius, angle);
        }

        private Capsule(Vector2 l1, Vector2 dir, float radius, float angle)
        {
            this.l1 = l1;
            this.dir = dir;
            this.radius = radius;
            this.angle = angle;
        }
    }
}
