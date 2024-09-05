using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OkLabTest : MonoBehaviour
{
    [Range(0, 1)] public float L, a, b;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Color c = new OkLab(L, a, b).ToColor();
        DRAW.Circle(new Vector3(0, 0, -1), 3, 40).SetColor(c).Fill(1);
    }
}
