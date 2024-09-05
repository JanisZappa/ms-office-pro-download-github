using System;
#if UNITY_STANDALONE_WIN
using System.Runtime.InteropServices;
#endif
using UnityEngine;


public class QuickBuildVariation : MonoBehaviour
{
    public int variations;
    
#if UNITY_STANDALONE_WIN
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    private static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);
    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    private static extern IntPtr FindWindow(System.String className, System.String windowName);

    private static void SetPosition(string name, int x, int y, int resX = 0, int resY = 0) {
        SetWindowPos(FindWindow(null, name), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }


    public void VariationWindowPos(float x, float y, int margin = 0)
    {
        int id = int.Parse(Application.productName.Split('_')[1]);
        SetPosition(Application.productName, (int)(x + margin) * id, 0);
    }
#endif
}
