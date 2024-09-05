using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;   
#endif

public class CoolCompass : Singleton<CoolCompass>
{
    public TextAsset data;
    public CityData cityData;

    private bool init;


    private Vector3 ToPos(float lat, float lng)
    {
        lng *= Mathf.Deg2Rad;
        lat *= Mathf.Deg2Rad;

        float latCos = Mathf.Cos(lat);
        
        return new Vector3(latCos * Mathf.Cos(lng), Mathf.Sin(lat), latCos * Mathf.Sin(lng));
    }


    private void ParseData()
    {
        string[] lines = data.text.Split('\n');
        HashSet<string> used = new HashSet<string>();
        //List<string> collected = new List<string>();
        
        List<string> names   = new List<string>();
        List<Vector2> latlng = new List<Vector2>();
        List<Vector3> pos    = new List<Vector3>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');

            string city = parts[0].Replace("\"", "");
            if(string.IsNullOrEmpty(city) || !used.Add(city))
                continue;
            
            float lat = float.Parse(parts[2].Replace("\"", ""));
            float lng = float.Parse(parts[3].Replace("\"", ""));
            
            names.Add(city);
            latlng.Add(new Vector2(lat, lng));
            pos.Add(ToPos(lat, lng));
            
            //collected.Add(city + "|" + lat + "|" + lng);
        }

        //DesktopTxt.Write("Cities", collected.ToArray());

        cityData.names = names.ToArray();
        cityData.latLng = latlng.ToArray();
        cityData.pos = pos.ToArray();
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(cityData);
#endif
    }


    private void Init()
    {
        init = true;
        
        if (data != null)
            ParseData();
        
        cityData.Init();
    }
    
    
    private void Start()
    {
        if (!init)
            Init();
    }

    public static Vector3 GetPos(string name)
    {
        if (!Inst.init)
            Inst.Init();
        
        return Inst.cityData.GetPos(name);
    }


    public static float GetAngleBetween(Vector3 p1, Vector3 p2)
    {
        Vector3 n = -p1.normalized;
        Plane plane = new Plane(n, p1);
       
        Vector3 north = plane.ClosestPointOnPlane(Vector3.up);
        p2 = plane.ClosestPointOnPlane(p2);

        return Vector3.SignedAngle((north - p1).normalized, (p2 - p1).normalized, n);
    }


    public static float AngleFromCologne(Vector3 pos)
    {
        return GetAngleBetween(GetPos("Cologne"), pos);
    }

    public static float GetDistanceFromCologne(Vector3 pos)
    {
        return GetDistance(GetPos("Cologne"), pos);
    }
    

    public static float GetDistance(Vector3 p1, Vector3 p2)
    {
        const float radius = 6371;
        const float halfdist = Mathf.PI * radius;
        return Vector3.Angle(p1.normalized, p2.normalized) / 180f * halfdist;
    }
}
