using System.Collections.Generic;
using UnityEngine;


public class SwitchScreen : MonoBehaviour
{
    private int windowed;
    
    //  0  Fullscreen
    //  1  Window 720p
    //  2  Window 450p
    //  3  Window 225p


    private List<Vector2Int> res = new List<Vector2Int>();
    private int count;

    private void Start()
    {
        #if !UNITY_EDITOR && UNITY_STANDALONE_OSX || UNITY_EDITOR
        enabled = false;
        return;
        #endif

        Camera cam = Camera.main;
        if (cam == null)
            cam = FindObjectOfType<Camera>();

        Resolution[] r = Screen.resolutions;
        int c = r.Length;
        Resolution biggest = r[r.Length - 1];
        float aspect = biggest.width * 1f / biggest.height;
        
        for (int i = c - 1; i >= 0; i--)
        {
            Resolution rr = r[i];
            int x = rr.width, y = rr.height;
            float a = (float)x / y;
            if (Mathf.Abs(aspect - a) > .1f)
                goto skip;
            
            /*for (int e = 0; e < count; e++)
                if (res[e].y == y)
                    goto skip;*/
            
            if(count > 0 && (float)y / res[count - 1].y > .65f)
                goto skip;
            
            res.Add(new Vector2Int(x, y));
            count++;
                
            skip: ;
        }

        int lowesty = res[res.Count - 1].y;
        if(lowesty > 720)
            res.Add(new Vector2Int(Mathf.RoundToInt(720 * aspect), 720));
        if(lowesty > 450)
            res.Add(new Vector2Int(Mathf.RoundToInt(450 * aspect), 450));
        if(lowesty > 225)
            res.Add(new Vector2Int(Mathf.RoundToInt(225 * aspect), 225));
        count = res.Count;
        
        if (Application.isMobilePlatform)
        {
            Destroy(gameObject);
            return;
        }

        windowed = 0;//Screen.fullScreen? 0 : Screen.height == 720? 1 : Screen.height == 450? 2 : 3;
        
        UpdateRes();
        
        
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            windowed = (windowed + 1) % count;

            UpdateRes();
        }
    }


    private void UpdateRes()
    {
        if(windowed == 0)
            Screen.SetResolution(res[0].x, res[0].y, true);
        else
            Screen.SetResolution(res[windowed].x, res[windowed].y, false);
        
        /*switch (windowed)
        {
            case 0:
                Screen.SetResolution(1920, 1080, true);
                break;
                
            case 1:
                Screen.SetResolution(1280, 720, false);
                break;
                
            case 2:
                Screen.SetResolution(800, 450, false);
                break;
                
            case 3:
                Screen.SetResolution(400, 225, false);
                break;
        }*/
    }
}
