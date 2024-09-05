using System.Collections;
using UnityEngine;

public class CastleFade : MonoBehaviour
{
    private float fade;

    private bool fadeIn, fading;
    private static readonly int SceneFade = Shader.PropertyToID("SceneFade");

    
    private void Start()
    {
        StartCoroutine(FadeAnim());
    }

    private void Update()
    {
        if (!fading && Input.GetKeyDown(KeyCode.F3))
            StartCoroutine(FadeAnim());      
        
        Shader.SetGlobalFloat(SceneFade, fade);
        //Shader.SetGlobalFloat(SceneFade, Mathf.SmoothStep(0, 1, 1f - Mathf.Pow(1f - Mathf.Pow(fade, 2), 4)));
    }


    private IEnumerator FadeAnim()
    {
        fadeIn = !fadeIn;
        fading = true;
        float t = 0;
        float start = fade;
        float end = fadeIn ? 1 : 0;
        while (t < 1)
        {
            t += Time.deltaTime * .77777f;
            fade = Mathf.SmoothStep(start, end, t);
            if(t < 1)
            yield return null;
        }

        fading = false;
    }
}
