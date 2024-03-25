using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public RoomManager roomManager;

    public void CreatingRoom()
    {
        Debug.Log("Button Presssed");
        roomManager.joiningRoom = false;
    }
}
