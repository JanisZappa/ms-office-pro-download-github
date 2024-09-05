using System;
using System.Collections.Generic;
using GeoMath;
using TMPro;
using UnityEngine;


public partial class DRAW : MonoBehaviour
{
    private static Camera setCamera;
    public static Camera DrawCam
    {
        get
        {
            if ( setCamera == null && Camera.main == null )
                return null;

            Camera cam = setCamera != null ? setCamera : Camera.main;
            if (inst == null)
            {
                inst = cam.gameObject.AddComponent<DRAW>();
            }

            return cam;
        }
        set
        {
            setCamera = value;
            
            if (inst != null)
                Destroy(inst);
            
            inst = setCamera.gameObject.AddComponent<DRAW>();
        }
    }

    private static Camera textCam;
    private const int TextLayer = 5;

    private static TMP_FontAsset DrawFont{ get { return Resources.Load<TMP_FontAsset>("Font/UI"); }}

    private static bool _enabled;
    public  static bool Enabled 
    { 
        set { _enabled = value; EnableOrDisable(); } 
        get { return _enabled; }
    }

    private static bool _editorDraw;
    public  static bool EditorDraw { set { _editorDraw = value && Application.isEditor; }}

    private static bool _drawStuff;
    private static bool DrawStuff { get { return _drawStuff && DrawCam != null; }}

    
    private static void EnableOrDisable()
    {
        if ( _enabled != _drawStuff )
        {
            _drawStuff = _enabled;

            shapeJobs.Clear();
            textJobs.Clear();
            
            if (_drawStuff)
                Shape.CreatePools();
            else
            {
                DestroyTextStuff();
                Destroy(inst);
                Shape.DestroyPools();
            }
        }
    }


    private static DRAW   inst;
    private static Canvas canvas;


    private static readonly List<Shape>       shapeJobs = new List<Shape>(1000);
    private static readonly List<OverlayText> textJobs  = new List<OverlayText>(1000);
    private static readonly List<GUIText>     guiJobs  = new List<GUIText>(1000);

    public static bool SomethingToDRAW
    {
        get { return shapeJobs.Count != 0; }
    }
    
    
    #region Setup

    private static bool cleanUp;

    public static readonly Shape NullShape = new Shape(0, null);
    
    private static void CreateTextStuff()
    {
        textCam = new GameObject("DRAW_TextCam").AddComponent<Camera>();
        textCam.depth = 100;
        textCam.clearFlags = CameraClearFlags.Depth;
        textCam.cullingMask = 1 << TextLayer;
        
        GameObject canvasObject = new GameObject { name = "DRAW_Text_Canvas" };
                   canvasObject.transform.SetParent(textCam.transform);
        
        canvas               = canvasObject.AddComponent<Canvas>();
        canvas.renderMode    = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera   = textCam;
        canvas.planeDistance = textCam.nearClipPlane + .01f;
        
        canvas.gameObject.layer = TextLayer;

        OverlayText.SetAllActive(true);
    }


    private static void DestroyTextStuff()
    {
        if(textCam != null)
            Destroy(textCam.gameObject);
        
        OverlayText.SetAllActive(false);
       // GUIText.SetAllActive(false);
        
        if(canvas != null)
            Destroy(canvas.gameObject);
    }
    
    
    #endregion
    

    public static void Text(string text, Vector3 pos, Color color, float size = 1, float z = 0, Vector2 offset = default(Vector2), bool screenSpace = false)
    {
        if(!DrawStuff)
            return;
        
        if(canvas == null)
            CreateTextStuff();
        
        pos = Mathf.Approximately(z, 0) ? pos : new Vector3(pos.x, pos.y, z);
        if (!screenSpace && !OnScreen(pos))
            return;

        OverlayText newText = OverlayText.Get();
        if (newText == null)
            return;
        
        newText.text.text                    = text;
        newText.text.fontSize                = TextScale(size);
        newText.text.color                   = color == default(Color)? Color.white : color;
        newText.text.rectTransform.sizeDelta = new Vector2(newText.text.preferredWidth, newText.text.preferredHeight);
     
        newText.offset        = offset;
        newText.pos           = pos;
        newText.frame         = Time.frameCount;
        newText.screenSpace   = screenSpace;
        
        textJobs.Add(newText);
    }


