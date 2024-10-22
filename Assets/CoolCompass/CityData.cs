using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class CityData : ScriptableObject
{
    public string[] names;
    public Vector2[] latLng;
    public Vector3[] pos;


    private Dictionary<string, Vector3> posMap;
    private Dictionary<string, Vector2> coordMap;


    public void Init()
    {
        posMap = new Dictionary<string, Vector3>();
        coordMap = new Dictionary<string, Vector2>();
        int count = names.Length;
        for (int i = 0; i < count; i++)
        {
            posMap.Add(names[i], pos[i]);
            coordMap.Add(names[i], latLng[i]);
        }
    }


    public Vector3 GetPos(string name)
    {
        return posMap.TryGetValue(name, out Vector3 p) ? p : Vector3.zero;
    }

    public Vector2 GetCoords(string name)
    {
        return coordMap.TryGetValue(name, out Vector2 p) ? p : Vector2.zero;
    }



    public string GetClosest(Vector2 coord)
    {
        float best = float.MaxValue;
        int id = -1;
        int count = latLng.Length;
        for(int i = 0; i < count; i++)
        {
            float m = CoolCompass.GetCoordDist(coord, latLng[i]);
            if(m < best)
            {
                best = m;
                id = i;
            }
        }

        return id != -1 ? names[id] : "";
    }
}
