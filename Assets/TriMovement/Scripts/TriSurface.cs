using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class TriSurface : MonoBehaviour
{
    public int triCount, edgeCount;
    public int show;

    [Space] 
    public Transform plane;
        
    [NonSerialized] public int[] tris;
    [NonSerialized] public int[] triEdges;
    [NonSerialized] public int[] triNeighbours;
    [NonSerialized] public Vector2Int[] neighbourIndex;
    
    [NonSerialized] public Vector3[] pts, normals, triNormals;

    [NonSerialized] public Vector2Int[] edges;
    [NonSerialized] public Vector2Int[] edgesTriangles;

    private Transform t;
    private const float third = 1f / 3;
    
    private readonly QuaternionForce rotForce = new QuaternionForce(300).SetSpeed(70).SetDamp(18);
    private Matrix4x4 m;
    
    
    private void OnEnable()
    {
        t = transform;
        
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        tris = mesh.triangles;
        pts  = mesh.vertices;
        normals = mesh.normals;
        
        triCount = tris.Length / 3;
        
        triEdges = new int[triCount * 3];

        List<Vector2Int> edgeList = new List<Vector2Int>();
        int edge = 0;
        for (int i = 0; i < triCount; i++)
        {
            int a = tris[i * 3];
            int b = tris[i * 3 + 1];
            int c = tris[i * 3 + 2];
            triEdges[edge++] = AddEdge(a, b, edgeList);
            triEdges[edge++] = AddEdge(b, c, edgeList);
            triEdges[edge++] = AddEdge(c, a, edgeList);
        }

        edges = edgeList.ToArray();
        edgeCount = edges.Length;
        
        edgesTriangles = new Vector2Int[edgeCount];
        
        for (int i = 0; i < edgeCount; i++)
        {
            int a = 0, b = 0, c = 0;
            for (int e = 0; e < triCount; e++)
            {
                int at = triEdges[e * 3];
                int bt = triEdges[e * 3 + 1];
                int ct = triEdges[e * 3 + 2];

                if (at == i || bt == i || ct == i)
                {
                    if (c == 0)
                        a = e;
                    else
                        b = e;

                    c++;

                    if (c == 2)
                        break;
                }
            }
            
            edgesTriangles[i] = new Vector2Int(a, b);
        }
        
        
        List<int> neighbourList = new List<int>();
        List<Vector2Int> neighbourIndexList = new List<Vector2Int>();
        
        for (int i = 0; i < triCount; i++)
        {
           int a = tris[i * 3];
           int b = tris[i * 3 + 1];
           int c = tris[i * 3 + 2];
        
           int offset = neighbourList.Count;
        
           for (int e = 0; e < triCount; e++)
           {
               if(e == i)
                   continue;
               
               int a2 = tris[e * 3];
               int b2 = tris[e * 3 + 1];
               int c2 = tris[e * 3 + 2];
        
               int value = (a == a2 || a == b2 || a == c2 ? 1 : 0) +
                           (b == a2 || b == b2 || b == c2 ? 1 : 0) +
                           (c == a2 || c == b2 || c == c2 ? 1 : 0);
        
               if (value >= 1)
                   neighbourList.Add(e);
           }
           
           neighbourIndexList.Add(new Vector2Int(offset, neighbourList.Count - offset));
        }
        
        triNeighbours = neighbourList.ToArray();
        neighbourIndex = neighbourIndexList.ToArray();
        
        triNormals = new Vector3[triCount];
        for (int i = 0; i < triCount; i++)
        {
            Vector3 a = pts[tris[i * 3]];
            Vector3 b = pts[tris[i * 3 + 1]];
            Vector3 c = pts[tris[i * 3 + 2]];

            triNormals[i] = Vector3.Cross((b - a).normalized, (c - a).normalized).normalized;
        }
    }


    private static int AddEdge(int a, int b, List<Vector2Int> edgeCollect)
    {
        Vector2Int edge = b < a? new Vector2Int(b, a) : new Vector2Int(a, b);

        int found = edgeCollect.IndexOf(edge);
        if (found != -1)
            return found;
        
        edgeCollect.Add(edge);
        return edgeCollect.Count - 1;
    }


    private void Update()
    {
    //  Input  //
        int input = (Input.GetKeyDown(KeyCode.LeftArrow) ? -1 : 0) + (Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0);
        if (input != 0)
        {
            show += input;
            if (show < 0)
                show = triCount - 1;
            if (show == triCount)
                show = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2Int index = neighbourIndex[show % triCount];
            show = triNeighbours[index.x + Random.Range(0, index.y)];
        }

    //  Rotation  //
        Vector3 f = -TriNormal(show * 3);
        Vector3 r = Vector3.Cross(Vector3.Cross(f, Vector3.up).normalized, f).normalized;

        t.localRotation = Quaternion.Euler(1, 1, 0) * rotForce.Update(Quaternion.Inverse(Quaternion.LookRotation(f, r)), Time.deltaTime);

        m = t.localToWorldMatrix;
    }


    public Vector3 TriNormal(int i)
    {
        int l = triCount * 3;
        
        Vector3 aN = normals[tris[i % l]];
        Vector3 bN = normals[tris[(i + 1) % l]];
        Vector3 cN = normals[tris[(i + 2) % l]];

        //return triNormals[i / 3];
        
        return (aN + bN + cN).normalized;
    }


    public Vector3 TriCenter(int i)
    {
        int l = triCount * 3;
        
        Vector3 a = pts[tris[i % l]];
        Vector3 b = pts[tris[(i + 1) % l]];
        Vector3 c = pts[tris[(i + 2) % l]];
        
        return a * third + b * third + c * third;
    }


    public Vector3 GetPos(TriPos pos, bool world = false)
    {
        Vector3 a = pts[tris[pos.tri * 3]];
        Vector3 b = pts[tris[pos.tri * 3 + 1]];
        Vector3 c = pts[tris[pos.tri * 3 + 2]];

        Vector3 r = a * pos.uvw.x + b * pos.uvw.y + c * pos.uvw.z;
        return world? m.MultiplyPoint(r) : r;
    }
    
    
    public Vector3 GetNormal(TriPos pos, bool world = false)
    {
        Vector3 a = normals[tris[pos.tri * 3]];
        Vector3 b = normals[tris[pos.tri * 3 + 1]];
        Vector3 c = normals[tris[pos.tri * 3 + 2]];

        Vector3 r = (a * pos.uvw.x + b * pos.uvw.y + c * pos.uvw.z).normalized;
        return world? m.MultiplyVector(r) : r;
    }
    
    
    public Vector3 GetTriNormal(TriPos pos, bool world = false)
    {
        Vector3 a = triNormals[pos.tri];

        return world? m.MultiplyVector(a) : a;
    }


    public TriPos GetRandomTriPos()
    {
        int pick = Random.Range(0, triCount);
       
        return new TriPos(pick, new Vector3(1, 1, 1) * third, (pts[tris[pick * 3]] * .5f + pts[tris[pick * 3 + 1]] * .5f - TriCenter(pick * 3)).normalized);
    }


    public TriPos Move(TriPos pos, float dist)
    {
        float moved = 0;
        int trys = 0;
        int lastEdge = -1;
        while (true)
        {
            Vector3 p = GetPos(pos);
            Vector3 n = GetTriNormal(pos);
            Vector3 d = Vector3.Cross(pos.dir, n).normalized;

            Vector3 a = pts[tris[pos.tri * 3]];
            Vector3 b = pts[tris[pos.tri * 3 + 1]];
            Vector3 c = pts[tris[pos.tri * 3 + 2]];

            if(!TriMath.TriPlaneIntersection(a, b, c, d, p, out EdgeHit outA, out EdgeHit outB))
                Debug.Log("Noo");
            Vector3 edgeA = outA.id == 0? a + (b - a) * outA.lerp : 
                            outA.id == 1? b + (c - b) * outA.lerp : c + (a - c) * outA.lerp;
            Vector3 edgeB = outB.id == 0? a + (b - a) * outB.lerp : 
                            outB.id == 1? b + (c - b) * outB.lerp : c + (a - c) * outB.lerp;

            int edgeIDA = triEdges[pos.tri * 3 + outA.id];
            int edgeIDB = triEdges[pos.tri * 3 + outB.id];
            bool useA = lastEdge == -1? Vector3.Dot(edgeA - p, pos.dir) > 0 : edgeIDA != lastEdge;
            Vector3 edgeGoal = useA? edgeA : edgeB;
            int edge = useA ? edgeIDA : edgeIDB;
            Vector3 dir = edgeGoal - p;
            float dirDist = dir.magnitude;
            float futureMoved = moved + dirDist;
            if (futureMoved > dist)
            {
                p += pos.dir * (dist - moved);
                //Debug.Log("f " + trys + " Future " + futureMoved + " > " + dist);
                Vector3 newUVW = TriMath.GetBarycentricCoordinates(p, a, b, c);
                // Debug.Log(newUVW.ToString("F5"));
                return new TriPos(pos.tri, newUVW, pos.dir);
            }

            lastEdge = edge;
            trys++;
            Vector2Int edgeTris = edgesTriangles[edge];
            int nextTri = edgeTris.x == pos.tri ? edgeTris.y : edgeTris.x;
            moved = futureMoved;
            //Debug.Log("Tri Switch to: " + nextTri + " Moved: " + moved);
            a = pts[tris[nextTri * 3]];
            b = pts[tris[nextTri * 3 + 1]];
            c = pts[tris[nextTri * 3 + 2]];

            p = edgeGoal;
            
            if(!TriMath.TriPlaneIntersection(a, b, c, d, p, out outA, out outB))
                Debug.Log("Nooo 2");

            useA = triEdges[nextTri * 3 + outA.id] != lastEdge;
            edgeA = outA.id == 0? a + (b - a) * outA.lerp : 
                outA.id == 1? b + (c - b) * outA.lerp : c + (a - c) * outA.lerp;
            edgeB = outB.id == 0? a + (b - a) * outB.lerp : 
                outB.id == 1? b + (c - b) * outB.lerp : c + (a - c) * outB.lerp;
            edgeGoal = useA? edgeA : edgeB;
            Vector3 nextDir = (edgeGoal - p).normalized;
            
            pos = new TriPos(nextTri, TriMath.GetBarycentricCoordinates(p, a, b, c), nextDir);
        }
    }
    
}

public struct TriPos
{
    public int tri;
    public Vector3 uvw;
    public Vector3 dir;

    public TriPos(int tri, Vector3 uvw, Vector3 dir)
    {
        this.tri = tri;
        this.uvw = uvw;
        this.dir = dir;
    }
}
