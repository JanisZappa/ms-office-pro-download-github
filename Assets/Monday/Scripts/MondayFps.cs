using TMPro;
using UnityEngine;

public class MondayFps : MonoBehaviour
{
    private TextMeshPro tmp;
    private float delta;
    private float checkTime;
    private int fps, frames;


    private void Start()
    {
        tmp = GetComponent<TextMeshPro>();
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
            
            tmp.text = fps.PrepString();
        }
    }
}
