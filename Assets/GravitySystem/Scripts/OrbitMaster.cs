using System.Collections.Generic;
using UnityEngine;


public class OrbitMaster : MonoBehaviour
{
    public float speed;
    private float t;
    
    private static readonly List<Orbiter> Orbiter = new List<Orbiter>();
    private static int Count;
    
    private const int maxPlanets = 128;
    private ComputeBuffer planetBuffer;
    private readonly Vector4[] planetData = new Vector4[maxPlanets];
    private static readonly int PlanetBuffer = Shader.PropertyToID("PlanetBuffer");
    private static readonly int PlanetCount = Shader.PropertyToID("PlanetCount");
    
    
    private const int maxLights = 128;
    private ComputeBuffer lightBuffer;
    private readonly Vector4[] lightData = new Vector4[maxLights];
    private static readonly int LightBuffer = Shader.PropertyToID("LightBuffer");
    private static readonly int LightCount = Shader.PropertyToID("LightCount");


    private void Start()
    {
        planetBuffer = Buff.New(planetData, 16);
        Shader.SetGlobalBuffer(PlanetBuffer, planetBuffer);
        
        lightBuffer = Buff.New(lightData, 16);
        Shader.SetGlobalBuffer(LightBuffer, lightBuffer);
    }
    
    
    private void Update()
    {
        t += Time.deltaTime * speed;
      
        int lCount = 0;
        int pCount = 0;
        for (int i = 0; i < Count; i++)
            if (Orbiter[i].lit)
            {
                Vector4 lD = Orbiter[i].OrbitUpdate(t);
                lD.w = 1 / lD.w;
                lightData[lCount++] = lD;
            }
            else
                planetData[pCount++] = Orbiter[i].OrbitUpdate(t);
            
            
        lightBuffer.SetData(lightData);
        Shader.SetGlobalInt(LightCount, lCount);
        
        planetBuffer.SetData(planetData);
        Shader.SetGlobalInt(PlanetCount, pCount);
    }


    public static void AddOrbiter(Orbiter orbiter)
    {
        for (int i = 0; i < Count; i++)
            if (Orbiter[i].chainID > orbiter.chainID)
            {
                Orbiter.Insert(i, orbiter);
                Count++;
                return;
            }
        
        Orbiter.Add(orbiter);
        Count++;
    }
}