    public static void GUI_Text(string text, Vector3 pos, Color color, float size = 1)
    {
        if(!DrawStuff)
            return;
        
        if(!GUIText.IsActive)
            GUIText.SetAllActive(true);
        
        GUIText t = GUIText.Get();
        if (t != null)
        {
            Vector2 p = DrawCam.WorldToViewportPoint(pos);
            guiJobs.Add(t.Set(text, new Vector2(p.x * Screen.width, (1 - p.y) * Screen.height), size, color));
        }  
    }


    public static void GUI_Text(string text, Vector3 pos, float size = 1)
    {
        GUI_Text(text, pos, Color.white, size);
    }
    
    
    #region Line

    public static Shape Ray(Vector3 pointA, Vector3 pointB)
    {
        if ( !DrawStuff )
            return NullShape;

        Vector3 dir = pointB - pointA;
        
        return Shape.Get(2).SetVectorPoints(pointA - dir * 100000, pointB + dir * 100000);
    }
    
    #endregion


    #region Vector
    public static Shape Vector(Vector3 point, Vector3 direction)
    {
        return !DrawStuff ? NullShape : Shape.Get(2).SetVectorPoints(point, point + direction);
    }

    
    public static Shape GapVector(Vector3 point, Vector3 direction, int gaps)
    {
        if ( !DrawStuff )
            return NullShape;

        int total = gaps * 2 + 1;
        Vector3 segmentDir = direction / total;
        
        Shape oldVector = null;
        Shape vector = null;
        for (int i = 0; i < total; i+=2)
        {
            float lerpA = (float) i / total;
            vector = Vector(Vector3.Lerp(point, point + direction, lerpA), segmentDir).SetSub(oldVector);
            oldVector = vector;
        }
        
        return vector;
    }
    
    
    public static Shape DotVector(Vector3 point, Vector3 direction, float radiusA, float radiusB, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        Shape dots = Circle(point, radiusA, segments).SetSub(Circle(point + direction, radiusB, segments));

        float vectorLength = direction.magnitude - radiusA - radiusB;
        return vectorLength > 0 ? 
            Vector(point + direction.normalized * radiusA, direction.normalized * vectorLength).SetSub(dots) : dots;
    }
    
   
    public static Shape Zapp(Vector3 point, Vector3 direction, int segments, float width, float length)
    {
        if ( !DrawStuff )
            return NullShape;

        Shape zapp = Shape.Get(segments);
        for (int i = 0; i < segments; i++)
            zapp[i] = GetZapp(i, point, direction, segments, width, length);

        return zapp;
    }
    #endregion
    

