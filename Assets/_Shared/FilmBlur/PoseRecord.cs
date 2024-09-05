using System.Collections.Generic;
using UnityEngine;


public class PoseRecord
{
    private readonly bool pos, rot, local;
    
    private readonly List<RecPose> record = new List<RecPose>(100);

    public PoseRecord(bool pos, bool rot, bool local)
    {
        this.pos   = pos;
        this.rot   = rot;
        this.local = local;
    }

    public void Record(Transform t, float time)
    {
        record.Add(new RecPose(pos? local? t.localPosition : t.position : Vector3.zero, rot? local? t.localRotation : t.rotation : Quaternion.identity, time));
    }


    public void Trimm(float time)
    {
        while(record.Count > 1 && record[1].time < time)
            record.RemoveAt(0);
    }


    public void Set(Transform t, float time)
    {
        int count = record.Count;
        int min = -1;
        for (int i = 0; i < count; i++)
        {
            RecPose p = record[i];
            
            if(p.time < time)
                min = i;
            
            if (i == count - 1 && min == -1)
            {
                if (local)
                {
                    if(pos)     t.localPosition = p.pos;
                    if(rot)     t.localRotation = p.rot;
                }
                else
                {
                    if(pos)     t.position = p.pos;
                    if(rot)     t.rotation = p.rot;
                }
            }
        }

        if (min != -1)
        {
            RecPose a = record[min];
            RecPose b = record[min + 1];
            
            float lerp = (time - a.time) / (b.time - a.time);
            
            if (local)
            {
                if(pos)     t.localPosition = Vector3.Lerp(a.pos, b.pos, lerp);
                if(rot)     t.localRotation = Quaternion.Slerp(a.rot, b.rot, lerp);
            }
            else
            {
                if(pos)     t.position = Vector3.Lerp(a.pos, b.pos, lerp);
                if(rot)     t.rotation = Quaternion.Slerp(a.rot, b.rot, lerp);
            }
        }
    }
}

public struct RecPose
{
    public readonly Vector3 pos;
    public readonly Quaternion rot;
    public readonly float time;

    public RecPose(Vector3 pos, Quaternion rot, float time)
    {
        this.pos = pos;
        this.rot = rot;
        this.time = time;
    }
}
