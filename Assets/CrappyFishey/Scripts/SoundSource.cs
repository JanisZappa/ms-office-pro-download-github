using System.Text;
using UnityEngine;



public class SoundSource : MonoBehaviour
{
    public float volumeMulti, distanceMulti;
    
    private AudioSource source;//, source2;
    private AudioReverbFilter filter;// filter2;
    private Transform trans;
    
    private float vol, vol2;
    private int id;
    private Vector3 pos;
    
    private void Start()
    {
        source      = GetComponent<AudioSource>();
        source.time = Random.Range(0f, source.clip.length * .9f);
        filter      = GetComponent<AudioReverbFilter>();
        trans       = transform;
    }
    

    private void LateUpdate()
    {
        pos = Vector3.Lerp(pos, trans.position, Time.deltaTime);
        if (SoundData.AudioEnabled == 2)
        {
            vol  = 0;
            source.volume = 0;
            return;
        }
        
        float volGoal = Mathf.Pow(Mathf.Pow(SoundData.GetVolume(pos), 1 + distanceMulti), 10) * volumeMulti;
        
        const float speed = 14;
        vol  = Mathf.Lerp(vol, volGoal, Time.deltaTime * speed);
        vol2 = Mathf.Lerp(vol2, Mathf.Pow(volGoal, 4), Time.deltaTime * speed);
        EffectLerp();
    }

    private void EffectLerp()
    {
        source.volume = vol;
    
        //source.spatialBlend     = .2f + vol2 * .8f;
        source.spread     = (1f - vol2) * 90;
        filter.decayTime        =  2f - vol2 * 2f + 1.3f;
        filter.reverbDelay      =  1f - vol2 + .04f;
        filter.reflectionsLevel = -3000f + vol * 1934f;
    }


    private void OnEnable()
    {
        DebugUI.TL += OnDebugUI;
    }
    private void OnDisable()
    {
        DebugUI.TL -= OnDebugUI;
    }
    private void OnDebugUI(StringBuilder builder)
    {
        if(SoundData.AudioEnabled == 1)
            builder.AppendLine(source.volume.ToString("F3") + " | " + source.clip.name);
    }
}
