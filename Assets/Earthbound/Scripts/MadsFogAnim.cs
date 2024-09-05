using System.Collections;
using UnityEngine;

public class MadsFogAnim : MonoBehaviour
{
    public Camera cam;
    public Color camBG;
    public AnimationCurve fogCurve;
    
    [Space]
    public float startWaitDuration;
    public float colorAnimDuration;
    public float fogAnimDuration;
    
    private void Start()
    {
        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        cam.backgroundColor = Color.black;
        RenderSettings.fogDensity = 0;
        
        yield return new WaitForSeconds(startWaitDuration);
        
        float t = 0;
        float s = 1f / colorAnimDuration;
        while (t < 1)
        {
            t += Time.deltaTime * s;
            cam.backgroundColor = Color.Lerp(Color.black, camBG, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }
        
        t = 0;
        s = 1f / fogAnimDuration;
        while (t < 1)
        {
            t += Time.deltaTime * s;
            RenderSettings.fogDensity = fogCurve.Evaluate(Mathf.Clamp01(t));
            yield return null;
        }
    }
}
