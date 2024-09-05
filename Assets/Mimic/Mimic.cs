using UnityEngine;


public class Mimic : MonoBehaviour
{
    public Transform goal;
    private Vector3 pos;
    
    public float frequency;
    public float damping;
    
    private float kp, kd;
    
    
    private Rigidbody rB;


    private void Start()
    {
        rB = GetComponent<Rigidbody>();
        pos = goal.position;
    }


    private void FixedUpdate()
    {
        CalcuateKPKD();
        
        Vector3 oldP = pos;
        Vector3 newP = goal.position;
        pos = newP;
        
        Vector3 vel = (newP - oldP) / Time.fixedDeltaTime;
        UpdatePos(newP, vel);
        UpdateRotation(goal.rotation);
    }


    private void CalcuateKPKD()
    {
        kp = (6f*frequency)*(6f*frequency)* 0.25f;
        kd = 4.5f*frequency*damping;
    }


    private void UpdatePos(Vector3 Pdes, Vector3 Vdes)
    {
        float dt = Time.fixedDeltaTime;
        float g = 1 / (1 + kd * dt + kp * dt * dt);
        float ksg = kp * g;
        float kdg = (kd + kp * dt) * g;
        Vector3 Pt0 = transform.position;
        Vector3 Vt0 = rB.velocity;
        
        Vector3 dir = Pdes - Pt0;
        Vector3 F = dir * ksg + (Vdes - Vt0) * kdg;
        rB.AddForce (F);
    }


    private void UpdateRotation(Quaternion desiredRotation)
    {
        float dt = Time.fixedDeltaTime;
        float g = 1 / (1 + kd * dt + kp * dt * dt);
        float ksg = kp * g;
        float kdg = (kd + kp * dt) * g;
        Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
// Q can be the-long-rotation-around-the-sphere eg. 350 degrees
// We want the equivalant short rotation eg. -10 degrees
// Check if rotation is greater than 190 degees == q.w is negative
        if (q.w < 0)
        {
            // Convert the quaterion to eqivalent "short way around" quaterion
            q.x = -q.x;
            q.y = -q.y;
            q.z = -q.z;
            q.w = -q.w;
        }
        q.ToAngleAxis (out var xMag, out var x);
        x.Normalize ();
        x *= Mathf.Deg2Rad;
        Vector3 pidv = kp * x * xMag - kd * rB.angularVelocity;
        Quaternion rotInertia2World = rB.inertiaTensorRotation * transform.rotation;
        pidv = Quaternion.Inverse(rotInertia2World) * pidv;
        pidv.Scale(rB.inertiaTensor);
        pidv = rotInertia2World * pidv;
        rB.AddTorque (pidv);
    }
}
