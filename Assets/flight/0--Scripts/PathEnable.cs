using UnityEngine;


public class PathEnable : MonoBehaviour
{
    private GameObject[] pieces;
    private int current = -1;
    private const int range = 4;
    private int count;
    
    
    private void Start()
    {
        count = transform.childCount;
        pieces = new GameObject[count];
        for (int i = 0; i < count; i++)
            pieces[i] = transform.GetChild(i).gameObject;
    }

    
    public void PathUpdate(float dist)
    {
        dist = FlightCore.WrapDist(dist - 200);
        int piece = Mathf.FloorToInt(dist / 4000);

        if (piece != current)
        {
            if (current != -1)
                for (int i = 0; i < range; i++)
                    pieces[(current + i) % count].SetActive(false);
            else
                for (int i = 0; i < count; i++)
                    pieces[i].SetActive(false);
            
            current = piece;
            
            for (int i = 0; i < range; i++)
                pieces[(current + i) % count].SetActive(true);
        }
    }
}
