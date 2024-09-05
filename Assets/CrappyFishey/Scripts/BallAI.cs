using UnityEngine;


public class BallAI : MonoBehaviour
{
    private Vector3 dir;
    private Rigidbody rB;
    
    
    private void Start()
    {
        rB = GetComponent<Rigidbody>();
        dir = Random.onUnitSphere;
    }


    private void FixedUpdate()
    {
        dir = Quaternion.AngleAxis(Random.Range(-360f, 360f), Random.onUnitSphere) * dir;
        rB.AddForce(dir * 15 + Vector3.down * .5f);
    }
}
