using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthHat : MonoBehaviour
{
    private Transform trans, parent;
    private Placement a, b;
    private PlacementForce pF = new PlacementForce(200).SetSpeed(200).SetDamp(30);
    private QuaternionForce hF = new QuaternionForce(200).SetSpeed(360).SetDamp(49);
    public bool down;
    
    void Start()
    {
       trans = transform;
       parent = trans.parent;
       a = trans.GetPlacement(true);
       b = parent.GetChild(trans.GetSiblingIndex() + 1).GetPlacement(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            down = !down;
        
        pF.Update(down? b : a, Time.deltaTime).Apply(trans, true);
        parent.localRotation = hF.Update(Quaternion.AngleAxis(down ? 9 : 0, Vector3.right), Time.deltaTime);
    }
}
