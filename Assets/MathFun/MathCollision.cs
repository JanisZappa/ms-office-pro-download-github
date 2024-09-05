using System.Collections;
using System.Collections.Generic;
using GeoMath;
using UnityEngine;


public class MathCollision : MonoBehaviour
{
    public class ShapeBody
    {
        public Vector2 pos, mV;
        public float angle, aV;
        
        private int type;
        
        public IGJK shape;
        private bool stuck;

        
        public ShapeBody(Vector2 pos, float angle, int type)
        {
            this.pos   = pos;
            this.angle = angle;
            this.type  = type;
            stuck = false;
            
            shape = UpdateShape();
            
            aV = Random.Range(-Mathf.PI, Mathf.PI);
        }
        
        
        public ShapeBody(IGJK shape)
        {
            this.shape = shape;
            stuck = true;
        }

        
        public IGJK Update(float dt)
        {
            if (!stuck)
            {
                mV *= 1f - dt;
                mV += Vector2.down * 10 * dt;
                pos += mV * dt;
            
                aV *= 1f - dt;;
                angle += aV * dt;
            
                shape = UpdateShape();
            }
            
            return shape;
        }


        private IGJK UpdateShape()
        {
            switch (type)
            {
                default:   return new Triangle(pos, 1, angle);
                
                case 1:    return new Circle(pos, 1);
                
                case 2:    return new Box(pos, angle * Mathf.Rad2Deg, new Vector2(.5f, 1));
                
                case 3:    return new Capsule(pos, 1, .25f, angle * Mathf.Rad2Deg);
            }
        }
    }
    
    
    private ShapeBody body, floor;
    private Stepper stepper;


    private void Start()
    {
        body = new ShapeBody(new Vector2(0, 5), Random.Range(0, 2 * Mathf.PI), Random.Range(0, 4));
        floor = new ShapeBody(new Box(new Vector2(0, -4), 0, new Vector2(14, 1)));
        
        stepper = new Stepper(300, Step);
    }


    private void Update()
    {
        stepper.Update(Time.deltaTime);
        
        body.shape.Draw(COLOR.red.tomato);
        floor.shape.Draw(COLOR.yellow.fresh);
    }


    private void Step(float dt)
    {
        body.Update(dt);
        floor.Update(dt);
        Solve(body, floor, dt);
    }

    private void Solve(ShapeBody s1, ShapeBody s2, float dt)
    {
        GJKHit hit = GJK.IntersectionHit(s2.shape, s1.shape);
        if(!hit.hit)
            return;
        
        //    Resolve Collision    //
        float currentRadSpin = s1.aV;

        Vector2 charPos      = s1.pos;
        Vector2 charVelocity = s1.mV;

        Vector2 charRadiusVector = hit.pos - charPos;
        Vector2 charAngularVelocity = currentRadSpin.Cross(charRadiusVector);


        Vector2 charVelAtContact = charVelocity + charAngularVelocity;


        Vector2 stickPos = s2.pos;
        Vector2 stickVel = s2.mV;

        Vector2 stickRadiusVector    = hit.pos - stickPos;
        Vector2 stickAngularVelocity = (0f /*stickSpin*/).Cross(stickRadiusVector);

        Vector2 stickVelAtContact = stickVel + stickAngularVelocity;


        Vector2 relativeVelocity = charVelAtContact - stickVelAtContact;


        float velAlongNormal = Vector2.Dot(relativeVelocity, hit.normal);

        float hitDirCross = charRadiusVector.Cross(hit.normal);
        
        float e = .5f;
        float mass = 1;
        float intertia = 2.5f;
        float impulse = -(1.0f + e) * velAlongNormal / 
                        ( 
                            1.0f / mass + 1.0f / float.MaxValue 
                                        + Mth.IntPow(hitDirCross, 2) / intertia 
                            /* + Mathf.Pow(Extensions.CrossProduct(stickRadiusVector, Tri.HitNormalInverse), 2) / inertia */  //Stick inertia is infinite
                        );

        Vector2 impulseV = hit.normal * impulse;

        s1.pos += -hit.normal * hit.depth;
        s1.mV = charVelocity + impulseV / mass;
        //s1.mV += -hit.normal * hit.depth * (.1f / dt);
        s1.aV = (currentRadSpin + hitDirCross / intertia * impulse);
    }
}
