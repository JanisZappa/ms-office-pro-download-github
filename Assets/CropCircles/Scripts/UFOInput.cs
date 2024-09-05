using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UFOInput
{
    public static float P1_Horizontal
    {
        get { return Input.GetAxis("Horizontal"); }
    }
    
    public static float P1_Vertical
    {
        get { return Input.GetAxis("Vertical"); }
    }
}
