using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class CastleManager : MonoBehaviour
{
    public CastleDevUI devUI;
    private static Camera cam;
    private Transform camTrans;

    [Space] 
    public CastleRoom currentRoom;
    

    private bool debugUI, debugDraw;

    private Castle castle;
    private ScatterMaster scatter;
    
    
    private void Start()
    {
        cam = Camera.main;
        camTrans = cam.transform;
        castle   = FindObjectOfType<Castle>();
        scatter  = FindObjectOfType<ScatterMaster>();

        for (int i = 0; i < 1000; i++)
            roomCheck.Add(i);
    }

    
    private void LateUpdate()
    {
    //  Input  //
        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugUI = !debugUI;
            if(!debugUI)
                devUI.Hide();
        }
                
        if (Input.GetKeyDown(KeyCode.F2))
            debugDraw = !debugDraw;
    
    
    //  Get Current Room  //
        Vector3 camPos = camTrans.position;
        
        if (Physics.SphereCast(new Ray(camPos, Vector3.down), .025f, out RaycastHit hit, 1000,
            CastleLayers.Mask_Floor))
        {
            if (!Castle.roomMap.TryGetValue(hit.collider, out currentRoom))
                Debug.Log("Can't find " + hit.collider.name);
        }
        else
        {
            currentRoom = null;
            //Debug.Log("!!!No Floor Hit!!!");
        }
            
        Shader.SetGlobalVector(CamForward, camTrans.forward);
        
     
    //  Visibility Check  //    
        Profiler.BeginSample("Visibility Check");
        DoorVisCheck();
        RoomVisibility();
        Profiler.EndSample();

        scatter.ShowObjectsForRooms(visibleRooms);
        
    //  UI  //
        if(debugUI)
            devUI.UpdateRoom(debugDraw, currentRoom);
    }
    
    
