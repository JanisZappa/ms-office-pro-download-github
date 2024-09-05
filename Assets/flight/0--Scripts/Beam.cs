using UnityEngine;
using UnityEngine.Profiling;


public class Beam : MeshFrame
{
    private Vector3 pathForward;
    private Vector3 root;
    private Quaternion rot;
    private float start, end;
    private bool left;

    private const float LifeTime = 1.5f;
    
    public string hitTag;

    
    public Beam Setup()
    {
        gameObject.SetActive(false);
        return this;
    }
    
    
    public Beam Init(Vector3 pathForward, Vector3 root, Quaternion rot, float start, bool left)
    {
        this.pathForward = pathForward;
        this.root  = root;
        this.rot   = rot;
        this.start = start;
        this.left  = left;
        
        end = 0;
        
        gameObject.SetActive(true);
        return this;
    }
    
    
    private Placement GetAt(float time)
    {
        if(time < start || time > start + LifeTime || end > 0 && time > end)
            return Placement.OutOfSight;
            
          float t = time - start;
        Vector3 f = rot * Vector3.forward;
        
        Vector3 p = root + pathForward * FlightCore.Speed * t + f * 700 * t;
        
        return new Placement(p, rot * ( Quaternion.AngleAxis(t * -2500 * (left? -1 : 1), Vector3.forward)));
    }
    
    
    public bool CanBeRemoved(float time)
    {
        bool remove = start + LifeTime < time -Step * (SnesPixel.FrameCount - 1);
        
        if(remove)
            gameObject.SetActive(false);
        
        return remove;
    }


    public bool Hit(float time)
    {
        Placement plc = GetAt(time);
        int hits = Physics.OverlapBoxNonAlloc(plc.pos + plc.rot * Vector3.forward * 14, new Vector3(3 * .5f, 1.5f * .5f, 5), coll, plc.rot, 1<<11);
        for (int i = 0; i < hits; i++)
        {
            Collider c = coll[i];
            if (c.gameObject.CompareTag(hitTag))
            {
                c.GetComponent<iHitable>().Hit();
                end = time;
                return true;
            }
        }
        
        return false;
    }
    
    private static readonly Collider[] coll = new Collider[10]; 
    
    
    public override void GetFrames()
    {
        Profiler.BeginSample("Blast_GetFrames");
        
        float t = FlightCore.CurrentTime;
        
        for (int e = 0; e < SnesPixel.FrameCount; e++)
            MeshMaster.SetFrame(GetAt(t - Step * e));
        
        Profiler.EndSample();
    }
}
