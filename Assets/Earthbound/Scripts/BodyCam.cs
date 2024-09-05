using UnityEngine;

public class BodyCam : MonoBehaviour
{
    public Body target;
    
    private void LateUpdate()
    {
        transform.position = target.Pos;
    }
}
