using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;


public class Redistribution : MonoBehaviour
{
    public int maxDifferenceMulti;
    public float fps;
    
    [Space]
    public AnimationCurve curve;
    public AnimationCurve cv;
    public TextAsset usData;
    
    private const int Count = 10000;
    private const double MaxMoney = 250000000000;
    private const int DCount = 500, DStep = Count / DCount;
    private const float width = 17f, step = width / DCount, height = 9.25f;
    
    private readonly double[] values = new double[Count], nValues = new double[Count], sValues = new double[Count];
    private readonly int[] map = new int[Count];
    
    private float startTime, measureTime;
    
    private Vector4 percentage;
    private int iterations, richer, maxed, oldMaxed, run, zoom;
    private bool mode;
    private double poorerLevel;
    
    
    private struct SortDouble
    {
        public double value;
        public int index;

        public SortDouble(double value, int index)
        {
            this.value = value;
            this.index = index;
        }
    }
    
    
    
    private void Start()
    {
        Init();
        CreateMesh();
        Evaluate();
        UpdateMesh(true);
        
        StartCoroutine(WaitAround());
    }


    private void Init()
    {
        string[] usLines = usData.text.Split('\n');
        
        int usCount = usLines.Length;
        double[] usValues = new double[usCount + 2];
        for (int i = 0; i < usCount; i++)
            usValues[i + 1] = double.Parse(usLines[i].Split()[1].Replace("$",""));
        //usCount++;
        
        usValues[0] = MaxMoney;
        
        SortDouble[] val = new SortDouble[Count];
        const float s = 1f / (Count - 1);
        for (int i = 0; i < Count; i++)
        {
            double v = Mathf.Pow(cv.Evaluate(Random.Range(0, 1f)), 100) * MaxMoney;
            
            float l = s * i * usCount * .99999999f;
            int a = Mathf.FloorToInt(l), b = a + 1;
            
            double aV = usValues[a], bV = usValues[b];
            l = l % 1;
            l = 1f - Mathf.Pow(1f - l, 8);
            
            if(i == 0)
                l = 1f - Mathf.Pow(1f - l, 8);
            v = aV + (bV - aV) * l;
            
            val[i] = new SortDouble(v, i);
        }
            
        val = val.OrderByDescending(x => x.value).ToArray();
        double min = 0;//val[Count - 1].value;
        for (int i = 0; i < Count; i++)
        {
            SortDouble sD = val[i];
            values[i] = sD.value - min;
            
            if(values[i] > MaxMoney)
                values[i] = MaxMoney;
            map[sD.index] = i;
        }
        
        //Normalize();
        SaveStartValues();
        
        run = 0;
    }


    private void Normalize(bool copy = false)
    {
        double max = 0;

        for (int i = 0; i < Count; i++)
        {
            double v = values[i];
            if(v > max)
                max = v;
        }
        
        max = MaxMoney / max;

        if (copy)
            for (int i = 0; i < Count; i++)
                nValues[i] = values[i] * max;
        else
            for (int i = 0; i < Count; i++)
                values[i] *= max;
    }


    private void SaveStartValues()
    {
        for (int i = 0; i < Count; i++)
            sValues[i] = values[i];
    }


    private void RicherPeople()
    {
        richer = 0;
        for (int i = 0; i < Count; i++)
            if(values[i] > sValues[i])
                richer++;
        
        richer = Mathf.FloorToInt(richer * 1f / Count * 100);
    }
    
    
    private void MaxPeople()
    {
        maxed = 0;
        oldMaxed = 0;
        double max = MaxMoney / ZoomMulti;
        for (int i = 0; i < Count; i++)
        {
            if(values[i] > max)
                maxed++;
            if(sValues[i] > max)
                oldMaxed++;
        }
            
        maxed    = Mathf.FloorToInt(maxed * 1f / Count * 100);
        oldMaxed = Mathf.FloorToInt(oldMaxed * 1f / Count * 100);
    }


    private void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            switch (run)
            {
                case 0:    StartCoroutine(RedistributeUntilFair()); break;
                case 2:    Init(); break;
            }
            
        if(Input.GetKeyDown(KeyCode.M))
            mode = !mode;
        
        if(Input.GetKeyUp(KeyCode.UpArrow))
            zoom++;
        
