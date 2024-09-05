using System.Collections;
using UnityEngine;


public class MadsSoundTrigger : MonoBehaviour
{
    public float volume;
    public string compareTag;
    
    public AudioSource source;
    
    [Space]
    //public AnimationCurve fadeInCurve;
    public float fadeInDuration;
    
    //public AnimationCurve fadeOutCurve;
    public float fadeOutDuration;
    
    private int anim;


    private void Start()
    {
        source.volume = 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(compareTag))
            StartCoroutine(VolumeAnim(true));
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(compareTag))
            StartCoroutine(VolumeAnim(false));
    }


    private IEnumerator VolumeAnim(bool fadeIn)
    {
        anim++;
        
        int myAnim = anim;
        
        float speed = 1f / (fadeIn? fadeInDuration : fadeOutDuration);
        //AnimationCurve curve = fadeIn? fadeInCurve : fadeOutCurve;
        
        float start = source.volume;
        float end = fadeIn? volume : 0;
        
        
        float t = 0;

        while (t < 1 && myAnim == anim)
        {
            t += Time.deltaTime * speed;
            
            source.volume = Mathf.Lerp(start, end, Mathf.Pow(t, 2));
            
            yield return null;
        }
    }
}
