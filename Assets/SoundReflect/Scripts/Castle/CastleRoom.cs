using UnityEngine;

public class CastleRoom : MonoBehaviour
{
    public int roomID;
    public int floor;

    [Space]
    public Collider floorCollider;
    public GameObject room;

    [Space] public Vector3 centerPoint;

    [HideInInspector] 
    public int[] doors;

    private string roomInfo;
    public string RoomInfo => roomInfo;
    private int doorCount;
    
    private bool visible;
    

    public CastleRoom Setup(int roomID, int floor)
    {
        this.roomID = roomID;
        this.floor = floor;
        doors = null;
        
        return this;
    }


    public void AddFloorCollider(Collider floorCollider)
    {
        this.floorCollider = floorCollider;
    }


    public void AddRoomGameObject(GameObject room)
    {
        this.room = room;
    }
    
    
    public void AddDoors(int[] doors)
    {
        this.doors = doors;
        
        doorCount = doors.Length;
        roomInfo = (floor == 0? "Ground" : floor.Ordinal()) + " Floor - Room " + roomID + " - Doors: " + doorCount;
    }


    public void AddItem(Transform item)
    {
        item.parent = room.transform;
    }


    private void Start()
    {
        visible = true;
        Show(false);
    }


    public void Show(bool show)
    {
        if(visible == show)
            return;

        visible = show;
        room.SetActive(visible);
    }
}
