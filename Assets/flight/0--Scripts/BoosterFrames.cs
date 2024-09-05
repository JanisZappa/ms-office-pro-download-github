using UnityEngine;

public class BoosterFrames : MeshFrame
{
    public Mesh useMesh;
    public float offset;
    private float time;
    private Placement current;
    private const float interval = .3f;
    
    
    protected override void OnEnable()
    {
        if (trans == null)
        {
            mesh = useMesh;
            gameObject.layer = 11;
            trans = transform;
            
            time = offset * interval;
        }
        
        MeshMaster.Add(this);
    }
   
    
    public override void GetFrames()
    {
        time += Time.deltaTime;
        if (time >= interval)
        {
            time -= interval;
            current = new Placement(trans.position, trans.rotation);
        }
        
        for (int i = 0; i < SnesPixel.FrameCount; i++)
            MeshMaster.SetFrame(current);
    }
}
