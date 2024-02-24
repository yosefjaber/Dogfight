using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

public class RoomList : MonoBehaviourPunCallbacks
{
    public static RoomList Instance;
    public GameObject roomManagerGameObject;
    public RoomManager roomManager;

    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    private List<RoomInfo> cachedRoomList = new List<RoomInfo>();

    public void ChangeRoomtoCreateName(string name)
    {
        roomManager.roomNameToJoin = name;
    }

    private void Awake() {
        Instance = this;
    }
    IEnumerator Start()
    {
        //Precautions
        if(PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if(cachedRoomList.Count <= 0)
        {
            //first time being called sends the whole roomlist
            cachedRoomList = roomList;
        }
        else
        {
            //just sending the updates
            foreach(var room in roomList)
            {
                for(int i = 0; i < cachedRoomList.Count; i++)
                {
                    if(cachedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cachedRoomList;
                        //room might be deleted or added or players might have joined or left

                        if(room.RemovedFromList)
                        {
                            //room was deleted
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            //room was added or updated
                            newList[i] = room;
                        }


                        cachedRoomList = newList;

                    }
                }
            }
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        foreach(Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach(var room in cachedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + " / " + 16;

            roomItem.GetComponent<RoomItemButton>().RoomName = room.Name;
        }
    }

    public void JoinRoomByName(string name)
    {
        roomManager.roomNameToJoin = name;
        roomManagerGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    //Checks if other rooms have the same name and return the number of rooms with the same name and if there are no rooms with the same name, return 0
    public int checkRoomAgainst(String room)
    {
        int count = 0;
        foreach(var roomInfo in cachedRoomList)
        {
            if(roomInfo.Name == room)
            {
                count++;
            }
        }
        return count;
    }
}
