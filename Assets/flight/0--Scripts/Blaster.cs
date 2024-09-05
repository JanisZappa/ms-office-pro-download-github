using System.Collections.Generic;
using UnityEngine;


public class Blaster : MonoBehaviour
{
    public GameObject beamPrefab;
    
    private readonly Stack<Beam> pool = new Stack<Beam>();
    private readonly List<Beam> activeBeams = new List<Beam>(100);
    
    private Transform trans;
    
    private bool single;
    
    private iPathInfo arwing;


    private void Start()
    {
        for (int i = 0; i < 100; i++)
            pool.Push(Instantiate(beamPrefab).GetComponent<Beam>().Setup());
        
        trans = transform;
        
        arwing = GetComponent<Arwing>();
    }


    private void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
            single = !single;
        
        float t = FlightCore.CurrentTime;
        
        //  Trimm  //
        int count = activeBeams.Count;
        
        for (int i = 0; i < count; i++)
        {
            Beam b = activeBeams[i];
            if (b.CanBeRemoved(t))
            {
                activeBeams.RemoveAt(i);
                pool.Push(b);
                i--;
                count--;
            }
        }
        
        for (int i = 0; i < count; i++)
            activeBeams[i].Hit(t);
        
        
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 p = trans.position;
            Quaternion r = trans.rotation;
            Placement plc = FlightCore.Sample(arwing.PathPos.z);
            Placement plc2 = FlightCore.Sample(arwing.PathPos.z + 1000);
            
            Quaternion look = Quaternion.LookRotation((plc2.pos - plc.pos).normalized, plc.up);
            
            
            if(single)
                activeBeams.Add(pool.Pop().Init(look * Vector3.forward, p, r, t, Random.Range(0, 2) == 0));
            else
            {
                activeBeams.Add(pool.Pop().Init(look * Vector3.forward, p + r * new Vector3(3.21f, -.4f, 1.2f), r, t, false));
                activeBeams.Add(pool.Pop().Init(look * Vector3.forward, p + r * new Vector3(-3.21f, -.4f, 1.2f), r, t, true));
            }
        }
    }
}
