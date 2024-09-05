using System.Runtime.InteropServices;
using UnityEngine;


public class GridLights : Singleton<GridLights>
{
    public float scale, speed;
    private const int PLightCount = 8, DLightCount = 4;
    
    private struct PLight
    {
        public Vector3 pos;
        Vector3 color;
        float rangemulti;
        float banana;

        public PLight(Vector3 pos, Vector3 color, float rangemulti, float banana)
        {
            this.pos = pos;
            this.color = color;
            this.rangemulti = rangemulti;
            this.banana = banana;
        }

        public void Update(float t, int x, float scale)
        {
            float speed = (.5f + RValue(x, 5.31f)) * .75f * 3;
            pos = 
                new Vector3(Mathf.Sin(t * .25f * speed + x * 53.211f) * 70 * scale, 3.75f, Mathf.Sin(t * .5f * speed + x * 211.1271f) * 70 / 16 * 9 * scale);

            rangemulti = (.2125f + Mathf.Sin(t * 54.52f * 2 + x * 13.14f) * .0115f * .125f) * .236f;
        }
    };
    
    
    private struct DLight
    {
        public Vector3 dir;
        Vector3 color;
        Vector2 banana;

        public DLight(Vector3 dir, Vector3 color)
        {
            this.dir = dir;
            this.color = color;
            
            banana = Vector2.zero;
        }
    };
    
    
    private readonly PLight[] _pLights = new PLight[PLightCount];
    private readonly DLight[] _dLights = new DLight[DLightCount];
    
    private ComputeBuffer pLightBuffer, dLightBuffer;
    private static readonly int PLightBuffer = Shader.PropertyToID("PLightBuffer");
    private static readonly int DLightBuffer = Shader.PropertyToID("DLightBuffer");
    private bool LightsOff;
    private static readonly int LightsOn = Shader.PropertyToID("LightsOn");


    private void Start()
    {
        pLightBuffer = Buff.New(_pLights, Marshal.SizeOf<PLight>());
        Shader.SetGlobalBuffer(PLightBuffer, pLightBuffer);
        
        for (int x = 1; x < PLightCount; x++)
        {
            Vector3 color = new Vector3(RValue(x, 5.31f) * .75f + .25f, RValue(x, 995.2141f) * .75f + .25f, RValue(x, 75.1221f) * .75f + .25f) * .45f;
            
            _pLights[x] = new PLight(Vector3.zero, color * 1.3f, .2125f, 0);
        }
        
        _dLights[1] = new DLight(new Vector3(-.025f,  .6f, -.2f).normalized, new Vector3(.015f, .0275f, .0425f) * .25f);
        _dLights[0] = new DLight(new Vector3(     0, 1.25f,  12).normalized, new Vector3(.015f, .0375f, .0425f) * 2);
        _dLights[2] = new DLight(new Vector3(   -10,  .6f,   -1).normalized, new Vector3(.0425f, .015f, .0275f) * .4f);
        _dLights[3] = new DLight(new Vector3(    10,  .6f,   -4).normalized, new Vector3(.015f, .0425f, .0275f) * .3f);
        
        dLightBuffer = Buff.New(_dLights, Marshal.SizeOf<DLight>());
        Shader.SetGlobalBuffer(DLightBuffer, dLightBuffer);
    }
    
    
    private static float RValue(float v, float m)
    {
        return Mathf.Repeat(v * m, 1);
    }

    
    private void LateUpdate()
    {
        float t = GridTime.Now * speed;
        for (int x = 1; x < PLightCount; x++)
            _pLights[x].Update(t, x, scale);
        
        pLightBuffer.SetData(_pLights);

        if (Input.GetKeyDown(KeyCode.Alpha9))
            LightsOff = !LightsOff;
        
        Shader.SetGlobalFloat(LightsOn, LightsOff? 0 : 1);
    }


    public static void SetLightPos(Vector3 pos, float intensity)
    {
        float t = GridTime.Now * .5f;

        pos += (Mathf.Sin(t * 44.52f * .45f) * Vector3.right * .034f + Mathf.Sin(t * 34.52f * .45f) * Vector3.forward * .034f) * 3.5f;
        Vector3 c = Vector3.Lerp(new Vector3(.35f, .4f, .45f), new Vector3(.45f, .3f, .25f), Mathf.Pow(Mathf.Sin(t * 35) * .5f + .5f, 10) * .125f) * 1.25f * intensity;
        Inst._pLights[0] = new PLight(pos, c, (.2125f * .5f + Mathf.Sin(t * 54.52f) * .00615f * .1f) * .7f, 0);
    }
}
