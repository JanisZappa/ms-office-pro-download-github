using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class ForceTweaker : ScriptableObject
{
    [System.Serializable]
    public class ForceSpeedDamp
    {
        public string forceName;
        public float speed;
        public float damp;
    }
    
    public ForceSpeedDamp[] forceTweaks;

    private Dictionary<string, ForceSpeedDamp> map;


    public void Tweak(string name, ISetSpeedDamp force)
    {
        if (map == null)
        {
            map = new Dictionary<string, ForceSpeedDamp>();
            int count = forceTweaks.Length;
            for (int i = 0; i < count; i++)
            {
                ForceSpeedDamp fsd = forceTweaks[i];
                map.Add(fsd.forceName, fsd);
            }
        }

        if (map.TryGetValue(name, out ForceSpeedDamp value))
        {
            force.SetSpeedDamp(value.speed, value.damp);
        }
    }
}
