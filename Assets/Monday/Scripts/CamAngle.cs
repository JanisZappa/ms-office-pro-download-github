using UnityEngine;

[ExecuteInEditMode]
public class CamAngle : MonoBehaviour
{
    public Transform goal;
    public LayerMask mask;
    
    private static Transform camAngler;
    private static float angle;
    private static int frame = -1;
    
    private static Quaternion rot;

    public static Quaternion Rot
    {
        get
        {
            if(camAngler == null)
                camAngler = Camera.main.transform.parent;

            if (Time.frameCount != frame || true)
            {
                frame = Time.frameCount;
                rot = Quaternion.AngleAxis(camAngler.rotation.eulerAngles.y, Vector3.up);
            }
            return rot;
        }
    }

    private void LateUpdate()
    {
        transform.localRotation = Rot;

        if (goal != null)
        {
            if(Physics.Raycast(new Ray(goal.position + Vector3.up, Vector3.down), out RaycastHit hit, 4.5f, mask))
                transform.position = hit.point;
        }
            
    }


    
}