    private static Vector3 GetZapp(int i, Vector3 point, Vector3 direction, int segments, float width, float length)
    {
        Vector3 right = Quaternion.AngleAxis(90, Vector3.forward) * direction.normalized * width;
        float stretch = length / direction.magnitude;
        return Vector3.Lerp(point, point + direction, (float) i / (segments - 1))
               + Vector3.Lerp(-right, right, Mathf.PingPong(i, 1))
               * Mathf.Clamp(stretch * (Mathf.Approximately(Mathf.PingPong((float) i / (segments - 1) * 2, 1) % 1, 0) ? 0 : 1), 0, width * 10);
    }
    

#region Arrow
    public static Shape Arrow(Vector3 point, Vector3 direction, float size, bool flip = false)
    {
        if ( !DrawStuff )
            return NullShape;

        float mag = direction.magnitude;
        if (size > mag)
            size = mag;
        
        Vector3 arrowClamp = direction.normalized * size * (!flip ? -1 : 1);
        
        Vector3 start = !flip ? point : point + direction;
        Vector3 end = (!flip ? point + direction : point) + arrowClamp;

        Shape line = Shape.Get(2).SetVectorPoints(start, end);
       
        
    //  Tip  //
        Vector3 tip = !flip ? point + direction : point;
        float tipSide = size / Mathf.Cos(30 * Mathf.Deg2Rad); 
        Vector3 wing = (!flip ? -direction : direction).normalized * tipSide;

        Shape tipShape = Shape.Get(4);
        
        tipShape[0] = tip;
        tipShape[1] = tip + Quaternion.AngleAxis(30, Vector3.forward) * wing;
        tipShape[2] = tip + Quaternion.AngleAxis(-30, Vector3.forward) * wing;
        tipShape[3] = tip;
        
        return tipShape.SetSub(line);
    }
    
    
    public static Shape ZappArrow(Vector3 point, Vector3 direction, float size, int segments, float width, float length, bool flip = false)
    {
        if ( !DrawStuff )
            return NullShape;

        float mag = direction.magnitude;
        if (size > mag)
            size = mag;
        
        Vector3 arrowClamp = direction.normalized * size * (!flip ? -1 : 1);
        
        Vector3 start = !flip ? point : point + direction;
        Vector3 end = (!flip ? point + direction : point) + arrowClamp;

        Shape line = Zapp(start, end - start, segments, width, length);
       
        
        //    Tip    //
        Vector3 tip = !flip ? point + direction : point;
        float tipSide = size / Mathf.Cos(30 * Mathf.Deg2Rad); 
        Vector3 wing = (!flip ? -direction : direction).normalized * tipSide;

        Shape tipShape = Shape.Get(4);
        
        tipShape[0] = tip;
        tipShape[1] = tip + Quaternion.AngleAxis(30, Vector3.forward) * wing;
        tipShape[2] = tip + Quaternion.AngleAxis(-30, Vector3.forward) * wing;
        tipShape[3] = tip;
        
        return tipShape.SetSub(line);
    }
    
    
    public static Shape TwoWayArrow(Vector3 point, Vector3 direction, float size)
    {
        if ( !DrawStuff )
            return NullShape;

        float mag = direction.magnitude;
        if (size * 2 > mag)
            size = mag * .5f;

        
        Vector3 start = point + direction.normalized * size;
        Vector3 end   =  point + direction - direction.normalized * size;

        Shape line = Shape.Get(2).SetVectorPoints(start, end);
       
        
        //    Tip A   //
        Shape tipShapeA = Shape.Get(4);
        {
            Vector3 tip     = point + direction;
            float   tipSide = size / Mathf.Cos(30 * Mathf.Deg2Rad);
            Vector3 wing    = -direction.normalized * tipSide;

            tipShapeA[0] = tip;
            tipShapeA[1] = tip + Quaternion.AngleAxis(30, Vector3.forward) * wing;
            tipShapeA[2] = tip + Quaternion.AngleAxis(-30, Vector3.forward) * wing;
            tipShapeA[3] = tip;
        }
        //    Tip B   //
        Shape tipShapeB = Shape.Get(4);
        {
            Vector3 tip     = point;
            float   tipSide = size / Mathf.Cos(30 * Mathf.Deg2Rad);
            Vector3 wing    = direction.normalized * tipSide;

            tipShapeB[0] = tip;
            tipShapeB[1] = tip + Quaternion.AngleAxis(30, Vector3.forward) * wing;
            tipShapeB[2] = tip + Quaternion.AngleAxis(-30, Vector3.forward) * wing;
            tipShapeB[3] = tip;
        }
        
        tipShapeA.SetSub(line);

        return tipShapeB.SetSub(tipShapeA);
    }
    
    
#endregion

    
    
    
#region Circle
    public static Shape Circle(Vector3 pos, float radius, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        if (segments == 0)
            segments = GetArcSegments(radius);

        return Shape.Get(segments).SetCirclePoints(pos, radius);
    }
    
    
    public static Shape Ellipse(Vector3 pos, float radius, float yMulti, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        if (segments == 0)
            segments = GetArcSegments(radius);

        return Shape.Get(segments).SetCirclePoints(pos, radius, yMulti);
    }
    
    
    public static Shape MultiCircle(Vector3 pos, float radius, int count, float ringDistance, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        ringDistance = Mathf.Min(ringDistance, radius / count);
        
        //count = (int)Mathf.Clamp(radius / ringDistance, 0, count);

        Shape oldCircle = null;
        Shape circle = null;
        for (int i = 0; i < count; i++)
        {
            circle = Circle(pos, radius - i * ringDistance, segments).SetSub(oldCircle);
            oldCircle = circle;
        }

        return circle;
    }
    
    
    public static Shape ZappCircle(Vector3 pos, float radius, float width, int segments = 0, float angle = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        if (segments == 0)
            segments = GetArcSegments(radius);
        
        if (segments % 2 == 0)
            segments++;

        return Shape.Get(segments).SetZappCirclePoints(pos, radius, width, segments, angle);
    }
    
    
    private static int GetArcSegments(float radius, float bend = 1)
    {
        const float circleSegmentFactor = 1.25f;
        return Mathf.CeilToInt(Mathf.Clamp(radius * 2 * Mathf.PI * circleSegmentFactor * bend, 8, float.MaxValue));
    }
#endregion


#region Sphere
    public static Shape Sphere(Vector3 pos, float radius, Quaternion rot, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;
    
        if (segments == 0)
            segments = GetArcSegments(radius);
        
        Shape aS = Shape.Get(segments).SetCirclePoints(pos, radius, rot);
        for (int i = 0; i < 7; i++)
        {
            Shape newShape = Shape.Get(segments).SetCirclePoints(pos, radius, rot * Quaternion.AngleAxis(22.5f + i * 22.5f, Vector3.right));
            newShape.SetSub(aS);
            aS = newShape;
        }
        Shape bS = Shape.Get(segments).SetCirclePoints(pos, radius, rot * Quaternion.AngleAxis(90, Vector3.right));
        return bS.SetSub(aS);
    }


#endregion
    
    
    
   
#region Arc
    public static Shape Arc(Vector3 pos, float radius, float bend, float angle = 0, int segments = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        if (segments == 0)
            segments = GetArcSegments(radius, bend);

        return Shape.Get(segments).SetArcPoints(pos, radius, angle, bend);
    }
#endregion
    
    
    
    
#region Rectangle
    public static Shape Rectangle(Vector3 pos, Vector2 dimensions, float angle = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        Shape rectShape = Shape.Get(5);
        
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 loopPoint = pos + rot * (Vector3.right * -.5f * dimensions.x + Vector3.up * .5f * dimensions.y);
        rectShape[0] = loopPoint;
        rectShape[1] = pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * .5f * dimensions.y);
        rectShape[2] = pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * -.5f * dimensions.y);
        rectShape[3] = pos + rot * (Vector3.right * -.5f * dimensions.x  + Vector3.up * -.5f * dimensions.y);
        rectShape[4] = loopPoint;

        return rectShape;
    }


    public static Shape GapRectangle(Vector3 pos, Vector2 dimensions, int gaps, float angle = 0)
    {
        if ( !DrawStuff )    return NullShape;

        
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3[] points =
        {
            pos + rot * (Vector3.right * -.5f * dimensions.x + Vector3.up * .5f * dimensions.y),
            pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * .5f * dimensions.y),
            pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * -.5f * dimensions.y),
            pos + rot * (Vector3.right * -.5f * dimensions.x  + Vector3.up * -.5f * dimensions.y)
        };

        int total = gaps * 2 + 1;
        Shape oldVector = null;
        Shape vector    = null;

        for (int i = 0; i < 4; i++)
        {
            Vector3 p1 = points[i], p2 = points[(i + 1) % 4];
            
            Vector3 segmentDir = (p2 - p1) / total;
            for (int e = 0; e < total; e += 2)
            {
                float lerpA = (float) e / total;
                vector = Vector(Vector3.Lerp(p1, p2, lerpA), segmentDir).SetSub(oldVector);
                oldVector = vector;
            }
        }

        return vector;
    }
    
    
    public static Shape ZappRectangle(Vector3 pos, Vector2 dimensions, int segments, float width, float length, float angle = 0)
    {
        if ( !DrawStuff )
            return NullShape;

        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        
        Vector3[] points =
        {
            pos + rot * (Vector3.right * -.5f * dimensions.x + Vector3.up * .5f * dimensions.y),
            pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * .5f * dimensions.y),
            pos + rot * (Vector3.right * .5f * dimensions.x  + Vector3.up * -.5f * dimensions.y),
            pos + rot * (Vector3.right * -.5f * dimensions.x  + Vector3.up * -.5f * dimensions.y)
        };

        Shape oldVector = null;
        Shape vector    = null;

        for (int i = 0; i < 4; i++)
        {
            Vector3 p1 = points[i], p2 = points[(i + 1) % 4];
            
            vector = Zapp(p1, p2 - p1, segments, width, length).SetSub(oldVector);
            oldVector = vector;
        }

        return vector;
    }
