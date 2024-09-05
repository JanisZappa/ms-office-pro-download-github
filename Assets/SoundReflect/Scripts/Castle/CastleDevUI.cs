using TMPro;
using UnityEngine;

public class CastleDevUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    
    private CastleRoom currentRoom;

    private void Start()
    {
        Hide();
    }
    
    public void UpdateRoom(bool show, CastleRoom room)
    {
        if (room != currentRoom)
        {
            currentRoom = room;
            text.text = room != null ? room.RoomInfo : "";
        }
    }

    public void Hide()
    {
        text.text = "";
        currentRoom = null;
    }
}
