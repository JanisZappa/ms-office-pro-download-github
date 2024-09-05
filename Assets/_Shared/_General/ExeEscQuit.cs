using UnityEngine;
using UnityEngine.SceneManagement;


public class ExeEscQuit : MonoBehaviour 
{
    public bool noCursorChange;
    
    private static Vector2 lastPos;
    private static float lastMove;
    

    private void Awake()
    {
        lastPos  = Input.mousePosition;
        lastMove = float.MinValue;
    }


    private void Update()
    {
        if (!Application.isEditor && Input.GetKeyDown(KeyCode.Escape))
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        
        
        if(Input.GetKeyDown(KeyCode.F11))
            Screen.fullScreen = !Screen.fullScreen;


        Vector2 mousePos = Input.mousePosition;
        if (lastPos != mousePos)
        {
            lastPos = mousePos;
            lastMove = Time.realtimeSinceStartup;
        }
        
        if(!noCursorChange)
            Cursor.visible = lastMove >= Time.realtimeSinceStartup - 2;
    }
    
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Register()
    {
        if (!Application.isMobilePlatform && !Application.isEditor)
            SceneManager.sceneLoaded += PutInScene;
    }
    
    private static void PutInScene(Scene scene, LoadSceneMode mode)
    {
        if(FindObjectOfType<ExeEscQuit>() == null)
            new GameObject().AddComponent<ExeEscQuit>();
    }
}