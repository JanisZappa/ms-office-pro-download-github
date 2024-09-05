using UnityEngine;


public class CropCam : MonoBehaviour
{
    public bool move;
    private Transform trans;

    private Vector3 pos;

    
    private void Start()
    {
        trans = transform;
    }
    
    
    public void UFOUpdate(Vector3 pos)
    {
        if(!move)
            return;
        
        this.pos = Vector3.Lerp(this.pos, pos, Time.deltaTime * 16);
        trans.position = this.pos;
    }
}
