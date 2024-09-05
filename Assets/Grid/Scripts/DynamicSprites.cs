using System;
using System.Runtime.InteropServices;
using System.Text;
using GeoMath;
using GridData;
using UnityEngine;
using UnityEngine.Profiling;


public class DynamicSprites : MonoBehaviour
{
    public Material[] mat;
    
    private Mesh mesh;
    
    private static readonly int ItemInfoBuffer = Shader.PropertyToID("ItemInfoBuffer");
    private static readonly int RenderBuffer   = Shader.PropertyToID("RenderBuffer");

    private const int maxCount = 4096 * 4;
    
    private readonly uint[] argsData = new uint[5];
    private ComputeBuffer argsBuffer;
    private readonly Bounds worldBounds = new Bounds(Vector3.zero, Vector3.one * 100000);

    private static readonly ScatterObject[] scatterObj    = new ScatterObject[maxCount];
    private static readonly Bounds2D[]      scatterBounds = new Bounds2D[maxCount];
    private ComputeBuffer rB;

    private static int renderCount;
    private static bool Draw;
    private static readonly int Selection = Shader.PropertyToID("Selection");

    public delegate void SpriteCollect();
    public static event SpriteCollect OnSpriteCollect; 
    
    
    private void Start()
    {
        mesh = new Mesh
        {
            vertices  = new[] { Vector3.zero, Vector3.up, new Vector3(1, 1, 0), Vector3.right },
            triangles = new[] { 0, 1, 2, 2, 3, 0 }
        };
        
        
        argsBuffer = Buff.Add(new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments));
        
        argsData[0] = mesh.GetIndexCount(0);
        argsData[2] = mesh.GetIndexStart(0);
        argsData[3] = mesh.GetBaseVertex(0);

        rB = Buff.New(scatterObj, Marshal.SizeOf<ScatterObject>());

        for (int i = 0; i < 2; i++)
        {
            Material m = mat[i];
            m = Instantiate(m);
            m.SetBuffer(RenderBuffer, rB);
            m.SetBuffer(ItemInfoBuffer, Buff.New(ItemInfo.GetData(i), Marshal.SizeOf<ItemData>()));
            mat[i] = m;
        }
        
        DebugUI.TL += DebugUiOnTl;
    }

    
    private void DebugUiOnTl(StringBuilder builder)
    {
        builder.AppendLine(renderCount.PrepString());
    }


    public static void RenderThis(ScatterObject sO)
    {
        Profiler.BeginSample("RenderThis");
        Bounds2D bounds = sO.GetBounds;
        
        if (GridCam.Bounds.Intersects(bounds))
        {
            scatterObj[renderCount] = sO;
            scatterBounds[renderCount++] = bounds;
        }
        Profiler.EndSample();
    }
    
    
    public static void RenderThisBounds(ScatterObject sO, Bounds2D bounds)
    {
        Profiler.BeginSample("RenderThisBounds");
        if (GridCam.Bounds.Intersects(bounds))
        {
            scatterObj[renderCount] = sO;
            scatterBounds[renderCount++] = bounds;
        }
        Profiler.EndSample();
    }
    
    
    public static void RenderThisForced(ScatterObject sO, Bounds2D bounds)
    {
        Profiler.BeginSample("RenderThisForced");
        scatterObj[renderCount] = sO;
        scatterBounds[renderCount++] = bounds;
        Profiler.EndSample();
    }
    
    
    public static void RenderThisTile(ScatterObject[] sO, Bounds2D[] bounds)
    {
        Profiler.BeginSample("RenderThisTile");
        int count = sO.Length;
        Array.Copy(sO, 0, scatterObj, renderCount, count);
        Array.Copy(bounds, 0, scatterBounds, renderCount, count);
        renderCount += count;
        Profiler.EndSample();
    }
    
    
    private void LateUpdate()
    {
        renderCount = 0;

        OnSpriteCollect?.Invoke();

        argsData[1] = (uint) renderCount;
        argsBuffer.SetData(argsData);

        rB.SetData(scatterObj);

        for (int i = 0; i < 2; i++)
            Graphics.DrawMeshInstancedIndirect(
                mesh, 0, mat[i], worldBounds, argsBuffer, 0);

        if (Input.GetKeyDown(KeyCode.P))
            Draw = !Draw;

        CursorDebug();
    }


    private static void CursorDebug()
    {
        float best = float.MaxValue;
        Color c = COLOR.red.tomato;
        Vector2 cursor = GridCam.CursorCoords.V2UseZ();
        int count = Mathf.Min(maxCount, renderCount);
        int pick = -1;
        
        if(false)
            for (int i = 0; i < count; i++)
            {
                ScatterObject sO = scatterObj[i];
                
                if(sO.pos.z > best)
                    continue;
    
                Bounds2D bounds = scatterBounds[i];
    
                if (bounds.Contains(cursor))
                {
                    Vector2 lerps = bounds.GetUV(cursor);
                    ItemData data = ItemInfo.itemData[sO.id];
                    Vector2 uv = new Vector2(Mathf.Lerp(data.uvMin.x, data.uvMax.x, 1 - lerps.x), Mathf.Lerp(data.uvMin.y, data.uvMax.y, 1 - lerps.y));
                    float alpha = ItemInfo.GetAlpha(uv);
                    if (alpha > .25f)
                    {
                        best = sO.pos.z;
                        pick = i;
                    }
                }   
            }

        Shader.SetGlobalInt(Selection, pick);
        
        if(pick == -1)
            return;
        
        if(Input.GetMouseButtonDown(0))
            Debug.Log(ItemInfo.GetSpriteItemName(scatterObj[pick].id));
        
        if (Draw)
            scatterBounds[pick].Draw(c).SetDepth(-2);
    }
}
