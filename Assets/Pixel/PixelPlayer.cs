using ECM.Components;
using UnityEngine;


public class PixelPlayer : CharacterController
{
    [Header("Movement")]
    public float speed;
    public float turn;

    private Transform trans;
    private readonly Vector3Force posForce = new Vector3Force(300).SetSpeed(180).SetDamp(28);
    private readonly FloatForce angleForce = new FloatForce(300).SetSpeed(450).SetDamp(60);
    private readonly FloatForce lookForce = new FloatForce(300).SetSpeed(450).SetDamp(60);
    private float look;

    private Transform head;
    
    private void Start()
    {
        Cursor.visible = false;
        trans = transform;
        movement.capsuleCollider.radius = .3f;
        movement.GetComponent<GroundDetection>().groundLimit = 60;
        
        posForce.SetValue(movement.transform.position);

        head = GetComponentInChildren<Camera>().transform;
    }

    private void LateUpdate()
    {
        trans.position = posForce.Update(movement.transform.position, Time.deltaTime);
        trans.rotation = Quaternion.AngleAxis(angleForce.Update(angle, Time.deltaTime), Vector3.up);
        head.localRotation = Quaternion.AngleAxis(lookForce.Update(look, Time.deltaTime), Vector3.right);
    }

    public void FixedUpdate()
    {
        float turnAxis   = PixelInput.H2, 
              walkAxis   = PixelInput.V1,
              strafeAxis = PixelInput.H1,
              lookAxis   = Input.GetAxis("Mouse Y") * .765f;

        angle += turnAxis * turn * Time.fixedDeltaTime;
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);

        look = Mathf.Clamp(look + lookAxis * Time.fixedDeltaTime * turn * -1f, -90, 90);

        Vector3 move = rot * new Vector3(strafeAxis, 0, walkAxis).Clamp(1) * speed * (1f + Vector3.Dot(trans.forward, movement.groundNormal) * .25f);
        
        movement.Move(move, speed);
    }
}


