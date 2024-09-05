using UnityEngine;
using ECM.Components;


public class LeanAwayForce : MonoBehaviour
{
    public Mesh rayMesh;
    public float rayDist, rayForce;
    public LayerMask walls;
    
    private int dirCount;
    private Vector3[] dirs;
    private float[] dirMulti;

    private Transform trans, head;
    private CharacterMovement movement;
    
    private readonly QuaternionForce wallForce = new QuaternionForce(300, 16.8f, 6.2f);
    private Quaternion smoothLeanAway = Quaternion.identity;
    
    
    private void Start()
    {
        FPS_Player player = GetComponent<FPS_Player>();

        movement = player.GetMovement;
        
        head  = player.head;
        trans = transform;
        
        dirs = rayMesh.vertices;
        dirCount = dirs.Length;
        dirMulti = new float[dirCount];
        for (int i = 0; i < dirCount; i++)
            dirMulti[i] = 1f - Mathf.Pow(Mathf.Abs(Vector3.Dot(dirs[i], Vector3.up)), 4);
    }

    
    public Quaternion LeanAwayRot(float dt)
    {
        Quaternion currentCamRot = head.rotation;
        
        Vector3 camPos = head.position;
            
        Vector3 bounceMV = Vector3.zero;
        
        for (int i = 0; i < dirCount; i++)
        {
            Vector3 d = currentCamRot * dirs[i];
            
            float multiDist = rayDist;
            float castDist  = Mathf.Max(multiDist, rayDist);
            
            if (Physics.Raycast(camPos, d, out RaycastHit hit, castDist, walls))
            {
                float dist = hit.distance;
                float push = (1f - Mathf.Min(dist, multiDist) / multiDist)
                             + Mathf.Pow(1f - Mathf.Min(dist, rayDist) / rayDist, 2) * .005f;
                              
                bounceMV -= d * push * rayForce * dirMulti[i];
            }
        }
        
        bounceMV = trans.InverseTransformDirection(bounceMV);
        bounceMV.x = Mathf.Pow(Mathf.Abs(bounceMV.x), 3) * Mathf.Sign(bounceMV.x);
        bounceMV.z = Mathf.Pow(Mathf.Abs(bounceMV.z), 3) * Mathf.Sign(bounceMV.z);
        
        Vector3 vel = movement.velocity;
        vel.y *= .66f;
        float velMulti = 1 + Mathf.Pow(vel.magnitude * .2125f, 7);
       
        smoothLeanAway = Quaternion.Slerp(smoothLeanAway, Quaternion.Euler(new Vector3(-bounceMV.z, 0, -bounceMV.x) * velMulti), dt * 3.65f * velMulti);
        
        return wallForce.Update(smoothLeanAway, dt);
    }
}
