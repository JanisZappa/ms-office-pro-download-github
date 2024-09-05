using UnityEngine;


namespace GeoMath
{
    public partial struct Capsule
    {
        public Vector2 l1, dir;
        public float radius, angle;

        public Capsule(Vector2 pos, float height, float radius, float angle)
        {
            this.angle = angle;
            
            dir = Vector2.up.Rot(angle) * height;
            
            l1 = pos - dir * .5f;
            
            this.radius = radius;
        }
    }
}