        if(Input.GetKeyUp(KeyCode.DownArrow))
            zoom = Mathf.Max(0, zoom - 1);
    }


    private IEnumerator RedistributeUntilFair()
    {
        run = 1;
        startTime = Time.realtimeSinceStartup;
        iterations = 0;
        double maxMulti = maxDifferenceMulti;
        const int max = Count - 1;
        bool reachedGoal = false;
        float thresh = 1f / fps;
        while (true)
        {
            float time = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - time < thresh)
            {
                Redistribute();
                if (values[max] >= 0 && values[0] / values[max] <= maxMulti)
                {
                    reachedGoal = true;
                    break;
                }
            }
            
            if (reachedGoal)
                break;
            
            yield return null;
        }    
        
        run = 2;

        StartCoroutine(WaitAround());
    }


    private IEnumerator WaitAround()
    {
        float thresh = 1f / fps;
        while (run != 1)
        {
            float time = Time.realtimeSinceStartup;
            while (Time.realtimeSinceStartup - time < thresh && run != 1)
                for (int i = 0; i < Count; i++)
                    if(run != 1)
                        values[i] = values[i];
                
            yield return null;
        }
    }
        
        
    private void Redistribute()
    {
        const int start = Count - 1;
        double a = values[start];
        for (int i = start; i > 0; i--)
        {
            double b    = values[i - 1];
            double give = (b - a) * .5;

            values[i] = a + give;
            a = b - give;
            values[i - 1] = a;
        }

        iterations++;
    }
    
    
    private void RedistributeTopDown()
    {
        const float max = Count - 1;
        double b = values[0];
        for (int i = 0; i < max; i++)
        {
            double a    = values[i + 1];
            double give = (b - a) * .5;

            values[i] = b - give;
            b = a + give;
            values[i + 1] = b;
        }

        iterations++;
    }


    private void CalculatePercentage()
    {
        double richest = values[0] / 4, half = richest * 3, third = richest * 2, fourth = richest;
        
        int a = 0, b = 0, c = 0, d = 0;
        for (int i = 0; i < Count; i++)
        {
            double v = values[i];
            if(v >= half)
                a++;
            else if (v > third)
                b++;
            else if (v > fourth)
                c++;
            else if(v < 1)
                d++;
        }
        
        int aInt = Mathf.FloorToInt((float)a / Count * 100);
        int bInt = Mathf.FloorToInt((float)b / Count * 100);
        int cInt = Mathf.FloorToInt((float)c / Count * 100);
        int dInt = Mathf.FloorToInt((float)d / Count * 100);
        percentage = new Vector4(aInt, bInt, cInt, dInt);
    }

    private void WhosPoorer()
    {
        poorerLevel = 0;
        for (int i = 0; i < Count; i++)
            if (sValues[i] < values[i])
            {
                poorerLevel = sValues[i];
                break;
            }
    }

    private void Evaluate()
    {
        Normalize(true);
        CalculatePercentage();
        RicherPeople();
        MaxPeople();
        WhosPoorer();
    }
    
        
    private void Update()
    {
        Inputs();
        
        Evaluate();
        
        //Draw();
        UpdateMesh();
    }


    private void Draw()
    {
        Vector2 root = new Vector2(width * -.5f, height * -.5f);
        int displayID = 0;
        Color a = Color.black, b = Color.black.A(.06f), c = COLOR.purple.violet, d = COLOR.red.tomato, e = COLOR.turquois.dark;
        double richest = values[0] / 4, half = richest * 3, third = richest * 2, fourth = richest;
        
        for (int i = 0; i < Count; i += DStep)
        {
            int index = mode? map[i] : i;

            Vector2 min = root + Vector2.right * displayID++ * step;
            double v = values[index], v2 = nValues[index] / MaxMoney;
            Color p = v > fourth? v > third? v > half? c : e : d : a;
            v *= ZoomMulti;
            if(v > MaxMoney)
                v = MaxMoney;
            v /= MaxMoney;
            
            Vector2 max = min + Vector2.right * step + Vector2.up * (float)v * height;
            DRAW.Box(min, max).SetColor(p);
        
            if (v2 > v)
            {
                max = min + Vector2.right * step + Vector2.up * (float)v2 * height;
                DRAW.Box(min + Vector2.up * (float)(v) * height, max).SetColor(b);
            }
        }    
    }


    private double ZoomMulti
    {
        get
        {
            double z = 1;
            for (int i = 0; i < zoom; i++)
                z *= 10;
            return z;
        }
    }
    
    
