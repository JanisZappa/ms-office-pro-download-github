using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScreen : MonoBehaviour
{
    
    private void Start()
    {
        /*#if !UNITY_EDITOR && UNITY_STANDALONE_OSX
                enabled = false;
                return;
        #endif*/

        if (Application.isMobilePlatform)
        {
            Destroy(gameObject);
            return;
        }
        
        PhoneWindow();
    }


    private void PhoneWindow()
    {
        float dpi = Screen.dpi;
        Debug.Log(dpi);
        float dpcm = 52;//dpi / 2.54f;
        Debug.Log(dpcm);
        int screenX = Screen.resolutions[Screen.resolutions.Length - 1].width;
        Debug.Log(screenX);
        float phonecm = 10.33f;
        float screencm = (float)screenX / dpcm;
        Debug.Log(screencm);
        int phonex = Mathf.RoundToInt(screenX * (phonecm / screencm));
        int phoney = Mathf.RoundToInt((float) phonex / 16f * 9f);
        Screen.SetResolution(phonex, phoney, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
