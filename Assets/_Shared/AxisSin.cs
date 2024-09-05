using UnityEngine;


public class AxisSin : MonoBehaviour
{
    public Vector3 axis;
    public float speed;
    public float range;
    
    public bool local;
    
    private Transform trans;
    private Vector3 p;
    private float t;
    
    
    private void Start()
    {
        trans = transform;
        p = local? trans.localPosition : trans.position;
        t = Random.Range(0, Mathf.PI * 2);
    }

    
    private void Update()
    {
        t += Time.deltaTime * speed;
        
        Vector3 offset = axis * Mathf.Sin(t) * range;
        if(local)
            trans.localPosition = p + offset;
        else
            trans.position = p + offset;
    }
}
