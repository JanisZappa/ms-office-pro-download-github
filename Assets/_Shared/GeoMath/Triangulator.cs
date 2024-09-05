using System.Collections.Generic;
using GeoMath;
using UnityEngine;


//    http://www.habrador.com/tutorials/math/10-triangulation/
//    http://www.habrador.com/tutorials/math/
public static class Triangulator
{
    private static readonly Vertex[] vertexPool = Array(1000);
    
    private static readonly List<Vertex> vertices = new List<Vertex>(1000);
    private static readonly List<Vertex> earVertices = new List<Vertex>(1000);
    
    private static readonly List<int> triangles = new List<int>(1000);


    public static List<int> Fill(Vector3[] points, int pointCount)
    {
        vertices.Clear();
        for (int i = 0; i < pointCount; i++)
            vertices.Add(vertexPool[i].SetPoint(points[i], i));


    //  Find the next and previous vertex  //
        for (int i = 0; i < pointCount; i++)
        {
            Vertex v = vertices[i];

            v.prev = vertices[(i - 1).Repeat(pointCount)];
            v.next = vertices[(i + 1) % pointCount];
        }
        
        
    //  Step 2. Find the reflex (concave) and convex vertices, and ear vertices  //
        for (int i = 0; i < vertices.Count; i++)
            vertices[i].CheckIfReflex();
        
        
    //  Have to find the ears after we have found if the vertex is reflex or convex  //
        earVertices.Clear();
        for (int i = 0; i < vertices.Count; i++)
            IsVertexEar(vertices[i]);
        
        
    //  Step 3. Triangulate!  //
        triangles.Clear();
        while (true)
        {
        //  This means we have just one triangle left  //
            if (vertices.Count == 3)
            {
            //  The final triangle  //
                Vertex v = vertices[0];
                
                triangles.Add(v.index);
                triangles.Add(v.prev.index);
                triangles.Add(v.next.index);
                break;
            }
                
            
        //  Make a triangle of the first ear  //
            Vertex earVertex     = earVertices[0];
            Vertex earVertexPrev = earVertex.prev;
            Vertex earVertexNext = earVertex.next;
            
            triangles.Add(earVertex.index);
            triangles.Add(earVertexPrev.index);
            triangles.Add(earVertexNext.index);
            

        //  Remove the vertex from the lists  //
            earVertices.Remove(earVertex);
               vertices.Remove(earVertex);

        //  Update the previous vertex and next vertex  //
            earVertexPrev.next = earVertexNext;
            earVertexNext.prev = earVertexPrev;

        //  ...see if we have found a new ear by investigating the two vertices that was part of the ear  //
            earVertexPrev.CheckIfReflex();
            earVertexNext.CheckIfReflex();

            earVertices.Remove(earVertexPrev);
            earVertices.Remove(earVertexNext);

            IsVertexEar(earVertexPrev);
            IsVertexEar(earVertexNext);
        }
        
        return triangles;
    }
    
    
//  Check if a vertex is an ear  //
    private static void IsVertexEar(Vertex vertex)
    {
    //  A reflex vertex cant be an ear!  //
        if (vertex.isReflex)
            return;

    //  This triangle to check point in triangle  //
        Vector2 a = vertex.prev.pos;
        Vector2 b = vertex.pos;
        Vector2 c = vertex.next.pos;
        
        int count = vertices.Count;
        for (int i = 0; i < count; i++)
        {
            Vertex v = vertices[i];
            
        //  We only need to check if a reflex vertex is inside of the triangle  //
            if (v.isReflex && Tri.Contains(a, b, c, v.pos))
                return;
        }

        earVertices.Add(vertex);
    }
    
    
    public class Vertex
    {
        public int index;
        public Vector2 pos;
        
    //  The previous and next vertex this vertex is attached to  //
        public Vertex prev, next;

    //  Properties this vertex may have  //
        public bool isReflex; 
        
        public Vertex SetPoint(Vector2 pos, int index)
        {
            this.pos   = pos;
            this.index = index;
            
            return this;
        }


        public void CheckIfReflex()
        {
            isReflex = Tri.IsClockwise(prev.pos, pos, next.pos);
        }
    }
    
    
    


    private static Vertex[] Array(int count)
    {
        Vertex[] array = new Vertex[count];
        
        for (int i = 0; i < count; i++)
            array[i] = new Vertex();
            
        return array;
    }
}
