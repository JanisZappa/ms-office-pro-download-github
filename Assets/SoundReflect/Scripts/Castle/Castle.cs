using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Castle : Singleton<Castle>
{
    private Camera cam;

    public static Dictionary<Collider, CastleRoom> roomMap;
    
    
    public void Start()
    {
        ParseData();
        
        roomMap = new Dictionary<Collider, CastleRoom>();
        int roomCount = rooms.Length;
        for (int i = 0; i < roomCount; i++)
        {
            CastleRoom room = rooms[i];
            roomMap.Add(room.floorCollider, room);
        }

        int doorCount = doors.Length;
        for (int i = 0; i < doorCount; i++)
            doors[i].CalculateCorners();
    }


//  Generation  //
    [Header("Generation")]
    public GameObject modelFile;

    public TextAsset doorData, roomDoorData, itemData, roomItemData;
    
    [Space]
    public Material[] mats;

    
    

    [HideInInspector] 
    public CastleRoom[] rooms;

    [HideInInspector] 
    public CastleDoor[] doors;
    
    
    public struct ScatterObject
    {
        public Vector3 pos;
        public Quaternion rot;
        public int type;

        public ScatterObject(Vector3 pos, Quaternion rot, int type)
        {
            this.pos = pos;
            this.rot = rot;
            this.type = type;
        }
    }


    public static CastleDoor GetDoor(int id)
    {
        return Inst.doors[id];
    }
    
    public static CastleRoom GetRoom(int id)
    {
        return Inst.rooms[id];
    }
    
    
    
    public void Generate()
    {
        Transform trans = transform;

        while (trans.childCount > 0)
            DestroyImmediate(trans.GetChild(0).gameObject);
        
        Transform modelTrans = modelFile.transform;
        int count = modelTrans.childCount;
        
        List<Transform> floors = new List<Transform>();
        Dictionary<int, Transform> roomGroups = new Dictionary<int, Transform>();
        List<CastleRoom> roomCollect = new List<CastleRoom>();

        for (int i = 0; i < count; i++)
        {
            GameObject original = modelTrans.GetChild(i).gameObject;

            if (original.name == "shell")
            {
                GameObject shell = Instantiate(original, trans);
                shell.name = "Shell";
                shell.GetComponent<MeshRenderer>().material = mats[0];
                shell.GetComponent<MeshRenderer>().enabled = false;
                continue;
            }

            string[] nameParts = original.name.Split('_');

            int floor = int.Parse(nameParts[0]);
            int roomID  = int.Parse(nameParts[1]);

            while (floors.Count < floor + 1)
            {
                GameObject newFloor = new GameObject("Floor " + floors.Count.ToString("D4"));
                newFloor.transform.parent = trans;
                floors.Add(newFloor.transform);
            }

            if (!roomGroups.TryGetValue(roomID, out Transform roomGroup))
            {
                GameObject newRoom = new GameObject("Room " + roomID.ToString("D4"));
                roomGroup = newRoom.transform;
                roomGroup.parent = floors[floor];
                roomCollect.Add(newRoom.AddComponent<CastleRoom>().Setup(roomID, floor));
                roomGroups.Add(roomID, roomGroup);
            }

            string partName = nameParts[2];
            GameObject part = Instantiate(original, roomGroup);
            part.name = partName;
            part.GetComponent<MeshRenderer>().sharedMaterial = GetMat(partName);
            part.layer = GetLayer(partName);

            Collider partCollider = SetupCollider(part, partName);
            if(partName == "floor")
                roomGroup.GetComponent<CastleRoom>().AddFloorCollider(partCollider);
            if(partName == "room")
                roomGroup.GetComponent<CastleRoom>().AddRoomGameObject(part);
        }

        rooms = roomCollect.ToArray();
        
        
    }


    private void ParseData()
    {
        float t = Time.realtimeSinceStartup;
        
        List<CastleDoor> doorCollect = new List<CastleDoor>();
        using (MemoryStream m = new MemoryStream(doorData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int roomA = r.ReadInt32();
                int roomB = r.ReadInt32();
                
                Vector3 pos = r.HoudiniVector3(true);
                Quaternion rot = Hou.Rot(r.ReadInt32());
                doorCollect.Add(new CastleDoor(roomA, roomB, pos, rot));
            }
        }

        doors = doorCollect.ToArray();
        
        using (MemoryStream m = new MemoryStream(roomDoorData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int doorCount = r.ReadInt32();
                int[] addDoors = new int[doorCount];
                for (int e = 0; e < doorCount; e++)
                    addDoors[e] = r.ReadInt32();
                
                rooms[i].AddDoors(addDoors);
            }
        }
        
        List<ScatterObject> objects = new List<ScatterObject>();
        using (MemoryStream m = new MemoryStream(itemData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int itemTypeCount = r.ReadInt32();
            int[] types = new int[itemTypeCount];
            int[] typeCounts = new int[itemTypeCount];
            for (int i = 0; i < itemTypeCount; i++)
                types[i] = r.ReadInt32();
            for (int i = 0; i < itemTypeCount; i++)
                typeCounts[i] = r.ReadInt32();
            
            
            for (int i = 0; i < itemTypeCount; i++)
            {
                int itemType = types[i];
                int count    = typeCounts[i];
                for (int e = 0; e < count; e++)
                {
                    Vector3 pos = r.HoudiniVector3(true);
                    Quaternion rot = Hou.Rot(r.ReadInt32());
                    objects.Add(new ScatterObject(pos, rot, itemType));
                }
            }
        }

        RoomObjectMesher mesher = GetComponent<RoomObjectMesher>();
        ScatterObject[] all = objects.ToArray();
        List<ScatterObject>roomObjects = new List<ScatterObject>();
        using (MemoryStream m = new MemoryStream(roomItemData.bytes))
        using (BinaryReader r = new BinaryReader(m))
        {
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                CastleRoom room = rooms[r.ReadInt32()];

                room.centerPoint = r.HoudiniVector3(true);
                
                int itemCount = r.ReadInt32();
                roomObjects.Clear();
                for (int e = 0; e < itemCount; e++)
                    roomObjects.Add(all[r.ReadInt32()]);
                
                room.AddItem(mesher.GetObjectRenderer(roomObjects));
            }
        }
        
        Debug.LogFormat("Parsed Data in {0:F4} Seconds", Time.realtimeSinceStartup - t);
    }

    private Material GetMat(string partName)
    {
        switch (partName)
        {
            default:
                return mats[0];
            
            case "glass":
                return mats[1];
        }
    }
    
    private static int GetLayer(string partName)
    {
        switch (partName)
        {
            default:         return CastleLayers.Default;
            case "walls":    return CastleLayers.Walls;
            case "floor":    return CastleLayers.Floor;
        }
    }


    private static Collider SetupCollider(GameObject part, string partName)
    {
        switch (partName)
        {
            default:
                return null;

            case "walls":
            case "floor":
            {
                MeshFilter mf = part.GetComponent<MeshFilter>();
                Mesh m = mf.sharedMesh;
                MeshCollider coll = part.AddComponent<MeshCollider>();
                coll.sharedMesh = m;
                DestroyImmediate(mf);
                DestroyImmediate(part.GetComponent<MeshRenderer>());
                return coll;
            }
        }
    }
}
