using UnityEngine;


public class OrbitCam : MonoBehaviour
{
    public float turnSpeed, flySpeed;

    private float a, z;
    
    private FloatForce turnForce = new FloatForce(200, 50, 20);
    private FloatForce flyForce = new FloatForce(200, 50, 20);

    private Transform trans, cam;
    private void Start()
    {
        trans = transform;
        cam = trans.GetChild(0);
        z = cam.localPosition.z;
        flyForce.SetValue(z);
    }

    private void Update()
    {
        a += Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
        trans.rotation = Quaternion.AngleAxis(turnForce.Update(-a, Time.deltaTime), Vector3.up);

        z += Input.GetAxis("Vertical") * flySpeed * Time.deltaTime;
        cam.localPosition = Vector3.forward * flyForce.Update(z, Time.deltaTime);
    }
}
