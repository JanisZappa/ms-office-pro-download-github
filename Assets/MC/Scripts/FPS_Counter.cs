using UnityEngine;


public class FPS_Counter : MonoBehaviour
{
    private GUI_Txt txt;
    private float delta;
    private float checkTime;
    private int fps, frames;
    
    private void Start()
    {
        txt = GetComponent<GUI_Txt>();
    }

    
    private void LateUpdate()
    {
        delta += Time.deltaTime;
        frames++;
        float now = Time.realtimeSinceStartup;
        if (now - checkTime > .125f)
        {
            float average = delta / frames;
            fps = Mathf.FloorToInt(1f / average);
            delta = 0;
            frames = 0;
            checkTime = now;
        }
        
        
        txt.Begin(Color.white, 12, TextAnchor.UpperLeft);
        txt.Add("FPS: ");
        txt.Add(fps.PrepString(), true);
    }
}