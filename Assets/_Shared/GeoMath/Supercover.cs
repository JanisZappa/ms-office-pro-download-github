using UnityEngine;


public static class SuperCover 
{
    private static readonly Vector2Int[] cells = new Vector2Int[100];
    
    public static Vector2Int[] GetLineCells(Vector2 p0, Vector2 p1, out int count)
    {
        Vector2 dir = p1 - p0;
        float dx = Mathf.Sqrt(1 + Mth.IntPow(dir.y / dir.x, 2));
        float dy = Mathf.Sqrt(1 + Mth.IntPow(dir.x / dir.y, 2));
        
        int cx = Mathf.FloorToInt(p0.x), 
            cy = Mathf.FloorToInt(p0.y);
        
        int sx = dir.x < 0? -1 : 1;
        int sy = dir.y < 0? -1 : 1;

        float ox = (dir.x < 0 ? p0.x - cx : cx + 1 - p0.x) * dx;
        float oy = (dir.y < 0 ? p0.y - cy : cy + 1 - p0.y) * dy;
        
        float length = dir.sqrMagnitude;

        int index = 0;
        while (true)
        {
            cells[index++] = new Vector2Int(cx, cy);
            
            if (Mathf.Min(ox * ox, oy * oy) < length)
            {
                if (ox < oy)
                {
                    ox += dx;
                    cx += sx;
                }
                else
                {
                    oy += dy;
                    cy += sy;
                }
            }
            else
                break;
        }

        count = index;
        return cells;
    }
}
