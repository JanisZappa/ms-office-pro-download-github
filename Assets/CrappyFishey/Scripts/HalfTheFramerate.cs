using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfTheFramerate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
