using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandCam : MonoBehaviour
{
    public float speed;
    public float dist;
    public float height;
    
    SpinUpTest.RotAxisStack stack = new SpinUpTest.RotAxisStack(8, 243);
    private float time;
    
    private Vector3Force pForce = new Vector3Force(200).SetSpeed(6).SetDamp(5);
    private QuaternionForce qForce = new QuaternionForce(200).SetSpeed(2).SetDamp(5);

    private Vector3 GetPos(float t, float multi = 1)
    {
        Vector2 v = stack.Update(t, 1) * dist;
        Vector3 r = v.V3YToZ().SetY(100);
        Physics.Raycast(r, Vector3.down, out RaycastHit hit);
        return hit.point + Vector3.up * height * multi;
    }

    private void Start()
    {
        Vector3 p1 = GetPos(time);
        Vector3 p2 = GetPos(time + 1, .5f);
        pForce.SetValue(p1);
        qForce.SetValue(Quaternion.LookRotation(p2 - p1, Vector3.up));
    }
    private void Update()
    {
        time += Time.deltaTime * speed;

        Vector3 p1 = GetPos(time);
        Vector3 p2 = GetPos(time + 1, .5f);
        transform.position = pForce.Update(p1, Time.deltaTime);
        transform.rotation = qForce.Update(Quaternion.LookRotation(p2 - p1, Vector3.up), Time.deltaTime);
    }
}
