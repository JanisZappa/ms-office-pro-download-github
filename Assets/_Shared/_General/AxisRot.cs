using UnityEngine;


public class AxisRot : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed;
    public Space space;
    
    
    private void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime, space);
    }
}
