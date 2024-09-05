using UnityEngine;


namespace GeoMath
{
    public partial struct Triangle
    {
        private Vector2 a, b, c;

        public Triangle(Vector2 a, Vector2 b, Vector2 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public Triangle(Vector2 center, float size, float angle)
        {
            Vector2 d = (Vector2.up * size).RotRad(angle);
            
            a = center + d;
            
            const float step = 2 / 3f * Mathf.PI;
            b = center + d.RotRad(step);
            c = center + d.RotRad(step * 2);
        }
    }
}