using UnityEngine;


public class GridCursor : MonoBehaviour
{
    public bool show;
    
    private Vector3 pos;
    
    private static bool Draw;
    
    
    private void Start()
    {
        Cursor.visible = show;
    }
}
