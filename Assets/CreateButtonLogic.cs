using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButtonLogic : MonoBehaviour
{
    // Start is called before the first frame update
    public RoomManager roomManager;

    void OnButtonClick()
    {
        roomManager.joiningRoom = false;
    }
}
