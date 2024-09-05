using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct WheelSettings
{
    public float radius, thickness;
    public WheelDrag drag;
}
[System.Serializable]
public struct SpringSettings
{
    public float length, strength, damping;
}

[System.Serializable]
public struct WheelDrag
{
    public float forward, sideways;
}

