using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CastleLayers
{
    public static int Default = LayerMask.NameToLayer("Default");
    public static int Walls   = LayerMask.NameToLayer("HardWalls");
    public static int Floor   = LayerMask.NameToLayer("Ground");

    public static int Mask_Default = 1 << Default;
    public static int Mask_Walls   = 1 << Walls;
    public static int Mask_Floor   = 1 << Floor;
}
