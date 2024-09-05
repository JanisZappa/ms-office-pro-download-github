using UnityEngine;

public class MeshFrame : MonoBehaviour
{
    public MeshType type = MeshType.Animated;
    
    public static float Step;
    
    public Mesh mesh;
    
    protected Transform trans;
    
    
    protected virtual void OnEnable()
    {
        if (trans == null)
        {
            gameObject.layer = 11;
            trans = transform;
        }
        
        if(mesh == null)
            mesh = GetComponent<MeshFilter>().sharedMesh;    
        
        MeshMaster.Add(this);
    }

    
    private void OnDisable()
    {
        MeshMaster.Remove(this);
    }

    
    public virtual void GetFrames()
    {
        Vector3    pos = trans.position;
        Quaternion rot = trans.rotation;
        Placement plc = new Placement(pos, rot);
        for (int i = 0; i < SnesPixel.FrameCount; i++)
            MeshMaster.SetFrame(plc);
    }
    
    
    protected virtual Vector2 Anim(float time) { return Vector2.zero; }
}


public enum MeshType { Animated, Static, StaticSingle, StaticSingleTex }
