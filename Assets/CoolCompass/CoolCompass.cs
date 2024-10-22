using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif


public class CoolCompass : Singleton<CoolCompass>
{
    public TextAsset data;
    public TextAsset newData;
    public CityData cityData;

    private bool init;


    public static Vector3 ToPos(float lat, float lng)
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

        List<string> names = new List<string>();
        List<Vector2> latlng = new List<Vector2>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');

            string city = parts[0].Replace("\"", "");
            if (string.IsNullOrEmpty(city) || !used.Add(city))
                continue;

            float lat = float.Parse(parts[2].Replace("\"", ""));
            float lng = float.Parse(parts[3].Replace("\"", ""));

            names.Add(city);
            latlng.Add(new Vector2(lat, lng));
            pos.Add(ToPos(lat, lng));
        }

        cityData.names = names.ToArray();
        cityData.latLng = latlng.ToArray();
        cityData.pos = pos.ToArray();

#if UNITY_EDITOR
        EditorUtility.SetDirty(cityData);
#endif
    }


    private void ParseNewData()
    {
        string[] lines = newData.text.Split('\n');
        HashSet<string> used = new HashSet<string>();
        //List<string> collected = new List<string>();

        List<string> names = new List<string>();
        List<Vector2> latlng = new List<Vector2>();
        List<Vector3> pos = new List<Vector3>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrEmpty(line))
                continue;

            string[] parts = line.Split(',');

            string city = parts[1];
            if (string.IsNullOrEmpty(city) || !used.Add(city))
                continue;

            int pCount = parts.Length;
            string a = parts[pCount - 3];
            string b = parts[pCount - 2];
            float lat = 0, lng = 0;
            if (!float.TryParse(a, out lat))
                Debug.Log("No Lat " + a);

            if (!float.TryParse(b, out lng))
                Debug.Log("No Lng " + b);

            names.Add(city);
            latlng.Add(new Vector2(lat, lng));
            pos.Add(ToPos(lat, lng));
        }

        cityData.names = names.ToArray();
        cityData.latLng = latlng.ToArray();
        cityData.pos = pos.ToArray();

#if UNITY_EDITOR
        EditorUtility.SetDirty(cityData);
#endif
    }


    private void ParseBoth()
    {
        HashSet<string> used = new HashSet<string>();
        List<string> names = new List<string>();
        List<Vector2> latlng = new List<Vector2>();
        List<Vector3> pos = new List<Vector3>();

        {
            string[] lines = data.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');

                string city = parts[0].Replace("\"", "");
                if (string.IsNullOrEmpty(city) || !used.Add(city))
                    continue;

                float lat = float.Parse(parts[2].Replace("\"", ""));
                float lng = float.Parse(parts[3].Replace("\"", ""));

                names.Add(city);
                latlng.Add(new Vector2(lat, lng));
                pos.Add(ToPos(lat, lng));
            }
        }
        {
            string[] lines = newData.text.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] parts = line.Split(',');

                string city = parts[1];
                if (string.IsNullOrEmpty(city) || !used.Add(city))
                    continue;

                int pCount = parts.Length;
                string a = parts[pCount - 3];
                string b = parts[pCount - 2];
                float lat = 0, lng = 0;
                if (!float.TryParse(a, out lat))
                    Debug.Log("No Lat " + a);

                if (!float.TryParse(b, out lng))
                    Debug.Log("No Lng " + b);

                names.Add(city);
                latlng.Add(new Vector2(lat, lng));
                pos.Add(ToPos(lat, lng));
            }
        }

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

        if (data != null && newData != null)
            ParseBoth();
        else
        {
            if (data != null)
                ParseData();
            if (newData != null)
                ParseNewData();
        }

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


    public static Vector2 GetCoords(string name)
    {
        if (!Inst.init)
            Inst.Init();

        return Inst.cityData.GetCoords(name);
    }


    public static float GetAngleBetween(Vector3 p1, Vector3 p2)
    {
        Vector3 n = -p1.normalized;
        Plane plane = new Plane(n, p1);

        Vector3 north = plane.ClosestPointOnPlane(Vector3.up);
        p2 = plane.ClosestPointOnPlane(p2);

        return Vector3.SignedAngle((north - p1).normalized, (p2 - p1).normalized, n);
    }


    public static float AngleFromUser(Vector3 pos, Vector3 userPos)
    {
        return GetAngleBetween(userPos, pos);
    }


    public static float GetCoordAngle(Vector2 a, Vector2 b)
    {
        /*
        const y = Math.sin(λ2 - λ1) * Math.cos(φ2);
        const x = Math.cos(φ1) * Math.sin(φ2) -
                  Math.sin(φ1) * Math.cos(φ2) * Math.cos(λ2 - λ1);
        const θ = Math.atan2(y, x);
        const brng = (θ * 180 / Math.PI + 360) % 360; // in degrees
        */

        float lat1 = a.x * Mathf.Deg2Rad;
        float lat2 = b.x * Mathf.Deg2Rad;
        float lon1 = a.y * Mathf.Deg2Rad;
        float lon2 = b.y * Mathf.Deg2Rad;

        float y = Mathf.Sin(lon2 - lon1) * Mathf.Cos(lat2);
        float x = Mathf.Cos(lat1) * Mathf.Sin(lat2) -
                  Mathf.Sin(lat1) * Mathf.Cos(lat2) * Mathf.Cos(lon2 - lon1);
        float z = Mathf.Atan2(y, x);
        return -180 - z * Mathf.Rad2Deg; // in degrees
    }


    public static float GetCoordDist(Vector2 a, Vector2 b)
    {
        float d1 = a.x * Mathf.Deg2Rad;
        float num1 = a.y * Mathf.Deg2Rad;
        float d2 = b.x * Mathf.Deg2Rad;
        float num2 = b.y * Mathf.Deg2Rad - num1;
        float d3 = Mathf.Pow(Mathf.Sin((d2 - d1) * .5f), 2f) + Mathf.Cos(d1) * Mathf.Cos(d2) * Mathf.Pow(Mathf.Sin(num2 * .5f), 2);

        return 6376.5f * (2 * Mathf.Atan2(Mathf.Sqrt(d3), Mathf.Sqrt(1f - d3)));
    }
}
