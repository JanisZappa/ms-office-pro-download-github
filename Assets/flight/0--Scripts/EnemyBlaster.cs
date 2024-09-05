using System.Collections.Generic;
using UnityEngine;

public class EnemyBlaster : MonoBehaviour
{
    public GameObject beamPrefab;
    
    private readonly Stack<Beam> pool = new Stack<Beam>();
    private readonly List<Beam> activeBeams = new List<Beam>(100);
    
    private Transform trans;
    
    private float blastTime, stateTime;
    
    private iPathInfo    arwing;
    private iPathInfo enemy;

    private bool shooting;
    
    private const float shotRate = .065f, shotLength = 1.6f, waitLength = 3;
    private Vector3 offset;

    
    private void Start()
    {
        arwing = GameObject.FindWithTag("Player").GetComponent<Arwing>();
        enemy  = GetComponent<EnemyTest>();
        
        for (int i = 0; i < 100; i++)
            pool.Push(Instantiate(beamPrefab).GetComponent<Beam>().Setup());
        
        trans = transform;
    }


    private void LateUpdate()
    {
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

        stateTime += Time.deltaTime;
        
        float threshTime = shooting? shotLength : waitLength;
        if (stateTime >= threshTime)
        {
            stateTime -= threshTime;
            shooting = !shooting;
            blastTime = 0;
        }
        
        const float missChance = .55f;
        offset = Vector3.Lerp(offset, new Vector3(Random.Range(-missChance, missChance), Random.Range(-missChance, missChance)), Time.deltaTime * .45f);
        if (shooting)
        {
            blastTime += Time.deltaTime;
            if (blastTime >= shotRate)
            {
                blastTime -= shotRate;
            
                Vector3 a = enemy.PathPos, b = arwing.PathPos;
                Vector3 dir = (b - a).normalized;
            
                
                dir = (dir + offset).normalized;
            
            
                Vector3 p = trans.position;
                Placement plc = FlightCore.Sample(a.z);
                Quaternion r  = plc.rot * Quaternion.LookRotation(dir, Vector3.up);
                
            
                activeBeams.Add(pool.Pop().Init(plc.rot * Vector3.forward, p, r, t, Random.Range(0, 2) == 0));
            }
        }
    }
}
