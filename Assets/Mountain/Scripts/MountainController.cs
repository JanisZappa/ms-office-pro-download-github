using System;
using ECM.Components;
using UnityEngine;


public class MountainController: CharacterController
{
    [Header("Movement")] 
    public float speed;
    public float drag;

    [Header("SkiAngle")] 
    public float skiAngle;
    public Transform ski;
    public float steer;

    private Vector3 vel;
    private float a;
    private QuaternionForce skiRotForce = new QuaternionForce(300).SetSpeed(700).SetDamp(80);

    private Vector3 startPos;


    public float GetVel
    {
        get { return vel.magnitude; }
    }
    
    
    private void Start()
    {
        movement.useGravity = true;
        movement.gravity = new Vector3(0, -200000, 0);
        movement.capsuleCollider.radius = 4;
        movement.capsuleCollider.height = 48;
        movement.slopeLimit = 90;
        movement.GetComponent<GroundDetection>().groundLimit = 90;

        startPos = transform.position;
    }

    
    private void LateUpdate()
    {
        transform.position = movement.transform.position;
        ski.rotation = skiRotForce.Update(Quaternion.AngleAxis(skiAngle, Vector3.up), Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Backspace))
            movement.transform.position = startPos;
    }


    public void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        
        a = Mathf.Lerp(a, ((Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0)),
            dt * 8);
        
        skiAngle += a * dt * 160;
       
        if (movement.isGrounded)
        {
            Vector3 g = movement.groundNormal.MultiY(0) * speed;
            
            const float grip = .3333f;

            Quaternion rot = Quaternion.AngleAxis(skiAngle, Vector3.up);
            Vector3 f = rot * Vector3.forward;
            Vector3 r = rot * Vector3.right;

            float fV = Vector3.Dot(vel, f);
            float rV = Vector3.Dot(vel, r);

            float fG = Vector3.Dot(g, f);
            float rG = Vector3.Dot(g, r) * grip;

            float newR = rV * (1f - (1f - grip) * dt);
            float s = rV - newR;
            vel = f * (fV + fG + Mathf.Abs(s) * Mathf.Max(0, Mathf.Sign(fV))) + r * ((newR + rG));
            
            vel *= 1f - dt * drag;
        }
        movement.Move(vel * dt, float.MaxValue, false);
    }
}
