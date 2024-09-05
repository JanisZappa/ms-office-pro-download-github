using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public static class MeshDivider
{
    private static readonly List<int>     trisA  = new List<int>(10000),     trisB  = new List<int>(10000);
    private static readonly List<Vector3> vertsA = new List<Vector3>(10000), vertsB = new List<Vector3>(10000);
    private static readonly List<Vector3> normsA = new List<Vector3>(10000), normsB = new List<Vector3>(10000);
    private static readonly List<Vector4> tansA  = new List<Vector4>(10000), tansB  = new List<Vector4>(10000);
    private static readonly List<Color>   colsA  = new List<Color>(10000),   colsB  = new List<Color>(10000);
    private static readonly List<Vector4> uvsA   = new List<Vector4>(10000), uvsB   = new List<Vector4>(10000);
    private static readonly List<Mesh>    meshCollect = new List<Mesh>();
    
    private static readonly Dictionary<Vector2Int, int> edgePoints = new Dictionary<Vector2Int, int>(10000);
    private static readonly int[] vertDone = new int[1000000];
    
    public static List<Mesh> Subdivide(Mesh mesh, int subdivisions)
    {
        meshCollect.Clear();
        meshCollect.Add(mesh);
        
        mesh.GetTriangles(trisB, 0);
        mesh.GetVertices(vertsB);
        mesh.GetNormals(normsB);
        mesh.GetTangents(tansB);
        mesh.GetColors(colsB);
        mesh.GetUVs(0, uvsB);
        
        
        int     a, b, c;
        Vector3 vertA, vertB, vertC;
        Vector3 normA, normB, normC;
        Vector4 tanA, tanB, tanC;
        Color   colA, colB, colC;
        Vector2 uvA, uvB, uvC;
        bool shortAB, shortBC, shortCA;
        
        float sideLength;
        //int skip1 = 0, skip2 = 0, skip3 = 0;
        for (int i = 0; i < subdivisions; i++)
        {
            sideLength = Mathf.Pow(1f / Mathf.Pow(2, 1 + i), 2) * .99f;
            
            bool bToA = i % 2 == 0;
            List<int>     readTris  = bToA ? trisB  : trisA,  writeTris   = bToA ? trisA  : trisB;
            List<Vector3> readVerts = bToA ? vertsB : vertsA, writeVerts  = bToA ? vertsA : vertsB;
            List<Vector3> readNorms = bToA ? normsB : normsA, writeNorms  = bToA ? normsA : normsB;
            List<Vector4> readTans  = bToA ? tansB  : tansA,  writeTans   = bToA ? tansA  : tansB;
            List<Color>   readCols  = bToA ? colsB  : colsA,  writeCols   = bToA ? colsA  : colsB;
            List<Vector4> readUVs   = bToA ? uvsB   : uvsA,   writeUVs    = bToA ? uvsA   : uvsB;
            
            writeTris.Clear();
            writeVerts.Clear();
            writeNorms.Clear();
            writeTans.Clear();
            writeCols.Clear();
            writeUVs.Clear();
                
            int index    = 0;
            int triCount = readTris.Count;
            for (int t = 0; t < triCount; t += 3)
            {
                a = readTris[t];
                b = readTris[t + 1];
                c = readTris[t + 2];

                vertA = readVerts[a];
                vertB = readVerts[b];
                vertC = readVerts[c];

                normA = readNorms[a];
                normB = readNorms[b];
                normC = readNorms[c];

                tanA = readTans[a];
                tanB = readTans[b];
                tanC = readTans[c];

                colA = readCols[a];
                colB = readCols[b];
                colC = readCols[c];

                uvA = readUVs[a];
                uvB = readUVs[b];
                uvC = readUVs[c];

                shortAB = (vertA - vertB).sqrMagnitude < sideLength;
                shortBC = (vertB - vertC).sqrMagnitude < sideLength;
                shortCA = (vertC - vertA).sqrMagnitude < sideLength;


                //  These Always Have To Be Written
                writeVerts.Add(vertA);
                writeVerts.Add(vertB);
                writeVerts.Add(vertC);

                writeNorms.Add(normA);
                writeNorms.Add(normB);
                writeNorms.Add(normC);

                writeTans.Add(tanA);
                writeTans.Add(tanB);
                writeTans.Add(tanC);

                writeCols.Add(colA);
                writeCols.Add(colB);
                writeCols.Add(colC);

                writeUVs.Add(uvA);
                writeUVs.Add(uvB);
                writeUVs.Add(uvC);

                if (true)
                {
                    //  All Good  //
                    if (shortAB && shortBC && shortCA)
                    {
                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);

                        index += 3;
                        continue;
                    }

                    //  One Long Edge  //
                    if (shortAB && shortBC)
                    {
                        writeVerts.Add((vertC + vertA) * .5f);
                        writeNorms.Add((normC + normA).normalized);
                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                        writeCols.Add((colC + colA) * .5f);
                        writeUVs.Add((uvC + uvA) * .5f);

                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);

                        index += 4;
                        continue;
                    }

                    if (shortBC && shortCA)
                    {
                        writeVerts.Add((vertA + vertB) * .5f);
                        writeNorms.Add((normA + normB).normalized);
                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                        writeCols.Add((colA + colB) * .5f);
                        writeUVs.Add((uvA + uvB) * .5f);

                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 2);
                        writeTris.Add(index);

                        index += 4;
                        continue;
                    }

                    if (shortCA && shortAB)
                    {
                        writeVerts.Add((vertB + vertC) * .5f);
                        writeNorms.Add((normB + normC).normalized);
                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                        writeCols.Add((colB + colC) * .5f);
                        writeUVs.Add((uvB + uvC) * .5f);

                        writeTris.Add(index + 2);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index);
                        writeTris.Add(index + 1);

                        index += 4;
                        continue;
                    }

                    if (shortAB)
                    {
                        writeVerts.Add((vertB + vertC) * .5f);
                        writeVerts.Add((vertC + vertA) * .5f);

                        writeNorms.Add((normB + normC).normalized);
                        writeNorms.Add((normC + normA).normalized);

                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);

                        writeCols.Add((colB + colC) * .5f);
                        writeCols.Add((colC + colA) * .5f);

                        writeUVs.Add((uvB + uvC) * .5f);
                        writeUVs.Add((uvC + uvA) * .5f);

                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }

                    if (shortBC)
                    {
                        writeVerts.Add((vertC + vertA) * .5f);
                        writeVerts.Add((vertA + vertB) * .5f);

                        writeNorms.Add((normC + normA).normalized);
                        writeNorms.Add((normA + normB).normalized);

                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);

                        writeCols.Add((colC + colA) * .5f);
                        writeCols.Add((colA + colB) * .5f);

                        writeUVs.Add((uvC + uvA) * .5f);
                        writeUVs.Add((uvA + uvB) * .5f);

                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }

                    if (shortCA)
                    {
                        writeVerts.Add((vertA + vertB) * .5f);
                        writeVerts.Add((vertB + vertC) * .5f);

                        writeNorms.Add((normA + normB).normalized);
                        writeNorms.Add((normB + normC).normalized);

                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);

                        writeCols.Add((colA + colB) * .5f);
                        writeCols.Add((colB + colC) * .5f);

                        writeUVs.Add((uvA + uvB) * .5f);
                        writeUVs.Add((uvB + uvC) * .5f);

                        writeTris.Add(index + 2);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }
                }

                writeVerts.Add((vertA + vertB) * .5f);
                writeVerts.Add((vertB + vertC) * .5f);
                writeVerts.Add((vertC + vertA) * .5f);

                writeNorms.Add((normA + normB).normalized);
                writeNorms.Add((normB + normC).normalized);
                writeNorms.Add((normC + normA).normalized);

                writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                writeTans.Add((tanC * .5f + tanA * .5f).normalized);

                writeCols.Add((colA + colB) * .5f);
                writeCols.Add((colB + colC) * .5f);
                writeCols.Add((colC + colA) * .5f);

                writeUVs.Add((uvA + uvB) * .5f);
                writeUVs.Add((uvB + uvC) * .5f);
                writeUVs.Add((uvC + uvA) * .5f);

                //  Indexes  //
                writeTris.Add(index);
                writeTris.Add(index + 3);
                writeTris.Add(index + 5);

                writeTris.Add(index + 1);
                writeTris.Add(index + 4);
                writeTris.Add(index + 3);

                writeTris.Add(index + 2);
                writeTris.Add(index + 5);
                writeTris.Add(index + 4);

                writeTris.Add(index + 3);
                writeTris.Add(index + 4);
                writeTris.Add(index + 5);

                index += 6;
            }

            Mesh newMesh = new Mesh {name = mesh.name + "_" + (1 + i), indexFormat = index <= 65535? IndexFormat.UInt16 : IndexFormat.UInt32};
            newMesh.SetVertices(writeVerts);
            newMesh.SetTriangles(writeTris, 0);
            newMesh.SetNormals(writeNorms);
            newMesh.SetTangents(writeTans);
            newMesh.SetColors(writeCols);
            newMesh.SetUVs(0, writeUVs);
            newMesh.RecalculateBounds();
            
            meshCollect.Add(newMesh);
        }
        
        // Debug.LogFormat("Skipped 1: {0} | Skipped 2:  {1} | Skipped 3: {2}", skip1, skip2, skip3);
        return meshCollect;
    }
    
    
    public static List<Mesh> SubdivideVertMerge(Mesh mesh, int subdivisions)
    {
        mesh.GetTriangles(trisB, 0);
        mesh.GetVertices(vertsB);
        mesh.GetNormals(normsB);
        mesh.GetTangents(tansB);
        mesh.GetColors(colsB);
        mesh.GetUVs(0, uvsB);
        
        meshCollect.Clear();
        meshCollect.Add(mesh);
        
        Bounds bounds = mesh.bounds;
        bounds.Expand(1);
        
        
        int a, b, c;
        int offset;
        int edgeAB, edgeBC, edgeCA;
        Vector3 vertA = Vector3.zero, vertB = Vector3.zero, vertC = Vector3.zero;
        Vector3 normA = Vector3.zero, normB = Vector3.zero, normC = Vector3.zero;
        Vector4 tanA  = Vector4.zero, tanB  = Vector4.zero, tanC  = Vector4.zero;
        Color   colA  = Color.white,  colB  = Color.white,  colC  = Color.white;
        Vector4 uvA   = Vector4.zero, uvB   = Vector4.zero, uvC   = Vector4.zero;
        bool shortAB, shortBC, shortCA;
        int indexA, indexB, indexC;
        bool doneA, doneB, doneC;
        Vector2Int check;
        float sideLength;
        
        
        for (int i = 0; i < subdivisions; i++)
        {
            sideLength = Mathf.Pow(1f / Mathf.Pow(2, 1 + i), 2) * .99f;
            
            bool bToA = i % 2 == 0;
            List<int>     readTris  = bToA ? trisB  : trisA,  writeTris   = bToA ? trisA  : trisB;
            List<Vector3> readVerts = bToA ? vertsB : vertsA, writeVerts  = bToA ? vertsA : vertsB;
            List<Vector3> readNorms = bToA ? normsB : normsA, writeNorms  = bToA ? normsA : normsB;
            List<Vector4> readTans  = bToA ? tansB  : tansA,  writeTans   = bToA ? tansA  : tansB;
            List<Color>   readCols  = bToA ? colsB  : colsA,  writeCols   = bToA ? colsA  : colsB;
            List<Vector4> readUVs   = bToA ? uvsB   : uvsA,   writeUVs    = bToA ? uvsA   : uvsB;
            
            writeTris.Clear();
            writeVerts.Clear();
            writeNorms.Clear();
            writeTans.Clear();
            writeCols.Clear();
            writeUVs.Clear();
            
            edgePoints.Clear();
                
            int index     = 0;
            int triCount  = readTris.Count;
            int vertCount = readVerts.Count;
            for (int v = 0; v < vertCount; v++)
                vertDone[v] = -1;
            
            for (int t = 0; t < triCount; t += 3)
            {
                a = readTris[t];
                b = readTris[t + 1];
                c = readTris[t + 2];
                
                indexA = vertDone[a];
                indexB = vertDone[b];
                indexC = vertDone[c];
                
                doneA = indexA != -1;
                doneB = indexB != -1;
                doneC = indexC != -1;
                 
                offset = 0;
                indexA = doneA? indexA : index + offset++;
                indexB = doneB? indexB : index + offset++;
                indexC = doneC? indexC : index + offset++;
                int reverse = -(3 - offset);
                    
                vertDone[a] = indexA;
                vertDone[b] = indexB;
                vertDone[c] = indexC;

                vertA = readVerts[a];
                vertB = readVerts[b];
                vertC = readVerts[c];
                
                normA = readNorms[a];
                tanA  = readTans[a];
                colA  = readCols[a];
                uvA   = readUVs[a];
                
                normB = readNorms[b];
                tanB  = readTans[b];
                colB  = readCols[b];
                uvB   = readUVs[b];
                
                normC = readNorms[c];
                tanC  = readTans[c];
                colC  = readCols[c];
                uvC   = readUVs[c];
                
                
                if (!doneA)
                {
                    writeVerts.Add(vertA);
                    writeNorms.Add(normA);
                     writeTans.Add(tanA);
                     writeCols.Add(colA);
                      writeUVs.Add(uvA);
                }
                if (!doneB)
                {
                    writeVerts.Add(vertB);
                    writeNorms.Add(normB);
                     writeTans.Add(tanB);
                     writeCols.Add(colB);
                      writeUVs.Add(uvB);
                }
                if (!doneC)
                {
                    writeVerts.Add(vertC);
                    writeNorms.Add(normC);
                     writeTans.Add(tanC);
                     writeCols.Add(colC);
                      writeUVs.Add(uvC);
                }
                
                
                shortAB = (vertA - vertB).sqrMagnitude < sideLength;
                shortBC = (vertB - vertC).sqrMagnitude < sideLength;
                shortCA = (vertC - vertA).sqrMagnitude < sideLength;

                if (true)
                {
                    //  All Good  //
                    if (shortAB && shortBC && shortCA)
                    {
                        writeTris.Add(indexA);
                        writeTris.Add(indexB);
                        writeTris.Add(indexC);

                        index += 3 + reverse;
                        continue;
                    }

                    //  One Long Edge  //
                    if (shortAB && shortBC)
                    {
                        check = new Vector2Int(c, a);
                        if (edgePoints.TryGetValue(check, out edgeCA))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertC + vertA) * .5f);
                            writeNorms.Add((normC + normA).normalized);
                            writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                            writeCols.Add((colC + colA) * .5f);
                            writeUVs.Add((uvC + uvA) * .5f);
                            
                            edgeCA = index + 3 + reverse;
                            edgePoints.Add(check, edgeCA);
                        }

                        writeTris.Add(indexA);
                        writeTris.Add(indexB);
                        writeTris.Add(edgeCA);

                        writeTris.Add(edgeCA);
                        writeTris.Add(indexB);
                        writeTris.Add(indexC);

                        index += 4 + reverse;
                        continue;
                    }

                    if (shortBC && shortCA)
                    {
                        check = new Vector2Int(a, b);
                        if (edgePoints.TryGetValue(check, out edgeAB))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertA + vertB) * .5f);
                            writeNorms.Add((normA + normB).normalized);
                            writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                            writeCols.Add((colA + colB) * .5f);
                            writeUVs.Add((uvA + uvB) * .5f);
                            
                            edgeAB = index + 3 + reverse;
                            edgePoints.Add(check, edgeAB);
                        }

                        writeTris.Add(indexB);
                        writeTris.Add(indexC);
                        writeTris.Add(edgeAB);

                        writeTris.Add(edgeAB);
                        writeTris.Add(indexC);
                        writeTris.Add(indexA);

                        index += 4 + reverse;
                        continue;
                    }

                    if (shortCA && shortAB)
                    {
                        check = new Vector2Int(b, c);
                        if (edgePoints.TryGetValue(check, out edgeBC))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertB + vertC) * .5f);
                            writeNorms.Add((normB + normC).normalized);
                            writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                            writeCols.Add((colB + colC) * .5f);
                            writeUVs.Add((uvB + uvC) * .5f);
                            
                            edgeBC = index + 3 + reverse;
                            edgePoints.Add(check, edgeBC);
                        }
                        
                        writeTris.Add(indexC);
                        writeTris.Add(indexA);
                        writeTris.Add(edgeBC);

                        writeTris.Add(edgeBC);
                        writeTris.Add(indexA);
                        writeTris.Add(indexB);

                        index += 4 + reverse;
                        continue;
                    }

                    if (shortAB)
                    {
                        offset = 0;
                        check = new Vector2Int(b, c);
                        if (edgePoints.TryGetValue(check, out edgeBC))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertB + vertC) * .5f);
                            writeNorms.Add((normB + normC).normalized);
                            writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                            writeCols.Add((colB + colC) * .5f);
                            writeUVs.Add((uvB + uvC) * .5f);
                            
                            edgeBC = index + 3 + reverse + offset++;
                            edgePoints.Add(check, edgeBC);
                        }
                        check = new Vector2Int(c, a);
                        if (edgePoints.TryGetValue(check, out edgeCA))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertC + vertA) * .5f);
                            writeNorms.Add((normC + normA).normalized);
                            writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                            writeCols.Add((colC + colA) * .5f);
                            writeUVs.Add((uvC + uvA) * .5f);
                            
                            edgeCA = index + 3 + reverse + offset;
                            edgePoints.Add(check, edgeCA);
                        }

                        writeTris.Add(indexA);
                        writeTris.Add(indexB);
                        writeTris.Add(edgeBC);

                        writeTris.Add(edgeBC);
                        writeTris.Add(indexC);
                        writeTris.Add(edgeCA);

                        writeTris.Add(edgeCA);
                        writeTris.Add(indexA);
                        writeTris.Add(edgeBC);

                        index += 5 + reverse;
                        continue;
                    }

                    if (shortBC)
                    {
                        offset = 0;
                        check = new Vector2Int(c, a);
                        if (edgePoints.TryGetValue(check, out edgeCA))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertC + vertA) * .5f);
                            writeNorms.Add((normC + normA).normalized);
                            writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                            writeCols.Add((colC + colA) * .5f);
                            writeUVs.Add((uvC + uvA) * .5f);
                            
                            edgeCA = index + 3 + reverse + offset++;
                            edgePoints.Add(check, edgeCA);
                        }
                        check = new Vector2Int(a, b);
                        if (edgePoints.TryGetValue(check, out edgeAB))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertA + vertB) * .5f);
                            writeNorms.Add((normA + normB).normalized);
                            writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                            writeCols.Add((colA + colB) * .5f);
                            writeUVs.Add((uvA + uvB) * .5f);
                            
                            edgeAB = index + 3 + reverse + offset;
                            edgePoints.Add(check, edgeAB);
                        }

                        writeTris.Add(indexB);
                        writeTris.Add(indexC);
                        writeTris.Add(edgeCA);

                        writeTris.Add(edgeCA);
                        writeTris.Add(indexA);
                        writeTris.Add(edgeAB);

                        writeTris.Add(edgeAB);
                        writeTris.Add(indexB);
                        writeTris.Add(edgeCA);

                        index += 5 + reverse;
                        continue;
                    }

                    if (shortCA)
                    {
                        offset = 0;
                        check = new Vector2Int(a, b);
                        if (edgePoints.TryGetValue(check, out edgeAB))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertA + vertB) * .5f);
                            writeNorms.Add((normA + normB).normalized);
                            writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                            writeCols.Add((colA + colB) * .5f);
                            writeUVs.Add((uvA + uvB) * .5f);
                            
                            edgeAB = index + 3 + reverse + offset++;
                            edgePoints.Add(check, edgeAB);
                        }
                        check = new Vector2Int(b, c);
                        if (edgePoints.TryGetValue(check, out edgeBC))
                            reverse--;
                        else
                        {
                            writeVerts.Add((vertB + vertC) * .5f);
                            writeNorms.Add((normB + normC).normalized);
                            writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                            writeCols.Add((colB + colC) * .5f);
                            writeUVs.Add((uvB + uvC) * .5f);
                            
                            edgeBC = index + 3 + reverse + offset;
                            edgePoints.Add(check, edgeBC);
                        }

                        writeTris.Add(indexC);
                        writeTris.Add(indexA);
                        writeTris.Add(edgeAB);

                        writeTris.Add(edgeAB);
                        writeTris.Add(indexB);
                        writeTris.Add(edgeBC);

                        writeTris.Add(edgeBC);
                        writeTris.Add(indexC);
                        writeTris.Add(edgeAB);

                        index += 5 + reverse;
                        continue;
                    }
                }

                offset = 0;
                check = new Vector2Int(a, b);
                if (edgePoints.TryGetValue(check, out edgeAB))
                    reverse--;
                else
                {
                    writeVerts.Add((vertA + vertB) * .5f);
                    writeNorms.Add((normA + normB).normalized);
                    writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                    writeCols.Add((colA + colB) * .5f);
                    writeUVs.Add((uvA + uvB) * .5f);
                            
                    edgeAB = index + 3 + reverse + offset++;
                    edgePoints.Add(check, edgeAB);
                }
                check = new Vector2Int(b, c);
                if (edgePoints.TryGetValue(check, out edgeBC))
                    reverse--;
                else
                {
                    writeVerts.Add((vertB + vertC) * .5f);
                    writeNorms.Add((normB + normC).normalized);
                    writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                    writeCols.Add((colB + colC) * .5f);
                    writeUVs.Add((uvB + uvC) * .5f);
                            
                    edgeBC = index + 3 + reverse + offset++;
                    edgePoints.Add(check, edgeBC);
                }
                check = new Vector2Int(c, a);
                if (edgePoints.TryGetValue(check, out edgeCA))
                    reverse--;
                else
                {
                    writeVerts.Add((vertC + vertA) * .5f);
                    writeNorms.Add((normC + normA).normalized);
                    writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                    writeCols.Add((colC + colA) * .5f);
                    writeUVs.Add((uvC + uvA) * .5f);
                            
                    edgeCA = index + 3 + reverse + offset;
                    edgePoints.Add(check, edgeCA);
                }

                //  Indexes  //
                writeTris.Add(indexA);
                writeTris.Add(edgeAB);
                writeTris.Add(edgeCA);

                writeTris.Add(indexB);
                writeTris.Add(edgeBC);
                writeTris.Add(edgeAB);

                writeTris.Add(indexC);
                writeTris.Add(edgeCA);
                writeTris.Add(edgeBC);

                writeTris.Add(edgeAB);
                writeTris.Add(edgeBC);
                writeTris.Add(edgeCA);

                index += 6 + reverse;
            }

            Mesh newMesh = new Mesh {name = mesh.name + "_" + (1 + i), indexFormat = index <= 65535? IndexFormat.UInt16 : IndexFormat.UInt32};
            newMesh.SetVertices(writeVerts);
            newMesh.SetTriangles(writeTris, 0);
            newMesh.SetNormals(writeNorms);
            newMesh.SetTangents(writeTans);
            newMesh.SetColors(writeCols);
            newMesh.SetUVs(0, writeUVs);
            newMesh.bounds = bounds;
            
            meshCollect.Add(newMesh);
        }
        
        // Debug.LogFormat("Skipped 1: {0} | Skipped 2:  {1} | Skipped 3: {2}", skip1, skip2, skip3);
        return meshCollect;
    }
    
    
    /*public static List<Mesh> Subdivide2(Mesh mesh, float side, int subdivisions)
    {
        meshCollect.Clear();
        
        mesh.GetTriangles(trisB, 0);
        mesh.GetVertices(vertsB);
        mesh.GetNormals(normsB);
        mesh.GetTangents(tansB);
        mesh.GetColors(colsB);
        mesh.GetUVs(0, uvsB);
        
        int     a, b, c;
        Vector3 vertA, vertB, vertC;
        Vector3 normA, normB, normC;
        Vector4 tanA, tanB, tanC;
        Color   colA, colB, colC;
        Vector2 uvA, uvB, uvC;
        bool shortAB, shortBC, shortCA;
        
        float sideLength;
        subdivisions++;
        int iteration = 0;
        for (int i = 0; i < subdivisions; i++)
        {
            while (true)
            {
                sideLength = Mathf.Pow((side / (i * 2f)) * .99f, 2);

                bool bToA = iteration++ % 2 == 0;
                List<int> readTris = bToA ? trisB : trisA, writeTris = bToA ? trisA : trisB;
                List<Vector3> readVerts = bToA ? vertsB : vertsA, writeVerts = bToA ? vertsA : vertsB;
                List<Vector3> readNorms = bToA ? normsB : normsA, writeNorms = bToA ? normsA : normsB;
                List<Vector4> readTans = bToA ? tansB : tansA, writeTans = bToA ? tansA : tansB;
                List<Color> readCols = bToA ? colsB : colsA, writeCols = bToA ? colsA : colsB;
                List<Vector2> readUVs = bToA ? uvsB : uvsA, writeUVs = bToA ? uvsA : uvsB;

                writeTris.Clear();
                writeVerts.Clear();
                writeNorms.Clear();
                writeTans.Clear();
                writeCols.Clear();
                writeUVs.Clear();

                int index = 0;
                int triCount = readTris.Count;
                for (int t = 0; t < triCount; t += 3)
                {
                    a = readTris[t];
                    b = readTris[t + 1];
                    c = readTris[t + 2];

                    vertA = readVerts[a];
                    vertB = readVerts[b];
                    vertC = readVerts[c];

                    normA = readNorms[a];
                    normB = readNorms[b];
                    normC = readNorms[c];

                    tanA = readTans[a];
                    tanB = readTans[b];
                    tanC = readTans[c];

                    colA = readCols[a];
                    colB = readCols[b];
                    colC = readCols[c];

                    uvA = readUVs[a];
                    uvB = readUVs[b];
                    uvC = readUVs[c];

                    shortAB = (vertA - vertB).sqrMagnitude < sideLength;
                    shortBC = (vertB - vertC).sqrMagnitude < sideLength;
                    shortCA = (vertC - vertA).sqrMagnitude < sideLength;


                    //  These Always Have To Be Written
                    writeVerts.Add(vertA);
                    writeVerts.Add(vertB);
                    writeVerts.Add(vertC);

                    writeNorms.Add(normA);
                    writeNorms.Add(normB);
                    writeNorms.Add(normC);

                    writeTans.Add(tanA);
                    writeTans.Add(tanB);
                    writeTans.Add(tanC);

                    writeCols.Add(colA);
                    writeCols.Add(colB);
                    writeCols.Add(colC);

                    writeUVs.Add(uvA);
                    writeUVs.Add(uvB);
                    writeUVs.Add(uvC);

                    
                    //  All Good  //
                    if (shortAB && shortBC && shortCA)
                    {
                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);

                        index += 3;
                        continue;
                    }

                    //  One Long Edge  //
                    if (shortAB && shortBC)
                    {
                        writeVerts.Add((vertC + vertA) * .5f);
                        writeNorms.Add((normC + normA).normalized);
                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                        writeCols.Add((colC + colA) * .5f);
                        writeUVs.Add((uvC + uvA) * .5f);

                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);

                        index += 4;
                        continue;
                    }

                    if (shortBC && shortCA)
                    {
                        writeVerts.Add((vertA + vertB) * .5f);
                        writeNorms.Add((normA + normB).normalized);
                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                        writeCols.Add((colA + colB) * .5f);
                        writeUVs.Add((uvA + uvB) * .5f);

                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 2);
                        writeTris.Add(index);

                        index += 4;
                        continue;
                    }

                    if (shortCA && shortAB)
                    {
                        writeVerts.Add((vertB + vertC) * .5f);
                        writeNorms.Add((normB + normC).normalized);
                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                        writeCols.Add((colB + colC) * .5f);
                        writeUVs.Add((uvB + uvC) * .5f);

                        writeTris.Add(index + 2);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index);
                        writeTris.Add(index + 1);

                        index += 4;
                        continue;
                    }

                    if (shortAB)
                    {
                        writeVerts.Add((vertB + vertC) * .5f);
                        writeVerts.Add((vertC + vertA) * .5f);

                        writeNorms.Add((normB + normC).normalized);
                        writeNorms.Add((normC + normA).normalized);

                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);

                        writeCols.Add((colB + colC) * .5f);
                        writeCols.Add((colC + colA) * .5f);

                        writeUVs.Add((uvB + uvC) * .5f);
                        writeUVs.Add((uvC + uvA) * .5f);

                        writeTris.Add(index);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }

                    if (shortBC)
                    {
                        writeVerts.Add((vertC + vertA) * .5f);
                        writeVerts.Add((vertA + vertB) * .5f);

                        writeNorms.Add((normC + normA).normalized);
                        writeNorms.Add((normA + normB).normalized);

                        writeTans.Add((tanC * .5f + tanA * .5f).normalized);
                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);

                        writeCols.Add((colC + colA) * .5f);
                        writeCols.Add((colA + colB) * .5f);

                        writeUVs.Add((uvC + uvA) * .5f);
                        writeUVs.Add((uvA + uvB) * .5f);

                        writeTris.Add(index + 1);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }

                    if (shortCA)
                    {
                        writeVerts.Add((vertA + vertB) * .5f);
                        writeVerts.Add((vertB + vertC) * .5f);

                        writeNorms.Add((normA + normB).normalized);
                        writeNorms.Add((normB + normC).normalized);

                        writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                        writeTans.Add((tanB * .5f + tanC * .5f).normalized);

                        writeCols.Add((colA + colB) * .5f);
                        writeCols.Add((colB + colC) * .5f);

                        writeUVs.Add((uvA + uvB) * .5f);
                        writeUVs.Add((uvB + uvC) * .5f);

                        writeTris.Add(index + 2);
                        writeTris.Add(index);
                        writeTris.Add(index + 3);

                        writeTris.Add(index + 3);
                        writeTris.Add(index + 1);
                        writeTris.Add(index + 4);

                        writeTris.Add(index + 4);
                        writeTris.Add(index + 2);
                        writeTris.Add(index + 3);

                        index += 5;
                        continue;
                    }
                    

                    writeVerts.Add((vertA + vertB) * .5f);
                    writeVerts.Add((vertB + vertC) * .5f);
                    writeVerts.Add((vertC + vertA) * .5f);

                    writeNorms.Add((normA + normB).normalized);
                    writeNorms.Add((normB + normC).normalized);
                    writeNorms.Add((normC + normA).normalized);

                    writeTans.Add((tanA * .5f + tanB * .5f).normalized);
                    writeTans.Add((tanB * .5f + tanC * .5f).normalized);
                    writeTans.Add((tanC * .5f + tanA * .5f).normalized);

                    writeCols.Add((colA + colB) * .5f);
                    writeCols.Add((colB + colC) * .5f);
                    writeCols.Add((colC + colA) * .5f);

                    writeUVs.Add((uvA + uvB) * .5f);
                    writeUVs.Add((uvB + uvC) * .5f);
                    writeUVs.Add((uvC + uvA) * .5f);

                    //  Indexes  //
                    writeTris.Add(index);
                    writeTris.Add(index + 3);
                    writeTris.Add(index + 5);

                    writeTris.Add(index + 1);
                    writeTris.Add(index + 4);
                    writeTris.Add(index + 3);

                    writeTris.Add(index + 2);
                    writeTris.Add(index + 5);
                    writeTris.Add(index + 4);

                    writeTris.Add(index + 3);
                    writeTris.Add(index + 4);
                    writeTris.Add(index + 5);

                    index += 6;
                }

                bool allGood = true;
                
                if (i == 0)
                {
                    triCount = writeTris.Count;
                    for (int t = 0; t < triCount; t += 3)
                    {
                        a = writeTris[t];
                        b = writeTris[t + 1];
                        c = writeTris[t + 2];

                        vertA = writeVerts[a];
                        vertB = writeVerts[b];
                        vertC = writeVerts[c];
                        
                        shortAB = (vertA - vertB).sqrMagnitude < sideLength;
                        shortBC = (vertB - vertC).sqrMagnitude < sideLength;
                        shortCA = (vertC - vertA).sqrMagnitude < sideLength;

                        if (!shortAB || !shortBC || !shortCA)
                        {
                            allGood = false;
                            Debug.Log("DoingItAgain");
                            break;
                        }
                    }
                }

                if (allGood)
                {
                    Mesh newMesh = new Mesh {name = mesh.name, indexFormat = index <= 65535 ? IndexFormat.UInt16 : IndexFormat.UInt32};
                    newMesh.SetVertices(writeVerts);
                    newMesh.SetTriangles(writeTris, 0);
                    newMesh.SetNormals(writeNorms);
                    newMesh.SetTangents(writeTans);
                    newMesh.SetColors(writeCols);
                    newMesh.SetUVs(0, writeUVs);
                    newMesh.RecalculateBounds();

                    meshCollect.Add(newMesh);
                    
                    break;
                }
            }
        }
        
        // Debug.LogFormat("Skipped 1: {0} | Skipped 2:  {1} | Skipped 3: {2}", skip1, skip2, skip3);
        return meshCollect;
    }*/
}
