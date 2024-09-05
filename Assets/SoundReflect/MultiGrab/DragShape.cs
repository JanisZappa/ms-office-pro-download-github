using UnityEngine;


public class DragShape : MonoBehaviour
{
    private CircleCollider2D coll;
    private Transform trans;
    
    
    private void Start()
    {
        coll = GetComponent<CircleCollider2D>();
        trans = transform;
    }

    
    private void LateUpdate()
    {
        Vector3 root = trans.position;
        DRAW.Circle(root, coll.radius, 40).SetColor(Color.yellow);
        
        Quaternion rot = trans.rotation;
        for (int i = 0; i < 6; i++)
        {
            DRAW.Vector(root, (rot * Quaternion.AngleAxis(i * 60, Vector3.forward)) * Vector3.up * coll.radius).SetColor(Color.yellow);
        }
    }
}
