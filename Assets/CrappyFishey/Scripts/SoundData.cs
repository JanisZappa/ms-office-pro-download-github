using System.IO;
using UnityEngine;


public class SoundData : MonoBehaviour
{
    public TextAsset data;
    
    [Space]
    public int samplePts;
    public int roomPts;
    public int usedSamplePts;
    public int usedRoomPts;
    
    [Space]
    public Vector3 min;
    public Vector3Int dimensions;
    public float step;
    public int yMulti;
    
    [Space]
    public Vector3 min2;
    public Vector3Int dimensions2;
    public float step2;
    public int yMulti2;
    
    private byte[] volume;
    
    private static SoundData Inst;

    
    #region Helper Vars
    private float stepMulti, stepMultiY;
    private int yDimensionMulti;
    private float stepMulti2, stepMultiY2;
    private int yDimensionMulti2;
    #endregion
    
    
    private Transform cam;
    private int frame = -1000;
    private int camIndex;
    
    
    private void Start()
    {
        Inst = this;
        
        Check.It();
        using (MemoryStream m = new MemoryStream(data.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            samplePts     = r.ReadInt32();
            roomPts       = r.ReadInt32();
            usedSamplePts = r.ReadInt32();
            usedRoomPts   = r.ReadInt32();
                
            step       = r.ReadSingle();
            min        = r.ReadVector3();
            dimensions = new Vector3Int(r.ReadInt32() + 1, r.ReadInt32() + 1, r.ReadInt32() + 1);
            yMulti     = r.ReadInt32();
            
            step2       = r.ReadSingle();
            min2        = r.ReadVector3();
            dimensions2 = new Vector3Int(r.ReadInt32() + 1, r.ReadInt32() + 1, r.ReadInt32() + 1);
            yMulti2     = r.ReadInt32();
            
            int[]ptMap = new int[usedSamplePts];
            for (int i = 0; i < usedSamplePts; i++)
                ptMap[i] = r.ReadInt32();
            
            int[]roomMap = new int[usedRoomPts];
            for (int i = 0; i < usedRoomPts; i++)
                roomMap[i] = r.ReadInt32();
            
            //  The data is structured this way:
            //  A Sample Point and its values for all Room Positions
            
            volume = new byte[samplePts * roomPts];
            
            for (int i = 0; i < usedSamplePts; i++)
            {
                //  We offset the start position by its index * Total Room Points
                int offset = ptMap[i] * roomPts;

                for (int e = 0; e < usedRoomPts; e++)
                    volume[offset + roomMap[e]] = r.ReadByte(); 
            }
        }
        
        //  Helper  //
        stepMulti       = 1f / step;
        stepMultiY      = 1f / (step * yMulti);
        yDimensionMulti = dimensions.x * dimensions.z;
        
        stepMulti2       = 1f / step2;
        stepMultiY2      = 1f / (step2 * yMulti2);
        yDimensionMulti2 = dimensions2.x * dimensions2.z;
        
        if(Application.isEditor)
            Debug.LogFormat("Parsed SoundData in {0} Seconds", Check.It());
        
        cam = Camera.main.transform;
    }


    public static float GetVolume(Vector3 pos)
    {
        return Inst != null? Inst.GetVol(pos) : 0;
    }
    
    
    private float GetVol(Vector3 pos)
    {
        if (frame != Time.frameCount)
        {
            frame = Time.frameCount;
            
            Vector3 camPos = cam.position;
            
            camPos = new Vector3(camPos.x * -1 - min.x, camPos.y - min.y, camPos.z - min.z);
        
            int x = Mathf.RoundToInt(camPos.x * stepMulti);
            int y = Mathf.RoundToInt(camPos.y * stepMultiY);
            int z = Mathf.RoundToInt(camPos.z * stepMulti);
        
            int samplePoint = y * yDimensionMulti + z * dimensions.x + x;
            
            camIndex = samplePoint * roomPts;
        }

        {
            pos = new Vector3(pos.x * -1 - min2.x, pos.y - min2.y, pos.z - min2.z);
        
            int x = Mathf.Clamp(Mathf.RoundToInt(pos.x * stepMulti2), 0, dimensions2.x - 2);
            int y = Mathf.Clamp(Mathf.RoundToInt(pos.y * stepMultiY2), 0, dimensions2.y - 2);
            int z = Mathf.Clamp(Mathf.RoundToInt(pos.z * stepMulti2), 0, dimensions2.z - 2);
        
            int samplePoint = y * yDimensionMulti2 + z * dimensions2.x + x;
        
            const float byteMulti = 1f / 255;
        
            return volume[camIndex + samplePoint] * byteMulti;
        }
    }


    #region Audio Toggle
    private void Update()
    {
        if (Input.GetKeyDown(Keys.ß))
            _audioEnabled = (_audioEnabled + 1) % 3;
    }
    
    
    public static int AudioEnabled;
    private static int _audioEnabled
    {
        get
        {
            AudioEnabled = PlayerPrefs.GetInt("SoundEnabled");
            return AudioEnabled;
        }
        set
        {
            AudioEnabled = value;
            PlayerPrefs.SetInt("SoundEnabled", value);
        }
    }


    private void OnEnable()
    {
        AudioEnabled = _audioEnabled;
    }
    #endregion
}