#endregion


#region Triangle
    public static Shape Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        if ( !DrawStuff )
            return NullShape;
    
        Shape rectShape = Shape.Get(4);
    
        rectShape[0] = a;
        rectShape[1] = b;
        rectShape[2] = c;
        rectShape[3] = a;
    
        return rectShape;
    }
#endregion


#region Box
public static Shape Box(Vector3 min, Vector3 max)
{
    if ( !DrawStuff )
        return NullShape;
    
        Vector3 a = new Vector3(min.x, min.y, min.z);
        Vector3 b = new Vector3(min.x, min.y, max.z);
        Vector3 c = new Vector3(max.x, min.y, max.z);
        Vector3 d = new Vector3(max.x, min.y, min.z);

        Vector3 e = new Vector3(min.x, max.y, min.z);
        Vector3 f = new Vector3(min.x, max.y, max.z);
        Vector3 g = new Vector3(max.x, max.y, max.z);
        Vector3 h = new Vector3(max.x, max.y, min.z);
    
    return Vector(a, b - a).SetSub(
           Vector(b, c - b).SetSub(
           Vector(c, d - c).SetSub(
           Vector(d, a - d).SetSub(
               
           Vector(a, e - a).SetSub(
           Vector(b, f - b).SetSub(
           Vector(c, g - c).SetSub(
           Vector(d, h - d).SetSub(
               
           Vector(e, f - e).SetSub(
           Vector(f, g - f).SetSub(
           Vector(g, h - g).SetSub(
           Vector(h, e - h))))))))))));
}

