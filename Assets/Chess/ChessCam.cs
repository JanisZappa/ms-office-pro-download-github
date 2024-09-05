using System.Collections;
using TMPro;
using UnityEngine;


public class ChessCam : Singleton<ChessCam>
{
    private static Camera Cam;
    private TextMeshProUGUI friendCode;
    
    private float ortho;
    private static Bounds currentBounds;
    
    public static bool Zoomed, Animating;
    
    
    private void Start()
    {
        if (!Application.isEditor && !Application.isMobilePlatform)
        {
            float a = 561f / 1080f * Screen.currentResolution.height;
            float b = 315f / 561f * a;
            Screen.SetResolution((int)b, (int)a, false);
            
            #if UNITY_STANDALONE_WIN
            QuickBuildVariation qV = FindObjectOfType<QuickBuildVariation>();
            if (qV != null)
                qV.VariationWindowPos(b, a, 5);
            #endif
        }
    }


    public static void FrameGames()
    {
        if(Cam == null)
            Cam = Inst.GetComponent<Camera>();
        
        SetBounds(ChessApp.GetAllGameBounds());
    }


    private static void SetBounds(Bounds b)
    {
        currentBounds = b;
        
        float s = b.size.x * .5f / Cam.aspect;
        
        Cam.orthographicSize   = s;
        Cam.transform.position = b.center.SetY(10) + Vector3.forward * (b.size.z * .5f - s);
        Cam.transform.GetChild(0).localScale = new Vector3(s / 8f, 1, s / 8.5f);
    }
    
    
    public static Vector3 TouchPos { get { return Cam.ScreenPointToRay(Input.mousePosition).origin; }}

    
    


    public static Vector2 ScreenPos(Vector3 worldPos)
    {
        return Cam != null? (Vector2)Cam.WorldToScreenPoint(worldPos) : Vector2.zero;
    }


    private void LateUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(90 + (ChessUI.IsActive? 180 : 0), Vector3.right);
    }


    public static void Zoom(int game)
    {
        Zoomed = game != -1;
        Inst.StartCoroutine(ZoomAnim(Zoomed? ChessApp.GameBounds(ChessApp.IdToGamePos(game)) : ChessApp.GetAllGameBounds()));
    }


    private static IEnumerator ZoomAnim(Bounds b)
    {
        Animating = true;
        
        float t = 0;
        
        Bounds a = currentBounds;

        while (t < 1)
        {
            t += Time.deltaTime * 2.5f;
            float l = Mathf.SmoothStep(0, 1, t);
            
            Vector3 c = Vector3.Lerp(a.center, b.center, l);
            Vector3 s = Vector3.Lerp(a.size, b.size, l);
            
            SetBounds(new Bounds(c, s));
            
            yield return null;
        }
        
        Animating = false;
    }
}
