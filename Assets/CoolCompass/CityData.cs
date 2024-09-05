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


    public void Init()
    {
        posMap = new Dictionary<string, Vector3>();
        int count = names.Length;
        for (int i = 0; i < count; i++)
        {
            posMap.Add(names[i], pos[i]);
        }
    }


    public Vector3 GetPos(string name)
    {
        return posMap.TryGetValue(name, out Vector3 p) ? p : Vector3.zero;
    }
}
