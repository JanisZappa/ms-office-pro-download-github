using UnityEngine;


public struct OkLab
{
    public float L;
    public float a;
    public float b;

    public OkLab(float l, float a, float b)
    {
        L = l;
        this.a = a;
        this.b = b;
    }
}


public static class OkLabExt
{
    public static Color ToColor(this OkLab lab)
    {
        float l_ = lab.L + 0.3963377774f * lab.a + 0.2158037573f * lab.b;
        float m_ = lab.L - 0.1055613458f * lab.a - 0.0638541728f * lab.b;
        float s_ = lab.L - 0.0894841775f * lab.a - 1.2914855480f * lab.b;

        float l = l_*l_*l_;
        float m = m_*m_*m_;
        float s = s_*s_*s_;

        return new Color(
            +4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
            -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
            -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s,
            1);
    }


    public static OkLab ToOkLab(this Color color)
    {
        float l = 0.4122214708f * color.r + 0.5363325363f * color.g + 0.0514459929f * color.b;
        float m = 0.2119034982f * color.r + 0.6806995451f * color.g + 0.1073969566f * color.b;
        float s = 0.0883024619f * color.r + 0.2817188376f * color.g + 0.6299787005f * color.b;

        float l_ = Mathf.Pow(l, 1f/ 3);
        float m_ = Mathf.Pow(m, 1f/ 3);
        float s_ = Mathf.Pow(s, 1f/ 3);

        return new OkLab(
            0.2104542553f*l_ + 0.7936177850f*m_ - 0.0040720468f*s_,
            1.9779984951f*l_ - 2.4285922050f*m_ + 0.4505937099f*s_,
            0.0259040371f*l_ + 0.7827717662f*m_ - 0.8086757660f*s_
        );
    }
}