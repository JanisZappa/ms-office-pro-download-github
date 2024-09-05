using UnityEngine;


public class CamLean : MonoBehaviour
{
    private Quaternion rot;
    
    private void Start()
    {
        rot = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(ShaderAngle.TiltAngle, ShaderAngle.Right) * rot;
    }
}
