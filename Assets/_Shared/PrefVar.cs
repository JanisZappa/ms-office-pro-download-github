using UnityEngine;

public class PrefBool
{
    private readonly string name;

    public PrefBool(string name)
    {
        this.name = "prefbool_" + name;
    }

    private bool v
    {
        set { PlayerPrefs.SetInt(name, value? 1 : 0); }
        get { return PlayerPrefs.GetInt(name) == 1; }
    }

    public void Toggle()
    {
        v = !v;
    }
    
    public static bool operator true(PrefBool p)=> p.v;
    public static bool operator false(PrefBool p)=> !p.v;
}

public class PrefInt
{
    private readonly string name;

    public PrefInt(string name)
    {
        this.name = "prefint_" + name;
    }

    private int v
    {
        set { PlayerPrefs.SetInt(name, value); }
        get { return PlayerPrefs.GetInt(name); }
    }


    public static int   operator +(PrefInt a, PrefInt b) => a.v + b.v;
    public static int   operator +(PrefInt a, int b) => a.v + b;
    public static int   operator +(int a, PrefInt b) => a + b.v;
    public static float operator +(PrefInt a, float b) => a.v + b;
    public static float operator +(float a, PrefInt b) => a + b.v;

    public static int   operator -(PrefInt a, PrefInt b) => a.v - b.v;
    public static int   operator -(PrefInt a, int b) => a.v - b;
    public static int   operator -(int a, PrefInt b) => a - b.v;
    public static float operator -(PrefInt a, float b) => a.v - b;
    public static float operator -(float a, PrefInt b) => a - b.v;

    public static int   operator *(PrefInt a, PrefInt b) => a.v * b.v;
    public static int   operator *(PrefInt a, int b) => a.v * b;
    public static int   operator *(int a, PrefInt b) => a * b.v;
    public static float operator *(PrefInt a, float b) => a.v * b;
    public static float operator *(float a, PrefInt b) => a * b.v;

    public static int   operator /(PrefInt a, PrefInt b) => b.v == 0 ? 0 : a.v * b.v;
    public static int   operator /(PrefInt a, int b) => b == 0 ? 0 : a.v * b;
    public static int   operator /(int a, PrefInt b) => b.v == 0 ? 0 : a * b.v;
    public static float operator /(PrefInt a, float b) => Mathf.Approximately(0, b) ? 0 : a.v * b;
    public static float operator /(float a, PrefInt b) => b.v == 0 ? 0 : a * b.v;

    public static PrefInt operator ++(PrefInt a)
    {
        a.v++;
        return a;
    }
    public static PrefInt operator --(PrefInt a)
    {
        a.v--;
        return a;
    }
}
