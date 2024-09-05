using UnityEngine;


public class Mirror : MonoBehaviour
{
    public Transform t;
    
    private void LateUpdate()
    {
        Vector3 p = t.position;
        p.y -= -16;
        p.y *= -1;
        p.y += -16;
        transform.position = p;
    }
}
