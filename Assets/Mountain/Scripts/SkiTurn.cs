using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkiTurn : MonoBehaviour
{
    private Rigidbody rB;
    // Start is called before the first frame update
    void Start()
    {
        rB = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 vel = rB.velocity;
        //if(Physics.Raycast(new Ray(transform.position, Vector3.down), 1.))
        float a = ((Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0)) * Time.fixedDeltaTime *
                  vel.magnitude * .125f;
        Quaternion r = Quaternion.AngleAxis(a, Vector3.up);
        rB.velocity = r * vel;
        //rB.AddTorque(0, a * 100, 0);
    }
}