public static Shape Box(Vector3 center, Vector3 size, Quaternion rot)
{
    if ( !DrawStuff )
        return NullShape;
    
    Vector3 v = size * .5f;
    
    Vector3 a = center + rot * new Vector3(-v.x, -v.y, -v.z);
    Vector3 b = center + rot * new Vector3(-v.x, -v.y,  v.z);
    Vector3 c = center + rot * new Vector3( v.x, -v.y,  v.z);
    Vector3 d = center + rot * new Vector3( v.x, -v.y, -v.z);

    Vector3 e = center + rot * new Vector3(-v.x, v.y, -v.z);
    Vector3 f = center + rot * new Vector3(-v.x, v.y,  v.z);
    Vector3 g = center + rot * new Vector3( v.x, v.y,  v.z);
    Vector3 h = center + rot * new Vector3( v.x, v.y, -v.z);
    
    return Vector(a, b - a).SetSub(
           Vector(b, c - b).SetSub(
           Vector(c, d - c).SetSub(
           Vector(d, a - d).SetSub(
               
           Vector(a, e - a).SetSub(
           Vector(b, f - b).SetSub(
           Vector(c, g - c).SetSub(
           Vector(d, h - d).SetSub(
               
           Vector(e, f - e).SetSub(
           Vector(f, g - f).SetSub(
           Vector(g, h - g).SetSub(
           Vector(h, e - h))))))))))));
}
#endregion



