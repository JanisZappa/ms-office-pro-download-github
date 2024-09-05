using UnityEngine;


public class PlacementRecorder
{
    private const int max = 120;
    private int index;
    
    private readonly Placement[] placements = new Placement[max];
    private readonly float[] times = new float[max];


    public void Record(float time, Placement placement)
    {
        index++;
        int id = index % max;
        times[id] = time;
        placements[id] = placement;
    }


    public Placement GetAt(float time)
    {
        for (int i = 0; i < max; i++)
        {
            int id = (index - i - 1 + max) % max;
            float t = times[id];
            if(t > time)
                continue;
            
            int id2 = (id + 1) % max;
            float t2 = times[id2];
            float l = (time - t) / (t2 - t);
            return Placement.Lerp(placements[id], placements[id2], l);
        }
        
        return Placement.OutOfSight;
    }

    public Placement GetAtStepped(float time)
    {
        int best = 0;
        for (int i = 0; i < max; i++)
        {
            int id = (index + 1 + i + max) % max;
            float t = times[id];
            if (t > time)
            {
               Debug.Log(best);
               return placements[best];
            }
                
            
            best = i;
        }
        
        return Placement.OutOfSight;
    }
}
