using UnityEngine;


public class SameRot : MonoBehaviour
{
    public Transform target;
    private Transform trans;

    private static int frame;
    private static Vector3 targetPos, targetForward;
    private Vector3 up;


    private Vector3 TargetPos
    {
        get
        {
            if (frame != Time.frameCount)
            {
                frame = Time.frameCount;
                targetPos = target.position;
                targetForward = target.forward.SetY(0).normalized;
            }

            return targetPos;
        }
    }


    private void Start()
    {
        trans = transform;
        up = trans.up;
    }


    private void LateUpdate()
    {
        Vector3 f = ((trans.position - TargetPos).SetY(0).normalized + targetForward).normalized;
        //f = targetForward;
        trans.rotation = Quaternion.LookRotation(f, up);
    }
}
