using GeoMath;
using UnityEngine;

public static class TriClip 
{
    public static void StoreCameraParams(Camera cam)
    {
        Transform trans = cam.transform;
        toObj   = trans.worldToLocalMatrix;
        toWorld = trans.localToWorldMatrix;
        
        clipDist = cam.nearClipPlane;
        TriClip.cam = cam;
        CamPos = trans.position;
        CamForward = trans.forward;
        CamUp = trans.up;
        CamRight = trans.right;
    }

    private static Matrix4x4 toObj, toWorld;
    private static float clipDist;
    private static Camera cam;
    public static Vector3 CamPos, CamForward, CamRight, CamUp;
    
    
    public static readonly Vector3[] triList = new Vector3[12];

    public static Vector3 GetLocalPoint(Vector3 pos)
    {
        return toObj.MultiplyPoint(pos).AddZ(-clipDist);
    }
    
    
    public static void ClipTriangle(Vector3 pa, Vector3 pb, Vector3 pc, ref int triCount)
    {
        switch ((pa.z > 0 ? 100 : 0) + (pb.z > 0 ? 10 : 0) + (pc.z > 0 ? 1 : 0))
        {
        //  All Good  //
            case 111:
                AddTriIfInView(pa, pb, pc, ref triCount);
                break;
        //  One in front  //
            case 100:
                OneInFront(pa, pb, pc, ref triCount);
                break;
            case 010:
                OneInFront(pb, pc, pa, ref triCount);
                break;
            case 001:
                OneInFront(pc, pa, pb, ref triCount);
                break;
        //  Two in front  //    
            case 110:
                TwoInFront(pa, pb, pc, ref triCount);
                break;
            case 011:
                TwoInFront(pb, pc, pa, ref triCount);
                break;
            case 101:
                TwoInFront(pa, pc, pb, ref triCount);
                break;
        }
    }


    private static void OneInFront(Vector3 front, Vector3 backA, Vector3 backB, ref int triCount)
    {
        AddTriIfInView(
            front, 
            front + (backA - front) * (front.z / (front.z - backA.z)), 
            front + (backB - front) * (front.z / (front.z - backB.z)),
            ref triCount);
    }
   
    private static void TwoInFront(Vector3 frontA, Vector3 frontB, Vector3 back, ref int triCount)
    {
        Vector3 aClip = frontA + (back - frontA) * (frontA.z / (frontA.z - back.z));
        Vector3 bClip = frontB + (back - frontB) * (frontB.z / (frontB.z - back.z));
            
        AddTriIfInView(frontA, frontB, aClip, ref triCount);
        AddTriIfInView( aClip, frontB, bClip, ref triCount);
    }


    private static void AddTriIfInView(Vector3 a, Vector3 b, Vector3 c, ref int triCount)
    {
        a = cam.WorldToViewportPoint(toWorld.MultiplyPoint(a.AddZ(clipDist)));
        b = cam.WorldToViewportPoint(toWorld.MultiplyPoint(b.AddZ(clipDist)));
        c = cam.WorldToViewportPoint(toWorld.MultiplyPoint(c.AddZ(clipDist)));
        
        if(TriTriOverlap(a, b, c, Vector2.zero, Vector2.up, Vector2.one) ||
           TriTriOverlap(a, b, c, Vector2.one, Vector2.right, Vector2.zero))
        {
            int offset = triCount * 3;
            triList[offset++] = a;
            triList[offset++] = b;
            triList[offset]   = c;
            triCount++;
        }
    }


    public static bool TriTriOverlap(Vector2 a1, Vector2 b1, Vector2 c1, Vector2 a2, Vector2 b2, Vector2 c2)
    {
        /*Profiler.BeginSample("GJK");
        GJK.Intersection(new Triangle(a1, b1, c1), new Triangle(a2, b2, c2));
        Profiler.EndSample();*/
        
    //  Bounds  //
        if(!new Bounds2D(a1).Add(b1).Add(c1).Intersects(new Bounds2D(a2).Add(b2).Add(c2)))
           return false;
           
    //  Check Points Inside
    if (Tri.Contains(a1, b1, c1, a2) ||
        Tri.Contains(a1, b1, c1, b2) ||
        Tri.Contains(a1, b1, c1, c2) ||
        Tri.Contains(a2, b2, c2, a1) ||
        Tri.Contains(a2, b2, c2, b1) ||
        Tri.Contains(a2, b2, c2, c1))
    {
        return true;
    }
            
        
        Line la1 = new Line(a1, b1);
        Line lb1 = new Line(b1, c1);
        Line lc1 = new Line(c1, a1);
        Line la2 = new Line(a2, b2);
        Line lb2 = new Line(b2, c2);
        Line lc2 = new Line(c2, a2);

        /*Vector2 hit;
        if (la1.Contact(la2, out hit) || la1.Contact(lb2, out hit) || la1.Contact(lc2, out hit) ||
            lb1.Contact(la2, out hit) || lb1.Contact(lb2, out hit) || lb1.Contact(lc2, out hit) ||
            lc1.Contact(la2, out hit) || lc1.Contact(lb2, out hit) || lc1.Contact(lc2, out hit))
        {
            return true;
        }*/

        if (LineLine(la1, la2) || LineLine(la1, lb2) || LineLine(la1, lc2) ||
            LineLine(lb1, la2) || LineLine(lb1, lb2) || LineLine(lb1, lc2) ||
            LineLine(lc1, la2) || LineLine(lc1, lb2) || LineLine(lc1, lc2))
            return true;
        
        return false;
    }


    private static bool LineLine(Line a, Line b)
    {
        float compareDot = Vector2.Dot(a.dir.normalized, b.dir.normalized);
        
        if (!(compareDot > -1 && compareDot < 1))
            return false;
        
        float denominator = 1f / (a.dir.y * b.dir.x - a.dir.x * b.dir.y);

        float x = a.l1.x - b.l1.x;
        float y = b.l1.y - a.l1.y;
        
        float t1 = (x * b.dir.y + y * b.dir.x) * denominator;
        if (t1 < 0 || t1 > 1)
            return false;

        float t2 = (x * -a.dir.y + y * -a.dir.x) * -denominator;
        
        return !(t2 < 0) && !(t2 > 1);
    }


    public static void GetScreenTris()
    {
        triList[0] = Vector2.zero;
        triList[1] = Vector2.up;
        triList[2] = Vector2.one;
        triList[3] = Vector2.one;
        triList[4] = Vector2.right;
        triList[5] = Vector2.zero;
    }
}
