using UnityEngine;


public class MountainCam : MonoBehaviour
{
    public Transform trans2;
    private Vector3 vel, velGoal;
    
    private readonly QuaternionForce rot = new QuaternionForce(300).SetSpeed(45 * 3.65f).SetDamp(31);
    private readonly QuaternionForce rot2 = new QuaternionForce(300).SetSpeed(250 * 1.75f).SetDamp(89);
    private readonly Vector3Force pos = new Vector3Force(300).SetSpeed(150 * 1.7f).SetDamp(31);
    private readonly Vector3Force pos2 = new Vector3Force(300).SetSpeed(50 * 1.75f).SetDamp(14);
    private Transform trans;

    private Vector3 oldPos;
    
    
    private void Start()
    {
        trans = transform;
        vel = Vector3.forward;
        trans.parent = null;

        oldPos = trans2.position;
        pos.SetValue(oldPos);
        pos2.SetValue(oldPos);
    }

    
    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        
        Vector3 newPos = pos2.Update(pos.Update(trans2.position, dt), dt);

        trans.position = newPos;
        
        Vector3 newVel = (newPos - oldPos).SetY(0) / dt;
        if (newVel.sqrMagnitude > .0001f)
            velGoal = newVel.normalized;

        vel = Vector3.Slerp(vel, velGoal, dt * 4);
        oldPos = newPos;

        

        //vel = rB.transform.forward;
        Quaternion skiRot = trans.GetChild(1).rotation;
        Quaternion velRot = Quaternion.LookRotation(vel.normalized * 1.25f + skiRot * Vector3.forward, Vector3.up);
        trans.GetChild(0).rotation = rot.Update(velRot, dt);
        //trans.GetChild(0).rotation = rot.Update(velRot, dt);
    }
}
