using UnityEngine;


public class FrameTest : MonoBehaviour
{
    private void Start()
    {
        Mesh     mesh = GetComponent<MeshFilter>().mesh;
        Material mat  = GetComponent<MeshRenderer>().material;
        
        for (int i = 0; i < 29; i++)
        {
            GameObject gO = new GameObject();
            gO.transform.SetParent(transform, false);
            gO.transform.localRotation = Quaternion.AngleAxis((i + 1) * -1.5f * .25f, Vector3.forward);
            gO.transform.localPosition = Vector3.back * (i + 1) * .1f;
            
            /*
            Mesh meshCopy = Instantiate(mesh);
            
            Vector2[] uv2 = meshCopy.uv2;
            int uvCount = uv2.Length;
            int frame   = i + 1;
            
            for (int e = 0; e < uvCount; e++)
                uv2[e].y = frame;
            
            meshCopy.SetUVs(1, uv2);
            */
            
            MeshRenderer mR = gO.AddComponent<MeshRenderer>();
            mR.material = mat;
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            mR.GetPropertyBlock(block);
            block.SetInt("_Frame", i + 1);
            mR.SetPropertyBlock(block);
            gO.AddComponent<MeshFilter>().mesh = mesh;
        }

        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            MeshRenderer mR = GetComponent<MeshRenderer>();
            mR.GetPropertyBlock(block);
            block.SetInt("_Frame", 0);
            
            mR.SetPropertyBlock(block);
        }
    }
}
