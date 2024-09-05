using UnityEngine;


public class AxisSway : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed, range, pow;
    
    [Space]
    public float offset;
    
    private float t;


    private void Start()
    {
        t = Random.Range(0, Mathf.PI * 2);
    }


    private void Update()
    {
        t += Time.deltaTime * speed;
        float s = Mathf.Sin(t);
        s = (1f - Mathf.Pow(1f - Mathf.Abs(s), pow)) * Mathf.Sign(s);
        transform.localRotation = Quaternion.AngleAxis(s * range + offset, axis);
    }
}
