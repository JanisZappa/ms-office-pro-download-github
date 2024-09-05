using UnityEngine;


public class AxisForceRot : MonoBehaviour
{
    public float steerSpeed;
    
    public float speed, damp;
    
    private readonly FloatForce force = new FloatForce(200);
    private float angle;
    
    
    private void Start()
    {
        angle = transform.eulerAngles.y;
        force.SetSpeed(speed).SetDamp(damp).SetValue(angle);
    }

    private void Update()
    {
        angle += Time.deltaTime * steerSpeed *
                 ((Input.GetKey(KeyCode.LeftArrow) ? -1 : 0) + (Input.GetKey(KeyCode.RightArrow) ? 1 : 0));
        
        transform.rotation = Quaternion.AngleAxis(force.Update(angle, Time.deltaTime), Vector3.up);
    }


    private void OnValidate()
    {
        force.SetSpeed(speed).SetDamp(damp);
    }
}
