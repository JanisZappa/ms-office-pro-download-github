using System.Collections.Generic;
using UnityEngine;


public class TriSurfaceDebug : MonoBehaviour
{
    public Transform plane;
    
    private TriSurface s;
    private int drawShow = -1;
    
    private readonly HashSet<int> triShowCollect = new HashSet<int>();
    private readonly List<int>    triShow        = new List<int>();
    private readonly List<Color>  triColors      = new List<Color>();

    private Transform t;
    private Matrix4x4 m, m2;
    
    private readonly List<Vector3> path = new List<Vector3>(1000);
    
    
    private void Start()
    {
        s = GetComponent<TriSurface>();
        t = transform;
    }

    
    private void LateUpdate()
    {
        m  = t.localToWorldMatrix;
        m2 = t.worldToLocalMatrix;
        
        if (drawShow != s.show)
        {
            drawShow = s.show;
            
            triShowCollect.Clear();
                   triShow.Clear();
                 triColors.Clear();
                 
            triShowCollect.Add(drawShow);
                   triShow.Add(drawShow);
                 triColors.Add(Color.white);
            
            Vector2Int index = s.neighbourIndex[drawShow % s.triCount];
            int c = 0;
            for (int i = 0; i < index.y; i++)
            {
                if (true)
                {
                    Vector2Int index2 = s.neighbourIndex[s.triNeighbours[index.x + i]];
                    for (int e = 0; e < index2.y; e++)
                    {
                        int triID = s.triNeighbours[index2.x + e];
                        if (triShowCollect.Add(triID))
                        {
                            triShow.Add(triID);
                            triColors.Add(COLOR.orange.redish.ShiftHue(c++ * .025f));
                        }
                    }
                }
                else
                {
                    int triID = s.triNeighbours[index.x + i];
                    if (triShowCollect.Add(triID))
                    {
                        triShow.Add(triID);
                        triColors.Add(COLOR.orange.redish.ShiftHue(c++ * .025f));
                    }
                }
            }
        }

        int triShowCount = triShow.Count;
        for (int i = 0; i < triShowCount; i++)
            ShowTriangle(triShow[i], triColors[i]);
        
    //  Plane  //
        plane.position = m.MultiplyPoint(s.TriCenter(drawShow * 3));
        
        
        Vector3 planePos = m2.MultiplyPoint(plane.position);
        Vector3 planeUp  = m2.MultiplyVector(plane.up);

        if (false)
        {
            for (int i = 0; i < triShowCount; i++)
            {
                int id = triShow[i] * 3;
                Vector3 a = s.pts[s.tris[id]];
                Vector3 b = s.pts[s.tris[id + 1]];
                Vector3 c = s.pts[s.tris[id + 2]];
            
                if(TriMath.TriPlaneIntersection(a, b, c, planeUp, planePos, out Vector3 uvw, out Vector3 uvw2))
                {
                    Vector3 na = s.normals[s.tris[id]];
                    Vector3 nb = s.normals[s.tris[id + 1]];
                    Vector3 nc = s.normals[s.tris[id + 2]];
                
                    Vector3 p1 = a * uvw.x + b * uvw.y + c * uvw.z;
                    Vector3 p2 = a * uvw2.x + b * uvw2.y + c * uvw2.z;

                    const float hover = .01f;

                    for (int e = 0; e < 1; e++)
                    {
                        p1 += (na * uvw.x  + nb * uvw.y  + nc * uvw.z).normalized * hover;
                        p2 += (na * uvw2.x + nb * uvw2.y + nc * uvw2.z).normalized * hover;

                        Vector3 d1 = m.MultiplyPoint(p1);
                        Vector3 d2 = m.MultiplyPoint(p2);
                        DRAW.Vector(d1, d2 - d1).SetColor(Color.white);
                    }
                
                    //ShowTriangle(triShow[i], triColors[i]);
                }
            }
        }
        else
        {
            path.Clear();
            int currentTri = drawShow;
            Vector3 last = s.TriCenter(currentTri * 3);
            path.Add(last);
            float dist = 0;
            int lastEdge = -1;
            while (dist < 10)
            {
                for (int i = 0; i < 3; i++)
                {
                    int edgeID = s.triEdges[currentTri * 3 + i];
                    if(edgeID == lastEdge)
                        continue;
                    
                    Vector2Int edge = s.edges[edgeID];
                    Vector3 a = s.pts[edge.x], b = s.pts[edge.y];
                    if (TriMath.LinePlaneIntersection(a, b, planeUp, planePos, out float x))
                    {
                        Vector3 next = a + (b - a) * x;
                        dist += (next - last).magnitude;
                        path.Add(next);
                        last = next;
                        Vector2Int edgeTris = s.edgesTriangles[edgeID];
                        currentTri = edgeTris.x == currentTri ? edgeTris.y : edgeTris.x;
                        lastEdge = edgeID;
                    }
                }
            }

            int pCount = path.Count;
            for (int i = 0; i < pCount; i++)
                path[i] = m.MultiplyPoint(path[i]);

            for (int i = 1; i < pCount; i++)
            {
                Vector3 a = path[i - 1], b = path[i];
                DRAW.Vector(a, b - a).SetColor(Color.white);
            }
        }
        
    }
    
    
    private void ShowTriangle(int i, Color color)
    {
        i *= 3;
        int l = s.triCount * 3;

        Vector3 a = m.MultiplyPoint(s.pts[s.tris[i % l]]);
        Vector3 b = m.MultiplyPoint(s.pts[s.tris[(i + 1) % l]]);
        Vector3 c = m.MultiplyPoint(s.pts[s.tris[(i + 2) % l]]);
        Vector3 d = m.MultiplyPoint(s.TriCenter(i));
        
        const float shrink = .85f;

        Vector3 n = m.MultiplyVector(s.TriNormal(i));
        
        const float hover = .0001f;
        a = d + (a - d) * shrink + n * hover;
        b = d + (b - d) * shrink + n * hover;
        c = d + (c - d) * shrink + n * hover;

        color.a = .2f;
        
        DRAW.Vector(a, b - a).SetColor(color);
        DRAW.Vector(b, c - b).SetColor(color);
        DRAW.Vector(c, a - c).SetColor(color);
        //DRAW.Vector(d, n * .045f).SetColor(color);
    }
}