//  Visibility Check  //
    private readonly List<Vector2Int> doorsOnScreen = new List<Vector2Int>(100);
    
    private readonly int[] visibleDoors = new int[100];
    private int visDoorCount;
    
    private readonly HashSet<int> lookedThrough = new HashSet<int>();
    private readonly Vector3[] storedTris = new Vector3[12 * 100];
    private readonly Dictionary<int, Vector2Int> triDoorMap = new Dictionary<int, Vector2Int>(100);
    private int triDoorCount;
    
    
    private readonly HashSet<int> roomCheck = new HashSet<int>();
    private readonly int[] checkRooms = new int[1000];
    private int visibleRooms;
    private static readonly int CamForward = Shader.PropertyToID("CamForward");


    private Vector2Int GetTrianglesInfo(int doorID)
    {
        if (triDoorMap.TryGetValue(doorID, out Vector2Int triInfo))
            return triInfo;
       
        CastleDoor door = Castle.GetDoor(doorID);
        int triCount = door.GetViewTriangles();

        if (triCount > 0)
        {
            for (int e = 0; e < triCount; e++)
            {
                storedTris[triDoorCount * 12 + e * 3] = TriClip.triList[e * 3];
                storedTris[triDoorCount * 12 + e * 3 + 1] = TriClip.triList[e * 3 + 1];
                storedTris[triDoorCount * 12 + e * 3 + 2] = TriClip.triList[e * 3 + 2];
            }

            triInfo = new Vector2Int(triDoorCount, triCount);
            triDoorCount++;
        }
        else
            triInfo = new Vector2Int(0, 0);
        
        triDoorMap.Add(doorID, triInfo);
        return triInfo;
    }
    

    private void DoorVisCheck()
    {
        if(currentRoom == null)
            return;
        
        TriClip.StoreCameraParams(cam);
        
        doorsOnScreen.Clear();
        
        lookedThrough.Clear();
        triDoorMap.Clear();
        triDoorCount = 0;
        visDoorCount = 0;

        for (int i = 0; i < currentRoom.doors.Length; i++)
        {
            int doorID = currentRoom.doors[i];
            CastleDoor door = Castle.GetDoor(doorID);
            if(!door.FacingAway(currentRoom.roomID))
                continue;
            
            Vector2Int triInfo = GetTrianglesInfo(doorID);
            
            if (triInfo.y > 0)
            {
                doorsOnScreen.Add(new Vector2Int(doorID, door.roomA == currentRoom.roomID? door.roomB : door.roomA));
             
                visibleDoors[visDoorCount++] = doorID;
            }
        }


        while (doorsOnScreen.Count > 0)
        {
        //  I'll be looking through this door now  //
            Vector2Int checkDoor = doorsOnScreen[doorsOnScreen.Count - 1];
            doorsOnScreen.RemoveAt(doorsOnScreen.Count - 1);
            
            int doorID = checkDoor.x;
            Vector2Int triInfo = GetTrianglesInfo(doorID);
            lookedThrough.Add(doorID);

            int nextRoomID = checkDoor.y;
            
            CastleRoom nextRoom = Castle.GetRoom(nextRoomID);

            for (int i = 0; i < nextRoom.doors.Length; i++)
            {
                int nextRoomDoorID = nextRoom.doors[i];
                
                if(lookedThrough.Contains(nextRoomDoorID))
                    continue;
                
                CastleDoor door = Castle.GetDoor(nextRoomDoorID);
                if(!door.FacingAway(nextRoomID))
                    continue;
                
                Vector2Int nextTriInfo = GetTrianglesInfo(nextRoomDoorID);
            //  If On Screen  //
                if (triInfo.y > 0)
                {
                //  See if triangles of Doors Overlap
                    
                    for (int e = 0; e < triInfo.y; e++)
                    {
                        Vector3 a = storedTris[triInfo.x * 12 + e * 3];
                        Vector3 b = storedTris[triInfo.x * 12 + e * 3 + 1];
                        Vector3 c = storedTris[triInfo.x * 12 + e * 3 + 2];

                        for (int f = 0; f < nextTriInfo.y; f++)
                        {
                            Vector3 a2 = storedTris[nextTriInfo.x * 12 + f * 3];
                            Vector3 b2 = storedTris[nextTriInfo.x * 12 + f * 3 + 1];
                            Vector3 c2 = storedTris[nextTriInfo.x * 12 + f * 3 + 2];

                            if (TriClip.TriTriOverlap(a, b, c, a2, b2, c2))
                                goto overlap;
                        }
                    }
                    
                    continue;
                    
                    overlap:
                    
                    
                    doorsOnScreen.Add(new Vector2Int(nextRoomDoorID, door.roomA == nextRoomID? door.roomB : door.roomA));
                    visibleDoors[visDoorCount++] = nextRoomDoorID;
                }
            }
        }

        if (debugDraw)
        {
            float drawDist = cam.nearClipPlane + .01f;
            for (int i = 0; i < visDoorCount; i++)
            {
                int doorID = visibleDoors[i];
                Vector2Int triInfo = GetTrianglesInfo(doorID);
            
                Color color = Color.red;
                Color.RGBToHSV(color, out float H, out float S, out float V);
                H = H + doorID * .1f;

                for (int e = 0; e < triInfo.y; e++)
                {
                    Vector3 a = cam.ViewportToWorldPoint(storedTris[triInfo.x * 12 + e * 3].SetZ(drawDist));
                    Vector3 b = cam.ViewportToWorldPoint(storedTris[triInfo.x * 12 + e * 3 + 1].SetZ(drawDist));
                    Vector3 c = cam.ViewportToWorldPoint(storedTris[triInfo.x * 12 + e * 3 + 2].SetZ(drawDist));

                    DRAW.Triangle(a, b, c).SetColor(Color.HSVToRGB((H + e * .2f) % 1, S, V).A(.2f));
                }
            }

            {
                Color color = Color.red;
                Color.RGBToHSV(color, out float H, out float S, out float V);
                H = H + currentRoom.roomID * .1f;
                DRAW.Vector(currentRoom.centerPoint, Vector3.up * 2).SetColor(Color.HSVToRGB(H % 1, S, V).A(.2f));
            }
        }
    }
    

    private void AddRoomIfUnique(int roomID)
    {
        if(roomCheck.Add(roomID))
            checkRooms[visibleRooms++] = roomID;
    }
    
    
    private void RoomVisibility()
    {
        TriClip.StoreCameraParams(cam);
        
        for (int i = 0; i < visibleRooms; i++)
            castle.rooms[checkRooms[i]].Show(false);
        
        visibleRooms = 0;
        roomCheck.Clear();
        
        if(currentRoom != null)
            AddRoomIfUnique(currentRoom.roomID);

        for (int i = 0; i < visDoorCount; i++)
        {
            CastleDoor door = Castle.GetDoor(visibleDoors[i]);
            AddRoomIfUnique(door.roomA);
            AddRoomIfUnique(door.roomB);
        }
        
        for (int i = 0; i < visibleRooms; i++)
            castle.rooms[checkRooms[i]].Show(true);
    }


    public static Vector3 RandomRoomCenter =>
        Castle.Inst.rooms[Random.Range(0, Castle.Inst.rooms.Length - 1)].centerPoint;
}