public static Shape Grid(Vector3 pos, Vector2 dimensions, int xCells, int yCells, Bounds2D constraint)
{
    if ( !DrawStuff )
        return NullShape;

    xCells = Mathf.Max(xCells, 1);
    yCells = Mathf.Max(yCells, 1);

    Shape shape = Rectangle(pos, dimensions);
    Vector3 min = pos + dimensions.V3() * -.5f;
    float xStep = dimensions.x / xCells;
    float yStep = dimensions.y / yCells;
    
    bool insideBounds = constraint.Area > 0;

    for (int x = 1; x < xCells; x++)
    {
        Vector2 p = min + new Vector3(x * xStep, 0);
        if(!insideBounds || p.x >= constraint.minX && p.x <= constraint.maxX)
            shape = Vector(p, new Vector3(0, dimensions.y)).SetSub(shape);
    }


    for (int y = 1; y < yCells; y++)
    {
        Vector2 p = min + new Vector3(0, y * yStep);
        if(!insideBounds || p.y >= constraint.minY && p.y <= constraint.maxY)
            shape = Vector(p, new Vector3(dimensions.x, 0)).SetSub(shape);
    }
        

    return shape;
}

    
    
    
#region Line
    public static Shape Line(Vector2[] points)
    {
        return Shape.Get(points.Length).SetPoints(points);
    }
    
    
    public static Shape Line(Vector3[] points)
    {
        return Shape.Get(points.Length).SetPoints(points);
    }
    
    
    public static Shape Line(int pointCount, Func<int, Vector3> pointFunc)
    {
        return Shape.Get(pointCount).SetPoints(pointFunc);
    }
    
#endregion




#region Mesh
    public static Shape Polygons(Mesh mesh, Transform transform = null)
    {
        return Polygons(mesh.vertices, mesh.triangles, transform);
    }


    public static Shape Polygons(Vector3[] vertices, int[] triangles, Transform transform = null)
    {
        Shape oldVector = null;
        Shape vector    = null;
        
        for (int i = 0; i < triangles.Length; i += 3)
        {
            {
                vector = transform != null ? 
                    Vector(transform.TransformPoint(vertices[triangles[i]]), transform.TransformPoint(vertices[triangles[i + 1]] - vertices[triangles[i]])).SetSub(oldVector) 
                    : Vector(vertices[triangles[i]], vertices[triangles[i + 1]] - vertices[triangles[i]]).SetSub(oldVector);
                oldVector = vector;
            }
            {
                vector = transform != null ? 
                    Vector(transform.TransformPoint(vertices[triangles[i + 1]]), transform.TransformPoint(vertices[triangles[i + 2]] - vertices[triangles[i + 1]])).SetSub(oldVector) 
                    : Vector(vertices[triangles[i + 1]], vertices[triangles[i + 2]] - vertices[triangles[i + 1]]).SetSub(oldVector);
                oldVector = vector;
            }
            {
                vector = transform != null ? 
                    Vector(transform.TransformPoint(vertices[triangles[i + 2]]), transform.TransformPoint(vertices[triangles[i]] - vertices[triangles[i + 2]])).SetSub(oldVector) 
                    : Vector(vertices[triangles[i + 2]], vertices[triangles[i]] - vertices[triangles[i + 2]]).SetSub(oldVector);
                oldVector = vector;
            }
        }
        
        return vector;
    }


#endregion



