using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class dateExt
{
    public static int Int(this DateTime date)
    {
        return date.Year + date.Month + date.Day + date.Hour + date.Minute + date.Second + date.Millisecond;
    }
}
