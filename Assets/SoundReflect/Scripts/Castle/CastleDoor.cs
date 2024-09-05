using UnityEngine;

[System.Serializable]
public class CastleDoor
{
    public int roomA, roomB;
    public Vector3 pos;
    public Quaternion rot;

    private Vector3 a, b, c, d;

    private const float doorWidth = 1.55f, doorHeight = 2.8f;
    private bool closeToCam;
    
    public CastleDoor(int roomA, int roomB, Vector3 pos, Quaternion rot)
    {
        this.roomA = roomA;
        this.roomB = roomB;
        this.pos = pos;
        this.rot = rot;
    }


    public void CalculateCorners()
    {
        Vector3 dir = rot * Vector3.right;
        a = pos + dir * doorWidth * .5f;
        d = pos - dir * doorWidth * .5f;
        b = a + Vector3.up * doorHeight;
        c = d + Vector3.up * doorHeight;
    }
    
    
    public int GetViewTriangles()
    {
        if (closeToCam)
        {
            TriClip.GetScreenTris();
            return 2;
        }
       
        Vector3 pa = TriClip.GetLocalPoint(a);
        Vector3 pb = TriClip.GetLocalPoint(b);
        Vector3 pc = TriClip.GetLocalPoint(c);
        Vector3 pd = TriClip.GetLocalPoint(d);

        int triCount = 0;
    
        TriClip.ClipTriangle(pa, pb, pc, ref triCount);
        TriClip.ClipTriangle(pc, pd, pa, ref triCount);

        return triCount;
    }


    public void Draw(Color color)
    {
        DRAW.Triangle(a, b, c).SetColor(color);
        DRAW.Triangle(c, d, a).SetColor(color);
    }


    public bool FacingAway(int fromRoom)
    {
        Vector3 dir = ((a + b + c + d) * .25f - TriClip.CamPos);
        closeToCam = dir.SetY(0).sqrMagnitude < Mathf.Pow(doorWidth * .5f, 2);
     
        if (closeToCam)
            return true;
        
        float dot = Vector3.Dot(rot * Vector3.forward, dir.normalized);
        return roomA == fromRoom && dot > 0f || roomB == fromRoom && dot < 0f;
    }
}
