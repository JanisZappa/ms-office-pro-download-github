using UnityEngine;


namespace GeoMath
{
    public partial struct Box
    {
        private Vector2 a, b, c, d;

        public Box(Vector2 p, float angle, Vector2 size)
        {
            size *= .5f;
            Vector2 v  = size.Rot(angle);
            Vector2 v2 = size.MultiX(-1).Rot(angle);
            a = p + v;
            b = p + v2;
            c = p - v;
            d = p - v2;
        }
    }
}