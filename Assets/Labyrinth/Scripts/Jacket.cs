using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jacket : MonoBehaviour
{
    private Transform guide;

    private Transform trans;
    
    private QuaternionForce jacketF = new QuaternionForce(200).SetSpeed(300).SetDamp(14f);
    private FloatForce jacketU = new FloatForce(200).SetSpeed(1400).SetDamp(34f);
    private FloatForce hatdown = new FloatForce(200).SetSpeed(400).SetDamp(34f);
    private LabyrinthHat hat;
    void Start()
    {
        
        trans = transform;
        guide = trans.parent;
        hat = guide.GetComponentInChildren<LabyrinthHat>();
        trans.SetParent(null);
        Quaternion r = guide.rotation;
        Quaternion r2 = Quaternion.LookRotation((r * Vector3.forward).MultiY(0).normalized, Vector3.up);
        jacketF.SetValue(Quaternion.Slerp(r, r2, .5f));
        jacketU.SetValue(guide.position.y);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 g = guide.position;
        g = g.SetY(jacketU.Update(g.y + hatdown.Update(hat.down ? .17f : 0, Time.deltaTime), Time.deltaTime) * .15f + .85f * g.y);
        trans.position = g;
        Quaternion r = guide.rotation;
        Quaternion r2 = Quaternion.LookRotation((r * Vector3.forward).MultiY(0).normalized, Vector3.up);
        trans.rotation = jacketF.Update(Quaternion.Slerp(r, r2, .5f), Time.deltaTime);
    }
}
