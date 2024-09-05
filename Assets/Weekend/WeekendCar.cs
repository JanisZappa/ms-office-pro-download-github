using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeekendCar : MonoBehaviour
{
    public float RideHeight, RideSpringStrength, RideSpringDamper;
    
    [Space]
    public float UprightSpringStrength;
    public float UprightSpringDamper;
    
    [Space]
    public float speed;
    
    private Rigidbody rB;
    private Transform trans;
    
    private float lookAngle, smoothLookAngle;
    
    private readonly Vector3[] feet = {
        new Vector3(-.15f, 0, -.2f), 
        new Vector3(-.15f, 0,  .2f),
        new Vector3( .15f, 0,  .2f),
        new Vector3( .15f, 0, -.2f)
    };
    
    private void Start()
    {
        rB = GetComponent<Rigidbody>();
        trans = transform;
    }
    

   
    private void FixedUpdate()
    {
        lookAngle += Input.GetAxis("Horizontal") * 240 * Time.fixedDeltaTime;
        smoothLookAngle = Mathf.Lerp(smoothLookAngle, lookAngle, Time.fixedDeltaTime * 9);
        
        
        Vector3 forward = trans.forward.SetY(0).normalized;
        Vector3 right   = trans.right;
        Vector3 up      = trans.up;
        
        Vector3 rayDir = -up;
        
        for (int i = 0; i < 4; i++)
        {
            Vector3 f = trans.TransformPoint(feet[i]);
            if (Physics.Raycast(new Ray(f, rayDir), out RaycastHit rayhit, RideHeight * 2))
            {
                Rigidbody hitBody = rayhit.rigidbody;
                Vector3 otherVel = hitBody != null? hitBody.velocity : Vector3.zero;
                float otherDirVel = Vector3.Dot(otherVel, rayDir);
                
                Vector3 vel = rB.GetPointVelocity(f);
                float rayDirVel = Vector3.Dot(vel, rayDir);
                float relVel = rayDirVel - otherDirVel;
                
                float x = rayhit.distance - RideHeight;
                
                float springForce = x * RideSpringStrength - relVel * RideSpringDamper;
                
                rB.AddForceAtPosition(rayDir * springForce, f);
                
                if(hitBody != null)
                    hitBody.AddForceAtPosition(rayDir * -springForce, f);
                
                Vector3 breakForce = right * Vector3.Dot(vel, right) * -.4f + forward * Vector3.Dot(vel, forward) * -.15f;
                
                Vector2 steer = Vector2.zero;;//new Vector2(Input.GetAxis("Horizontal") * .75f, Input.GetAxis("Vertical"));
                steer = steer.normalized * Mathf.Min(1, steer.magnitude);
                
                breakForce += forward * steer.y * speed;
                breakForce += right * steer.x * speed;
                rB.AddForceAtPosition(breakForce, f);
            }
        }
        
        UpdateUprighForce(Time.fixedDeltaTime);
       
    }


    private void UpdateUprighForce(float dt)
    {
        Quaternion currentRotation = trans.rotation;
        Quaternion wannaBe = Quaternion.Slerp(currentRotation, Quaternion.AngleAxis(smoothLookAngle, Vector3.up), 1);
        Quaternion toGoal = wannaBe * Quaternion.Inverse(currentRotation);

        toGoal.ToAngleAxis(out var rotDegrees, out var rotAxis);
        
        
        rB.AddTorque(rotAxis.normalized * (Mathf.Deg2Rad * rotDegrees * UprightSpringStrength) - rB.angularVelocity * UprightSpringDamper);
    }
}