#region Screen

    public static Shape ScreenPoint(Vector2 screenPos, float radius, int segments)
    {
            if ( !DrawStuff )
                return NullShape;

            Camera cam = DrawCam;
            Vector3 clipPos   = cam.ScreenToWorldPoint(screenPos.V3(DrawCam.nearClipPlane + .1f));
            Quaternion camRot = cam.transform.rotation;

            Shape s = Shape.Get(segments);
            if(s == null)
                Debug.Log("WTF " + segments);
            
            return s.SetupScreenPoint(clipPos, radius, camRot);
    }

    
    public static Shape ScreenRect(Vector2 screenPos, Vector2 dimensions)
    {
        if ( !DrawStuff )
            return NullShape;
        
        
        Shape rectShape = Shape.Get(5);

        float w = Screen.width, h = Screen.height;
        
        Vector2 lerpPos  = new Vector2(screenPos.x / w,  screenPos.y / h);
        Vector2 lerpSize = new Vector2(dimensions.x / w, dimensions.y / h);
        
        Vector3 loopPoint = ClipPlaneLerp(lerpPos.x - lerpSize.x * .5f, lerpPos.y - lerpSize.y * .5f);
        rectShape[0] = loopPoint;
        rectShape[1] = ClipPlaneLerp(lerpPos.x - lerpSize.x * .5f, lerpPos.y + lerpSize.y * .5f);
        rectShape[2] = ClipPlaneLerp(lerpPos.x + lerpSize.x * .5f, lerpPos.y + lerpSize.y * .5f);
        rectShape[3] = ClipPlaneLerp(lerpPos.x + lerpSize.x * .5f, lerpPos.y - lerpSize.y * .5f);
        rectShape[4] = loopPoint;

        return rectShape;
    }


    private static int frame;
    private static Vector3 a, b, c, d;
    
    private static Vector3 ClipPlaneLerp(float u, float v)
    {
        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            
            Camera cam = DrawCam;

            a = cam.ViewportToWorldPoint(new Vector3(0, 1, 1f));
            b = cam.ViewportToWorldPoint(new Vector3(1, 1, 1f));
            c = cam.ViewportToWorldPoint(new Vector3(1, 0, 1f));
            d = cam.ViewportToWorldPoint(new Vector3(0, 0, 1f));
        }

        return QuadLerp(a, b, c, d, u, v);
    }
    
    private static Vector3 QuadLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float u, float v)
    {
        Vector3 abu = Vector3.Lerp(a, b, u);
        Vector3 dcu = Vector3.Lerp(d, c, u);
        return Vector3.Lerp(abu, dcu, 1 - v);
    }

#endregion

    

    private static int TextScale(float scale)
    {
        return (int)((Screen.height > Screen.width ? Screen.height : (float)Screen.width) / 100 * scale);
    }
    

    private static Vector3 WorldToCanvas(Vector3 pos)
    {
        Vector3 viewport_position = DrawCam.WorldToViewportPoint(pos);
        Vector2 canvasSize        = canvas.GetComponent<RectTransform>().sizeDelta;

        return new Vector3(viewport_position.x * canvasSize.x - canvasSize.x * .5f,
                           viewport_position.y * canvasSize.y - canvasSize.y * .5f);
    }
    
    
    private static Vector3 ScreenToCanvas(Vector3 pos)
    {
        Vector3 viewport_position = DrawCam.ScreenToViewportPoint(pos);
        Vector2 canvasSize        = canvas.GetComponent<RectTransform>().sizeDelta;
        
        return new Vector3(viewport_position.x * canvasSize.x - canvasSize.x * .5f,
            viewport_position.y * canvasSize.y - canvasSize.y * .5f);
    }
    

    private static bool OnScreen(Vector3 pos)
    {
        Vector3 vP = DrawCam.WorldToViewportPoint(pos);
        float aspect = DrawCam.aspect;
        Vector2 extra = new Vector2(.1f / aspect, .1f * aspect);
        return vP.x + extra.x >= 0 && vP.x - extra.x <= 1 && vP.y + extra.y >= 0 && vP.y - extra.y <= 1;
    }
    
    