#region Log
    private void OnEnable()
    {
        DebugUI.TR += DebugTR;
        DebugUI.BR += DebugBR;
        DebugUI.BL += DebugBL;
        DebugUI.TL += DebugTL;
        DebugUI.T  += DebugT;
    }

    private void DebugTR(StringBuilder builder)
    {
        float d = run == 0? 0 : 100 - percentage.x -percentage.y - percentage.z;
        
        builder.AppendLine(Value(values[0], true) + " Richest").
            AppendLine(Value(values[Count - 1], true) + " Poorest").
            AppendLine().
            AppendLine(percentage.x + "% 1st").
            AppendLine(percentage.y + "% 2nd").
            AppendLine(percentage.z + "% 3rd").
            AppendLine(d + "% 4th").
            AppendLine().
            AppendLine(percentage.w + "% NO").
            AppendLine(richer + "% UP").
            AppendLine((richer != 0? 100 - richer : 0) + "% DN").AppendLine().AppendLine(Value(poorerLevel));
    }
    
    private void DebugBR(StringBuilder builder)
    {
        switch (run)
        {
            case 0: measureTime = startTime; break;
            case 1: measureTime = Time.realtimeSinceStartup; break;
        }  
        
        float rT = measureTime - startTime;
        builder.AppendLine(Value(iterations, true) + " Iterations - Runtime: " + TimeSpan.FromSeconds((long)rT).ToString("c"));
    }

    private void DebugBL(StringBuilder builder)
    {
        builder.AppendLine("Zoom " + Value(ZoomMulti, true));
    }
    
    private void DebugTL(StringBuilder builder)
    {
        builder.AppendLine("Display Max " + Value(MaxMoney / ZoomMulti, true));
    }

    private void DebugT(StringBuilder builder)
    {
        builder.AppendLine("Achieved by " + maxed + "%   Before by " + oldMaxed + "%");
    }
    
    private static string Value(double value, bool noDollar = false)
    {
        bool negative = value < 0;
        if(negative)
            value *= -1;
        
        string v = value.ToString("C");
        if(v[v.Length - 3] == '.')
            v = v.Substring(0, v.Length - 3);
        
        if(noDollar)
            v = v.Replace("$", "");
        
        if(negative)
            v = "-" + v;
        
        return v;
    }
#endregion

    private Mesh mesh;
    private Vector3[] verts;
    private int[] tris;
    private Color[] cols;
    private Color[] lerpCols;
    private void CreateMesh()
    {
        verts = new Vector3[DCount * 4 * 2];
        cols  = new Color[DCount * 4 * 2];
        lerpCols = new Color[DCount * 2];
        tris = new int[DCount * 6 * 2];
        for (int i = 0; i < DCount * 2; i++)
        {
            tris[i * 6]     = i * 4;
            tris[i * 6 + 1] = i * 4 + 1;
            tris[i * 6 + 2] = i * 4 + 2;
            tris[i * 6 + 3] = i * 4 + 2;
            tris[i * 6 + 4] = i * 4 + 3;
            tris[i * 6 + 5] = i * 4;
        }
        
        mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetColors(cols);
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000);
        
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
        
        Material mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat("Height", height);
    }


    private void UpdateMesh(bool init = false)
    {
        Vector2 root = new Vector2(width * -.5f, height * -.5f);
        int displayID = 0;
        Color a = Color.Lerp(Color.black, Color.white, .1f), b = Color.Lerp(Color.black, Color.white, .7f), c = COLOR.purple.violet, d = COLOR.red.tomato, e = COLOR.turquois.dark;
        double richest = values[0] / 4, half = richest * 3, third = richest * 2, fourth = richest;
        
        float dt = init? 1 : Time.deltaTime * 2;
        
        for (int i = 0; i < Count; i += DStep)
        {
            int index = mode? map[i] : i;

            Vector2 min = root + Vector2.right * displayID * step;
            double v = values[index], v2 = nValues[index] / MaxMoney;
            Color p = v > fourth? v > third? v > half? c : e : d : a;
            v *= ZoomMulti;
            if(v > MaxMoney)
                v = MaxMoney;
            v /= MaxMoney;
            
            Vector2 max = min + Vector2.right * step + Vector2.up * (float)v * height;
            MeshBox(displayID, min, max, p, dt);
        
            max = min + Vector2.right * step + Vector2.up * (float)v2 * height;
            MeshBox(displayID + DCount, min + Vector2.up * (float)(v) * height, max, b, dt);
            
            displayID++;
        }    
        
        mesh.SetVertices(verts);
        mesh.SetColors(cols);
    }


    private void MeshBox(int id, Vector2 min, Vector2 max, Color color, float dt)
    {
        verts[id * 4] = min;
        verts[id * 4 + 1] = new Vector2(min.x, max.y);
        verts[id * 4 + 2] = max;
        verts[id * 4 + 3] = new Vector2(max.x, min.y);
        
        Color c = lerpCols[id];
              c = Color.Lerp(c, color, dt);
        lerpCols[id] = c;
        
        for (int i = 0; i < 4; i++)
            cols[id * 4 + i] = c.A(i < 2? 0 : 1);
    }
}
