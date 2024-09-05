using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MakeLow : MonoBehaviour
{
    public int res;
    public Mesh high, low, superlow;
    public float range, range2;
    public Material mat;

    private bool hide;

    private Matrix4x4[] mx;
    
    private void Start()
    {
        MeshFilter[] r = GetComponentsInChildren<MeshFilter>();
        int max = Mathf.Min(1023, r.Length);
        mx = new Matrix4x4[max];
        for (int i = 0; i < r.Length; i++)
        {
            MeshFilter mf = r[i];
            float dist = mf.transform.position.magnitude;
            if (dist > range && false)
                mf.mesh = dist > range2? superlow : low;
            
            mf.transform.localRotation = Quaternion.AngleAxis(Random.Range(0, 4) * 90, Vector3.up);
            
            if(i < max)
                mx[i] = Matrix4x4.TRS(mf.transform.position, Quaternion.AngleAxis(Random.Range(0, 4) * 90, Vector3.up), Vector3.one);
            Destroy(mf.gameObject);
        }
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
            hide = !hide;
        

        if (!hide)
        {
            Graphics.DrawMeshInstanced(high, 0, mat, mx);
        }
    }
}
