using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public partial class DRAW
{
    private static Material _lineMaterial, _polygonMaterial;
    
    private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
    private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
    private static readonly int Cull     = Shader.PropertyToID("_Cull");
    private static readonly int ZWrite   = Shader.PropertyToID("_ZWrite");

    private static Material LineMaterial
    {
        get
        {
            if ( _lineMaterial == null )
            {
                _lineMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
                {
                    renderQueue = short.MaxValue, 
                    hideFlags   = HideFlags.HideAndDontSave
                };
                
                _lineMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _lineMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _lineMaterial.SetInt(Cull, (int)UnityEngine.Rendering.CullMode.Off);
                _lineMaterial.SetInt(ZWrite, 1);
            }
            return _lineMaterial;
        }
    }

    private static Material PolygonMaterial
    {
        get
        {
            if (_polygonMaterial == null)
                _polygonMaterial = new Material(Resources.Load("DRAW_Polygon") as Shader);
            
            return _polygonMaterial;
        }
    }
   
   
    public class Shape
    {
        private static Stack<Shape> vectors, points100, points1000;
        
        public static void CreatePools()
        {
            {
                const int capacity   = 100000;
                const int pointCount = 2;
                vectors = new Stack<Shape>(capacity);
                for (int i = 0; i < capacity; i++)
                    vectors.Push(new Shape(pointCount, vectors));
            }
            {
                const int capacity   = 100000;
                const int pointCount = 100;
                points100 = new Stack<Shape>(capacity);
                for (int i = 0; i < capacity; i++)
                    points100.Push(new Shape(pointCount, points100));
            }
            {
                const int capacity   = 1000;
                const int pointCount = 1000;
                points1000 = new Stack<Shape>(capacity);
                for (int i = 0; i < capacity; i++)
                    points1000.Push(new Shape(pointCount, points1000));
            }
        }
        
        
        public static void DestroyPools()
        {
            vectors    = null;
            points100  = null;
            points1000 = null;
        }
        
        
        public static Shape Get(int points)
        {
            if (cleanUp)
                CleanUp();

            if (points <= 2)
            {
                if (vectors.Count > 0)
                    return vectors.Pop().SetActive(points).SetColor(BaseColor);
                
                Debug.Log("No Shape in vectors! - PointCount: " + points);
                return NullShape;
            }

            if (points <= 100)
            {
                if (points100.Count > 0)
                    return points100.Pop().SetActive(points).SetColor(BaseColor);
                
                Debug.Log("No Shape in points100! - PointCount: " + points);
                return NullShape;
            }

            if (points <= 1000)
            {
                if (points1000.Count > 0)
                    return points1000.Pop().SetActive(points).SetColor(BaseColor);
                
                Debug.Log("No Shape in points1000! - PointCount: " + points);
                return NullShape;
            }   

            return NullShape;
        }


        public static void CleanUp()
        {
            while (true)
            {
                bool allFine = true;
                for (int i = 0; i < shapeJobs.Count; i++)
                    if (shapeJobs[i].ShouldBeReset)
                    {
                        shapeJobs[i].Reset();
                        allFine = false;
                        break;
                    }

                if (allFine)
                    break;
            }

            cleanUp = false;
        }
        
        public static Color BaseColor = Color.blue;

        
        private readonly Vector3[] points;
        public  int   pointCount;
        public  Color color = Color.white;
        public readonly ReusableMesh mesh;

        private Shape subShape;
        private int frame;

        private bool screenSpace, triangulate;
        private float fill;

        private readonly Stack<Shape> pool;

        private bool ShouldBeReset { get { return frame < Time.frameCount; }}

        
        public Vector3 this[int index]
        {
            get{ return points[index]; }
            set{ points[index] = value; }
        }


        public Shape(int maxPoints, Stack<Shape> pool)
        {
            points = new Vector3[maxPoints];
            if(maxPoints > 2)
                mesh = new ReusableMesh(maxPoints);

            this.pool = pool;
        }

        
        private Shape SetActive(int pointCount)
        {
            this.pointCount = pointCount;
            shapeJobs.Add(this);

            frame = Time.frameCount;
            return this;
        }

        
        private void Reset()
        {
            color      = BaseColor;
            pointCount = 0;
            
            if(shapeJobs.Remove(this))
                pool.Push(this);

            subShape?.Reset();

            subShape = null;
            
            if(mesh != null && mesh.used)
                mesh.Clear();

            frame = -1;

            screenSpace = false;
            triangulate = false;
            fill = 0;
        }


        public Shape SetSub(Shape subShape)
        {
            this.subShape = subShape;
            
            return this;
        }
        
        
        public Shape SetDepth(float z)
        {
            points.SetZ(0, pointCount, z);

            subShape?.SetDepth(z);

            return this;
        }
        
        
        public Shape SetColor(Color color)
        {
            this.color = color;

            subShape?.SetColor(color);

            return this;
        }


        public Shape Set(int index, Vector3 point)
        {
            points[index] = point;
            return this;
        }


        public Shape Reverse()
        {
            points.Reverse(pointCount);
            
            return this;
        }
        
        
        public Shape Copy(int from, int to)
        {
            points[to] =  points[from];
            return this;
        }


        public Shape SetPoints(Vector2[] points)
        {
            for (int i = 0; i < pointCount; i++)
                this.points[i] = points[i];

            return this;
        }
        
        
        public Shape SetPoints(Vector3[] points)
        {
            for (int i = 0; i < pointCount; i++)
                this.points[i] = points[i];

            return this;
        }
        
        
        public Shape Fill(float alpha = .3f, bool hideLines = false)
        {
            if (hideLines)
                color = color.A(0);

            subShape?.Fill(alpha, hideLines);

            fill = alpha;
            
            return this;
        }
        
        
        public Shape TriFill(float alpha = .3f, bool hideLines = false)
        {
            if (hideLines)
                color = color.A(0);

            subShape?.TriFill(alpha, hideLines);

            fill = alpha;
            triangulate = true;
            
            return this;
        }


        public Shape SetVectorPoints(Vector3 pointA, Vector3 pointB)
        {
            points[0] = pointA;
            points[1] = pointB;
            
            return this;
        }
        
        
        public Shape SetCirclePoints(Vector3 midPoint, float radius, float yMulti = 1)
        {
            float step = -360f / (pointCount - 1);
            for ( int i = 0; i < pointCount; i++ )
                points[i] = (Quaternion.AngleAxis(i * step, Vector3.forward) * Vector3.up * radius).MultiY(yMulti) + midPoint;
            
            return this;
        }
        
        
        public Shape SetCirclePoints(Vector3 midPoint, float radius, Quaternion rot)
        {
            float step = -360f / (pointCount - 1);
            for ( int i = 0; i < pointCount; i++ )
                points[i] = rot * Quaternion.AngleAxis(i * step, Vector3.forward) * Vector3.up * radius + midPoint;
            
            return this;
        }
        
        
        public Shape SetArcPoints(Vector3 midPoint, float radius, float angle, float bend)
        {
            Vector3 start = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * radius * Mathf.Sign(bend);
            float step = Mathf.Abs(bend * 360f / (pointCount - 1));
            
            for ( int i = 0; i < pointCount; i++ )
                points[i] = Quaternion.AngleAxis(i * step, Vector3.forward) * start + midPoint;
            
            return this;
        }
        
        
        public Shape SetZappCirclePoints(Vector3 midPoint, float radius, float width, int segments, float angle = 0)
        {
            radius = Mathf.Clamp(radius - width * .5f, 0, float.MaxValue);
            
            for ( int i = 0; i < pointCount; i++ )
                points[i] = Quaternion.AngleAxis(i * -360f / (segments - 1) + angle, Vector3.forward) 
                            * (Vector3.up * (radius + width * Mathf.PingPong(i, 1)))  + midPoint;
            
            return this;
        }
        
        
        public Shape SetupScreenPoint(Vector3 clipPos, float radius, Quaternion camRot)
        {
            float step = -360f / (pointCount - 1);
            for ( int i = 0; i < pointCount; i++ )
                points[i] = camRot * Quaternion.AngleAxis(i * step, Vector3.forward) * Vector3.up * radius + clipPos;
            
            return this;
        }

        
        public Shape SetPoints(Func<int, Vector3> pointFunc)
        {
            for (int i = 0; i < pointCount; i++)
                points[i] = pointFunc(i);

            return this;
        }


        public Shape HoldFor(float seconds)
        {
            frame += Mathf.RoundToInt(seconds * Application.targetFrameRate);

            if (subShape != null)
                subShape.HoldFor(seconds);
            
            return this;
        }


        public Shape ToScreen()
        {
            subShape?.ToScreen();

            screenSpace = true;
            
            return this;
        }


        public int RenderPrepare(int order)
        {
            if (screenSpace)
            {
                const float offset = .0001f;

                for (int i = 0; i < pointCount; i++)
                    points[i] = DrawCam.ScreenToWorldPoint(points[i].SetZ(DrawCam.nearClipPlane + offset * order + .1f));

                order++;
            }


            if (fill > 0 && pointCount >= 3 && points[0].Closeish(points[pointCount - 1]))
                mesh.FillThis(points, pointCount, color, fill, triangulate);
            
            
            return order;
        }


        public bool ShouldBeRendered(bool inGame)
        {
            return !screenSpace || inGame;
        }
    }


    public class ReusableMesh
    {
        public bool used;
        
        public readonly List<Vector3> verts;
        public readonly List<Color32> cols;
        public readonly List<int>     tris;
        
        public ReusableMesh(int maxPoints)
        {
            verts = new List<Vector3>(maxPoints);
            cols  = new List<Color32>(maxPoints);
            tris  = new List<int>(maxPoints * 3);
        }

        
        public void FillThis(Vector3[] points, int pointCount, Color color, float alpha, bool triangulate)
        {
            int count = pointCount - 1;
            
            if (triangulate)
            {
                verts.Clear();
                for(int i = 0; i < count; i++)
                    verts.Add(points[i]);
		
                tris.CopyFrom(Triangulator.Fill(points, count));
                
                Color32 meshColor = color.A(alpha);
                cols.Clear();
                for (int i = 0; i < count; i++)
                    cols.Add(meshColor);
            }
            else
            {
                Vector3 midPoint = Vector3.zero;
                for(int i = 0; i < count; i++)
                    midPoint += points[i];
                midPoint = midPoint / count;
            
                verts.Clear();
                verts.Add(midPoint);
            
                for(int i = 0; i < count; i++)
                    verts.Add(points[i]);
            
            
                tris.Clear();
                for (int i = 0; i < count; i++)
                {
                    tris.Add(0);
                    tris.Add(i + 1);
                    tris.Add(i + 2 < pointCount ? i + 2 : 1);
                }
                
                Color32 meshColor = color.A(alpha);
                cols.Clear();
                for ( int i = 0; i < pointCount; i++ )
                    cols.Add(meshColor);
            }

            used = true;
        }
        
        
        public void Clear()
        {
            used = false;
        }
    }


    private class OverlayText
    {
        private static Stack<OverlayText> poolTexts;
        
        
        public static OverlayText Get()
        {
            return poolTexts.Count > 0 ? poolTexts.Pop() : null;
        }
        
        
        public static void SetAllActive(bool active)
        {
            if (active)
            {
                const int count = 1000;
                poolTexts = new Stack<OverlayText>(count);
                for (int i = 0; i < count; i++)
                {
                    var overlayText = new OverlayText();
                }
            }
            else 
                if (poolTexts != null)
                {
                    while(poolTexts.Count > 0)
                        Destroy(poolTexts.Pop().text.gameObject);
                  
                    poolTexts = null;
                }
        }
        
        
        public Vector2 offset;
        public readonly TextMeshProUGUI text;
        public Vector3 pos;
        public int frame;
        public bool screenSpace;


        private OverlayText()
        {
            GameObject newObj = new GameObject("DRAW_Text", typeof(RectTransform));
            newObj.transform.SetParent(canvas.transform, false);
            
            text = newObj.AddComponent<TextMeshProUGUI>();
            text.font             = DrawFont;
            text.fontStyle        = FontStyles.Bold;
            text.raycastTarget    = false;
            text.alignment        = TextAlignmentOptions.Midline;
            text.gameObject.layer = TextLayer;
            
            Disable();
        }


        public void Disable()
        {
            text.rectTransform.anchoredPosition = Vector2.up * -10000;
            poolTexts.Push(this);
        }

        public void SetCanvasPos()
        {
            Vector3 shift = new Vector3(offset.x * text.preferredWidth * .5f, offset.y * text.preferredHeight * .5f, -Mathf.Abs(pos.z));
            text.rectTransform.anchoredPosition3D = (screenSpace? ScreenToCanvas(pos) : WorldToCanvas(pos)) + shift;
        }
    }


    private class GUIText
    {
        private static Stack<GUIText> poolTexts;
        
        public static bool IsActive
        {
            get { return poolTexts != null; }
        }
        
        public static GUIText Get()
        {
            return poolTexts.Count > 0 ? poolTexts.Pop() : null;
        }
        
        
        public static void SetAllActive(bool active)
        {
            if (active)
            {
                const int count = 100000;
                poolTexts = new Stack<GUIText>(count);
                for (int i = 0; i < count; i++)
                    poolTexts.Push(new GUIText());
            }
            else 
                poolTexts = null;
        }


        public GUIText Set(string text, Vector2 pos, float size, Color color)
        {
            this.text = text;
            this.pos = pos;
            this.size = size;
            this.color = color;

            frame = Time.frameCount;

            return this;
        }

        
        private string text;
        private Vector2 pos;
        private float size;
        private Color color;
        private int frame;


        public void Disable()
        {
            poolTexts.Push(this);
        }

        
        public bool Label(GUIStyle style)
        {
            if (frame == Time.frameCount)
            {
                style.fontSize = TextScale(size);
                style.normal.textColor = color;
                
                GUI.Label(new Rect(pos.x - 500, pos.y - 500, 1000, 1000), text, style);

                return true;
            }

            return false;
        }
    }
}
