using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMotionBlur : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float speed;
    public Space space;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime, space);
    }
}
