using System.Xml.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BendControll : MonoBehaviour
{
    public static bool flat
    {
        get
        {
            return PlayerPrefs.GetInt("WorldIsFlat") == 1;
        }
        private set
        {
            PlayerPrefs.SetInt("WorldIsFlat", value? 1 : 0);
        }
    }
    
    private const int res = 128;
    private static readonly    Color32[] colors = new Color32[res];
    private static readonly FloatForce[] forces = new FloatForce[res];
    public Texture2D tex;
    private static readonly int bendTex = Shader.PropertyToID("BendTex");
    
    private Stepper stepper;
    private Transform trans;
    
    private void Start()
    {
        tex = new Texture2D(res, 1, TextureFormat.RGBA32, false) {filterMode = FilterMode.Trilinear, wrapMode = TextureWrapMode.Repeat};
        Shader.SetGlobalTexture(bendTex, tex);

        for (int i = 0; i < res; i++)
            forces[i] = new FloatForce(7, .7f).SetValue(.5f);
        
        stepper = new Stepper(200, Step);
        
        trans = Camera.main.transform;
    }


    private void Step(float dt)
    {
        Vector3 fwd = trans.forward.SetY(0).normalized;
        const float step = 1f / res;
        for (int i = 0; i < res; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(180 + i * step * 360, Vector3.up) * Vector3.forward;
            float dot = Vector3.Dot(fwd, dir);
                  dot = Mathf.SmoothStep(-1, 1, Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, dot *.5f + .5f)));
            forces[i].Update(.5f + dot * .1f, dt);
        }
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            Toggle();
    }


    public static void Toggle()
    {
        flat = !flat;
        Shader.DisableKeyword( flat? "FLAT_OFF" : "FLAT_ON");
        Shader.EnableKeyword (!flat? "FLAT_OFF" : "FLAT_ON");
    }


    private void LateUpdate()
    {
        stepper.Update(Time.deltaTime);
        
        const float step = 1f / res;
        for (int i = 0; i < res; i++)
        {
            float radius = .5f + .5f * Mathf.Sin(i * step * Mathf.PI * 6 - Mathf.PI * .5f + Time.realtimeSinceStartup * 2);
            radius = forces[i].Value;
            byte bt = (byte)(255 * radius);
            colors[i] = new Color32(bt, bt, bt, bt);
        }
        
        tex.SetPixels32(colors);
        tex.Apply();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(BendControll))]
public class BendControllEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        BendControll bC = target as BendControll;
        if (GUILayout.Button(BendControll.flat? "Make Round" : "Make Flat"))
        {
            BendControll.Toggle();
            EditorWindow.GetWindow<SceneView>().Repaint();
            GetMainGameView().Repaint();
        }
           
    }
    
    
    public static EditorWindow GetMainGameView()
    {
        var assembly = typeof(EditorWindow).Assembly;
        var type = assembly.GetType("UnityEditor.GameView");
        var gameview = EditorWindow.GetWindow(type);
        return gameview;
    }
}
#endif