using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WonderTurn : MonoBehaviour
{
    public float speed;
    public int fps;

    [Space] 
    public float a;
    public float b;
    public float result;

    private float t;

    private Transform trans;
    
    private void Start()
    {
        trans = transform;
    }

    private void Update()
    {
        t += Time.deltaTime * speed;
        a = t / fps;
        b = a % 1;
        a = Mathf.Floor(a);

        result = (a + Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1,b)))) * fps;

        trans.rotation = Quaternion.AngleAxis(result, Vector3.up);
    }
}
