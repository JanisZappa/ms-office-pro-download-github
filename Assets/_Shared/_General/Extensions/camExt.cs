using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class camExt
{
    public static Ray CursorRay(this Camera cam)
    {
        return cam.ScreenPointToRay(Input.mousePosition);
    }
    
    public static Vector3 CursorCastPos(this Camera cam, Plane plane)
    {
        Ray r = cam.CursorRay();
        
        return plane.Raycast(r, out float enter)? r.origin + r.direction * enter : r.origin;
    }
    
    public static Vector3 CursorOrthoPos(this Camera cam)
    {
        return cam.CursorCastPos(new Plane(Vector3.forward, Vector3.zero));
    }
}
