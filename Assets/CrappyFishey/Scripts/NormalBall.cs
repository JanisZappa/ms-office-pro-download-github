using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBall : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mF = GetComponent<MeshFilter>();
        Mesh m = Instantiate(mF.mesh);
        Vector3[] normals = m.normals;
        for (int i = 0; i < normals.Length; i++)
            normals[i] *= .1f;
        m.normals = normals;
        mF.mesh = m;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