#region Rendering
    private void OnDrawGizmos()
    {
        if (_editorDraw)
            Render(false);
    }

    
    private static readonly GUIStyle guiStyle = new GUIStyle()
    {
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleCenter
    };
    
    
    #if UNITY_EDITOR
    
    private void OnGUI()
    {
        int count = guiJobs.Count;
        for (int i = 0; i < count; i++)
            if (!guiJobs[i].Label(guiStyle))
            {
                guiJobs.GetRemoveAt(i).Disable();
                i--;
                count--;
            }
    }
    
    #endif


    private void OnPostRender()
    {
       Render(true);
    }

    
    private void LateUpdate()
    {
        TextDraw();
    }
    
    
    private static void Render(bool inGame)
    {
        if (!DrawStuff || shapeJobs.Count == 0)
            return;

        
    //  CleanUp  //
        if (inGame)
            if (cleanUp)
                Shape.CleanUp();
            else
                cleanUp = true;

        int count = shapeJobs.Count;
        
        
    //  Prepare (Set To Screen if necessary)  //
        if (inGame)
        {
            int order = 0;

            for (int i = count - 1; i > -1; i--)
                order = shapeJobs[i].RenderPrepare(order);
        }

        meshShapes.Clear();
        
        
    //  LINES  //
        LineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        for (int i = 0; i < count; i++)
        {
            if (!shapeJobs[i].ShouldBeRendered(inGame))
                continue;

            if (shapeJobs[i].mesh != null && shapeJobs[i].mesh.used)
                meshShapes.Add(shapeJobs[i]);
            
            if(f.Same(shapeJobs[i].color.a, 0))
                continue;
                
            GL.Color(shapeJobs[i].color);
            int pCount = shapeJobs[i].pointCount - 1;
            for (int e = 0; e < pCount; e++)
            {
                GL.Vertex(shapeJobs[i][e]);
                GL.Vertex(shapeJobs[i][e + 1]);
            }
        }
        GL.End();
        GL.PopMatrix();


    //  MESHES  //
        int meshCount = meshShapes.Count;
        if (meshCount > 0)
        {
             vertices.Clear();
               colors.Clear();
            triangles.Clear();

            int meshOffset = 0;
            for (int i = 0; i < meshCount; i++)
            {
                ReusableMesh mesh = meshShapes[i].mesh;
                vertices.AddRange(mesh.verts);
                  colors.AddRange(mesh.cols);

                int triangleCount = mesh.tris.Count;
                for (int e = 0; e < triangleCount; e++)
                    triangles.Add(mesh.tris[e] + meshOffset);

                meshOffset += mesh.verts.Count;
            }
            
            m.Clear();
            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            m.SetColors(colors);
            
            PolygonMaterial.SetPass(0);
            Graphics.DrawMeshNow(m, Matrix4x4.identity);
        }
    }
    
    
    private static readonly Mesh m = new Mesh();
    private static float withOutTexts;
    private static readonly List<Shape> meshShapes = new List<Shape>(1000);
    
    private static readonly List<Vector3> vertices  = new List<Vector3>(short.MaxValue);
    private static readonly List<Color32> colors    = new List<Color32>(short.MaxValue);
    private static readonly List<int>     triangles = new List<int>(short.MaxValue);

    private static void TextDraw()
    {
    //  CleanUp  //
        int count = textJobs.Count;
        for ( int i = 0; i < count; i++ )
            if (textJobs[i].frame < Time.frameCount)
            {
                textJobs.GetRemoveAt(i).Disable();
                count--;
                i--;
            }

        if (textJobs.Count > 0)
        {
            for (int i = 0; i < textJobs.Count; i++)
                textJobs[i].SetCanvasPos();

            withOutTexts = 0;
        }
        else
            if (canvas != null)
            {
                withOutTexts += Time.deltaTime;
                if ( withOutTexts >= 5)
                    DestroyTextStuff();
            }
    }

    
    private void OnDisable()
    {
        if (inst != null)
            Destroy(inst);
    }
    

    #endregion 
}