using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string RoomName;

    public void OnButtonPressed()
    {
        Debug.Log("hello");
        RoomList.Instance.JoinRoomByName(RoomName);
    }
}
