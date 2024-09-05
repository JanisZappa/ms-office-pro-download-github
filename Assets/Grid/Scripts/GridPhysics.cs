using System.Collections.Generic;
using UnityEngine;

public class GridPhysics : MonoBehaviour
{
    public int fps;
    private Stepper stepper;

    public delegate void GridUpdateStart();
    public static event GridUpdateStart OnGridUpdateStart;
    public delegate void GridUpdate(float dt);
    public static event GridUpdate OnGridUpdate;
    public delegate void GridUpdateDone();
    public static event GridUpdateDone OnGridUpdateDone;

    public const float YMulti = .625f;

    private static bool Draw;
    

    private void Start()
    {
        stepper = new Stepper(fps, Step);
    }


    private void Update()
    {
        OnGridUpdateStart?.Invoke();
        stepper.Update(Time.deltaTime);
        OnGridUpdateDone?.Invoke();

        if (Input.GetKeyDown(KeyCode.O))
            Draw = !Draw;
    }


    private void Step(float dt)
    {
        GridTime.AddTime(dt);
        OnGridUpdate?.Invoke(dt);
    }


    private static void DrawEllipse(Vector3 pos, float radius, Color color)
    {
        DRAW.Ellipse(new Vector3(pos.x, -pos.z, 1), radius, YMulti, 20).SetColor(color);
    }
    
    
    private static void DrawEllipse(int id)
    {
        Vector3 ellipse = ellipses[id];
        DRAW.Ellipse(new Vector3(ellipse.x, ellipse.y, 1), ellipse.z, YMulti, 20).SetColor(Color.yellow);
    }


    public static void DrawCloseColliders(Vector3 pos, float radius, Color color)
    {
        if(!Draw)
            return;
        
        DrawEllipse(pos, radius, color);
        Vector2Int tile = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        
        Vector3 p = new Vector3(tile.x + .5f, tile.y + .5f, 1);
        DRAW.Rectangle(p, Vector2.one).SetColor(color.A(.25f));
        DRAW.Rectangle(p, Vector2.one * 3).SetColor(color.A(.25f));

        int count = 0;
        compare.Clear();
        collect.Clear();
        for (int x = -1; x < 2; x++)
        for (int y = -1; y < 2; y++)
        {
            if (tiles.TryGetValue(new Vector2Int(tile.x + x, tile.y + y), out List<int> values))
            {
                count = values.Count;
                for (int i = 0; i < count; i++)
                    if(compare.Add(values[i]))
                        collect.Add(values[i]);
            }
        }

        count = collect.Count;
        for (int i = 0; i < count; i++)
            DrawEllipse(collect[i]);
    }


    public static void AddEllipse(Vector3 pos, float radius)
    {
        Vector2Int tile = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));
        int id = ellipses.Count;
        
        if (tiles.ContainsKey(tile))
            tiles[tile].Add(id);
        else
            tiles.Add(tile, new List<int>{id});
        
        ellipses.Add(new Vector3(pos.x, pos.z, radius));
    }


    private static readonly List<Vector3> ellipses = new List<Vector3>();
    private static readonly Dictionary<Vector2Int,List<int>> tiles = new Dictionary<Vector2Int, List<int>>();
    private static readonly HashSet<int> compare = new HashSet<int>();
    private static readonly List<int> collect = new List<int>();
}
