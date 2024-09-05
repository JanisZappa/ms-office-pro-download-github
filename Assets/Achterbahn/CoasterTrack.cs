using System.IO;
using UnityEngine;


public class CoasterTrack : MonoBehaviour
{
    public TextAsset data;
    
    [Space]
    
    public float trackLength;
    public int steps;
    public float stepLength;
    private float stepMulti;
    
    public TrackPoint[] pts;

    [Space] 
    public float speed;
    public float accel;
    public float damp;

    public float cartStep;


    private static float Gradient(float v)
    {
        return (Mathf.Acos(v) / Mathf.PI * -1 + .5f) * 2;
    }


    private Vector3 V(BinaryReader r)
    {
        return new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
    }
    
    private void OnEnable()
    {
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            trackLength = r.ReadSingle();
            steps = r.ReadInt32();
            stepLength = trackLength / steps;
            stepMulti = 1f / stepLength;
            
            pts = new TrackPoint[steps];
            for (int i = 0; i < steps; i++)
                pts[i] = new TrackPoint(V(r), Gradient(r.ReadSingle()), Quaternion.LookRotation(V(r), V(r)));
        }
        
        Gradient(1);
        Gradient(-1);
    }


    public TrackPoint GetTrackPoint(float d)
    {
        d = ((d %= trackLength) < 0) ? d+trackLength : d;  
        
        int a = Mathf.FloorToInt(d * stepMulti);
        float l = (d - a * stepLength) * stepMulti;

        return TrackPoint.Lerp(pts[a % steps] , pts[(a + 1) % steps], l);
    }
}


[System.Serializable]
public struct TrackPoint
{
    public Vector3 pos;
    public float gradient;
    public Quaternion rot;

    public Placement Placement => new Placement(pos, rot);

    public TrackPoint(Vector3 pos, float gradient, Quaternion rot)
    {
        this.pos = pos;
        this.gradient = gradient;
        this.rot = rot;
    }


    public static TrackPoint Lerp(TrackPoint a, TrackPoint b, float l)
    {
        return new TrackPoint(Vector3.Lerp(a.pos, b.pos, l), Mathf.Lerp(a.gradient, b.gradient, l), Quaternion.Slerp(a.rot, b.rot, l));
    }
}
