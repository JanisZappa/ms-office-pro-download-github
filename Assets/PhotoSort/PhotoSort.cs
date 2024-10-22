using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class PhotoSort : MonoBehaviour
{
    public CityData cityData;

    private const string folderA = "/Users/andreasschonau/Desktop/PhotoCollection";
    private const string folderB = "/Users/andreasschonau/Desktop/PhotoResult";


    private class CopyInfo
    {
        public string original;
        public string copy;
        public string extension;
        public DateTime creationTime;
        public Vector3Int dateID;
        public string location;


        public CopyInfo(string original, string copy, string extension, DateTime creationTime, Vector3Int dateID, string location)
        {
            this.original     = original;
            this.copy         = copy;
            this.extension    = extension;
            this.creationTime = creationTime;
            this.dateID       = dateID;
            this.location     = location;
        }
    }


    private readonly List<string> dirs = new List<string>();


    private void Start()
    {
        for (int i = 0; i < 100; i++)
            Pad2[i] = i.ToString().PadLeft(2, '0');

        if (Directory.Exists(folderA) && Directory.Exists(folderB))
        {
            DeleteAll();
            AddDirectory(folderA);

            StartCoroutine(SlowGet());
        }   
    }


    private void DeleteAll()
    {
        DirectoryInfo di = new DirectoryInfo(folderB);

        foreach (FileInfo file in di.EnumerateFiles())
            file.Delete();
        foreach (DirectoryInfo dir in di.EnumerateDirectories())
            dir.Delete(true);
    }


    private void AddDirectory(string path)
    {
        dirs.Add(path);
        string[] subDirs = Directory.GetDirectories(path);
        int sCount = subDirs.Length;
        for (int i = 0; i < sCount; i++)
            AddDirectory(subDirs[i]);
    }


    private IEnumerator SlowGet()
    {
        int dCount = dirs.Count;
        Debug.Log("Dirs: " + dirs.Count);
        for (int i = 0; i < dCount; i++)
        {
            GetAllFileFromFolder(dirs[i]);
            Debug.Log("Scanned Folder " + i);
            yield return null;
        }

        Debug.Log("Scanned All Folders");

        StartCoroutine(SlowCopy());
    }



    private void GetAllFileFromFolder(string path)
    {
        string[] files = Directory.GetFiles(path);
        int fCount = files.Length;
        for (int i = 0; i < fCount; i++)
        {
            string original = files[i];
            string[] fParts = original.Split('/');
            int l = fParts.Length;
            
            FileInfo info = new FileInfo(original);
            
            if (!info.Attributes.HasFlag(FileAttributes.Hidden))
            {
                DateTime creationTime;
                string location = "";
                
                string extension = fParts[l - 1].Split('.')[1];
                if (extension == "jpeg" || extension == "jpg")
                {
                    ExifLib.JpegInfo inf = ExifLib.ExifReader.ReadJpeg(File.ReadAllBytes(original), "");
                    creationTime = inf.Date();
                    double[] x = inf.GpsLongitude;
                    double[] y = inf.GpsLatitude;
                    const float minute = 1f / 60, second = minute / 60f;
                    Vector2 coords = new Vector2((float)y[0] + (float)y[1] * minute + (float)y[2] * second, (float)x[0] + (float)x[1] * minute + (float)x[2] * second);
                    coords.y *= inf.GpsLongitudeRef == ExifLib.ExifGpsLongitudeRef.West ? -1 : 1;
                    coords.x *= inf.GpsLatitudeRef == ExifLib.ExifGpsLatitudeRef.South ? -1 : 1;

                    if (coords.magnitude > .00001f)
                    {
                        location = cityData.GetClosest(coords).Replace("\"", "").Replace("/", "_").Replace(" ", "_");
                    }
                }
                    
                else
                {
                    string date = fParts[l - 2];
                    string[] parts = date.Split(' ');
                    creationTime = new DateTime(int.Parse(parts[2]), monthNumbers[parts[1]], int.Parse(parts[0].Replace(".", "")));
                }

                string monthFolder = folderB + "/" + creationTime.Year + Pad2[creationTime.Month] + Months[creationTime.Month - 1];
                if (!Directory.Exists(monthFolder))
                    Directory.CreateDirectory(monthFolder);

                string destination = monthFolder + "/" + creationTime.Year + Pad2[creationTime.Month] + Pad2[creationTime.Day] + Days[(int)creationTime.DayOfWeek];
                Vector3Int dateID = new Vector3Int(creationTime.Year, creationTime.Month, creationTime.Day);

                if (fileID.TryGetValue(dateID, out int id))
                    fileID[dateID] = ++id;
                else
                    fileID.Add(dateID, 0);

                copyFiles.Add(new CopyInfo(original, destination, extension, creationTime, dateID, location));
            }
        }
    }


    private string ConvertFileName2(CopyInfo cI)
    {
        string extra = "";
        if (cI.extension == "jpeg" || cI.extension == "jpg")
            extra += "-" + Pad2[cI.creationTime.Hour] + Pad2[cI.creationTime.Minute];

        if (cI.location != "")
            extra += "-" + cI.location;
        
        int total = fileID[cI.dateID];
        if (total == 0)
            return cI.copy + extra + "." + cI.extension;

        int pad = total.ToString().Length;

        if (fileID2.TryGetValue(cI.dateID, out int id))
        {
            fileID2[cI.dateID] = ++id;
            return cI.copy + "-" + id.ToString().PadLeft(pad, '0') + extra + "." + cI.extension;
        }

        fileID2.Add(cI.dateID, 0);
        return cI.copy + "-" + 0.ToString().PadLeft(pad, '0') + extra  + "." + cI.extension;
    }


    private Dictionary<string, int> monthNumbers = new Dictionary<string, int>()
    {
        {"January",   1},
        {"February",  2},
        {"March",     3},
        {"April",     4},
        {"May",       5},
        {"June",      6},
        {"July",      7},
        {"August",    8},
        {"September", 9},
        {"October",  10},
        {"November", 11},
        {"December", 12}
    };

    private readonly string[] Days   = { "-SUN", "-MON", "-TUE", "-WED", "-THU", "-FRI", "-SAT" };
    private readonly string[] Months = { "-JAN", "-FEB", "-MAR", "-APR", "-MAY", "-JUN", "-JUL", "-AUG", "-SEP", "-OCT", "-NOV", "-DEC" };


    private readonly Dictionary<Vector3Int, int> fileID  = new Dictionary<Vector3Int, int>();
    private readonly Dictionary<Vector3Int, int> fileID2 = new Dictionary<Vector3Int, int>();

    private readonly List<CopyInfo> copyFiles = new List<CopyInfo>();
    private readonly string[] Pad2 = new string[100];


    private IEnumerator SlowCopy()
    {
        int cCount = copyFiles.Count;
        const float thresh = 1f / 2f;

        float t = Time.realtimeSinceStartup;
        for(int i = 0; i < cCount; i++)
        {
            CopyInfo cI = copyFiles[i];
            string destination = ConvertFileName2(cI);
            //Debug.Log(destination);
            File.Copy(cI.original, destination);

            if(Time.realtimeSinceStartup - t > thresh)
            {
                t += thresh;
                Debug.LogFormat("Copied {0} Files", i);
                yield return null;
            } 
        }

        Debug.LogFormat("Done Copying {0} Files", cCount);

        fileID.Clear();
        fileID2.Clear();
    }
}


public static class ExifInfoExt
{
    public static int Year (this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(0, 4)); }
    public static int Month(this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(5, 2)); }
    public static int Day  (this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(8, 2)); }

    public static int Hour   (this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(11, 2)); }
    public static int Minute (this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(14, 2)); }
    public static int Second (this ExifLib.JpegInfo jpi) { return int.Parse(jpi.DateTime.Substring(17, 2)); }

    public static DateTime Date (this ExifLib.JpegInfo jpi)
    {
        string dt = jpi.DateTime;
        return string.IsNullOrEmpty(dt) ? DateTime.Now : new DateTime(
            int.Parse(dt.Substring( 0, 4)),
            int.Parse(dt.Substring( 5, 2)),
            int.Parse(dt.Substring( 8, 2)),
            int.Parse(dt.Substring(11, 2)),
            int.Parse(dt.Substring(14, 2)),
            int.Parse(dt.Substring(17, 2)));
    }
}
