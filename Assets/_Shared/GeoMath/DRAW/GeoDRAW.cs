using UnityEngine;
using Shape = DRAW.Shape;
    
namespace GeoMath
{
    public interface IDRAW
    {
        Shape Draw(Color color);
    }
    
    public partial struct Box : IDRAW
    {
        public Shape Draw(Color color)
        {
            return Shape.Get(5).Set(0, a).Set(1, b).Set(2, c).Set(3, d).Set(4, a).SetColor(color);
        }
    }


    public partial struct Circle : IDRAW
    {
        public Shape Draw(Color color)
        {
            return DRAW.Circle(center, radius, 100).SetColor(color);
        }
    }
    
    
    public partial struct Line : IDRAW
    {
        public Shape Draw(Color color)
        {
            return DRAW.Vector(l1, dir).SetColor(color);
        }
    }
    
    
    public partial struct Triangle : IDRAW
    {
        public Shape Draw(Color color)
        {
            return Shape.Get(4).Set(0, a).Set(1, b).Set(2, c).Set(3, a).SetColor(color);
        }
    }
    
    
    public partial struct Bounds2D : IDRAW
    {
        public Shape Draw(Color color)
        {
            return DRAW.Box(BL, TR).SetColor(color);
        }
    }
    
    
    public partial struct Capsule : IDRAW
    {
        public Shape Draw(Color color)
        {
            const int shellSteps = 12;
            
            Vector2 pointer = dir.Rot90().SetLength(radius);

            Shape shape = Shape.Get(shellSteps * 2 + 1);

            const float aStep = 180f / (shellSteps - 1);
            for (int i = 0; i < shellSteps; i++)
                shape.Set(i, l1 + dir + pointer.Rot(180 + aStep * i));
        
            for (int i = 0; i < shellSteps; i++)
                shape.Set(i + shellSteps, l1 + pointer.Rot(aStep * i));

            shape.Copy(0, shellSteps + shellSteps);
        
            return shape.SetColor(color);
        }
    }
}