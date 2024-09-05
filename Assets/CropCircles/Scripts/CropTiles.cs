using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTiles : MonoBehaviour
{
    public float size;
    public int steps;
    public Mesh high;
    public Material mat;
    private Matrix4x4[] mx;
    private bool hide;
    
    void Start()
    {
        Vector3 min = new Vector3((steps - 1) * size, 0, (steps - 1) * size) * -.5f;
        
        mx = new Matrix4x4[steps * steps];
        int i = 0;
        for (int x = 0; x < steps; x++)
        for (int z = 0; z < steps; z++)
            mx[i++] = Matrix4x4.TRS(min + new Vector3(x * size, 0, z * size), Quaternion.AngleAxis(Random.Range(0, 4) * 90, Vector3.up), Vector3.one);
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
            hide = !hide;
        
        if (!hide)
            Graphics.DrawMeshInstanced(high, 0, mat, mx);
    }
}
