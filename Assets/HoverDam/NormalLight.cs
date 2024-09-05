using UnityEngine;

[ExecuteInEditMode]
public class NormalLight : MonoBehaviour
{
    private static readonly int Light = Shader.PropertyToID("NormalLight");

    private Quaternion r;
    public bool normal;
    
    
    private void Start()
    {
        r = Quaternion.FromToRotation(Vector3.one.normalized, Vector3.up);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.N))
            normal = !normal;
        
        Matrix4x4 m = Matrix4x4.Rotate(transform.rotation * r).inverse;
        Shader.SetGlobalMatrix(Light, normal? Matrix4x4.identity : m);
    }
}
