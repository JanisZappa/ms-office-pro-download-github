using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinSource : MonoBehaviour
{
    public float volumeMulti, distanceMulti;
    
    private AudioSource source;//, source2;
    private AudioReverbFilter filter;// filter2;
    private Transform trans;
    
    private float vol;
    private int id;
    
    [Space]
    [Range(1,20000)]  //Creates a slider in the inspector
    public float frequency1;
 
    [Range(1,20000)]  //Creates a slider in the inspector
    public float frequency2;
 
    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;
 
    int timeIndex = 0;
 
    void Start()
    {
        source      = GetComponent<AudioSource>();
        source.time = Random.Range(0f, source.clip.length * .9f);
        filter      = GetComponent<AudioReverbFilter>();
        trans       = transform;
        
        //source.clip = null;
    }
   
    private void LateUpdate()
    {
        if (SoundData.AudioEnabled == 2)
        {
            vol  = 0;
            source.volume = 0;
            return;
        }
        
        vol = Mathf.Lerp(vol, SoundData.GetVolume(trans.position), Time.deltaTime * .5f);
        
        EffectLerp();
    }

    private void EffectLerp()
    {
        source.volume = Mathf.Pow(Mathf.Pow(vol, 1 + distanceMulti), 20) * volumeMulti;
        
        float vol2 = Mathf.Pow(vol, 4);
        //source.spatialBlend     = .2f + vol2 * .8f;
        source.spread     = (1f - vol2) * 180;
        filter.decayTime        =  2f - vol2 * 2f + 1.3f;
        filter.reverbDelay      =  1f - vol2 + .04f;
        filter.reflectionsLevel = -3000f + vol * 1934f;
    }
    
   
    void OnAudioFilterRead(float[] data, int channels)
    {
        for(int i = 0; i < data.Length; i+= channels)
        {          
            data[i] = CreateSine(timeIndex, frequency1, sampleRate);
           
            if(channels == 2)
                data[i+1] = CreateSine(timeIndex, frequency2, sampleRate);
           
            timeIndex++;
           
            //if timeIndex gets too big, reset it to 0
            if(timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }
   
    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * timeIndex * frequency / sampleRate);
    }
    
    
}