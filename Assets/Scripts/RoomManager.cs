using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    public RoomList roomList;
    //private static bool spawnedAssets = false;

    public GameObject player;
    [Space]
    public Transform[] spawnPoints;
    public Transform[] spawnPlanePoints;

    [Space]
    public GameObject roomCam;

    [Space]
    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "anonymous";
    public string roomNameToJoin = "test";
    public bool ClientRender = false;


    [HideInInspector]
    public int kills = 0;
    [HideInInspector]
    public int deaths = 0;

    [Header("Plane")]
    public GameObject plane;




    [Header("Map Generator")]
    public GameObject test;
    public GameObject[] assets;

    [Header("Random Set Assets")]
    public float detailScale = 20f;
    [Range(0f, 1f)]
    public float chanceOfSpawn = 0.5f;
    [Range(0f, 1f)]
    public float chanceofBombSpawn = 0.1f;
    public float offSet = 5f;
    public bool spawnAssets = true;


    void Awake()
    {
        instance = this;
    }

    public void ChangeNickname(string name)
    {
        nickname = name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");
        int roomsWithSameName = roomList.checkRoomAgainst(roomNameToJoin);
        Debug.Log("Rooms with same name: " + roomsWithSameName);
        if(roomsWithSameName == 0)
        {
            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        }
        else
        {
            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin + " (" + roomsWithSameName + ")", null, null);
        }

        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We're connected and in a room");

        roomCam.SetActive(false);

        SpawnPlayer();
        SpawnPlane();
        GenerateAssets();
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        GameObject player = PhotonNetwork.Instantiate(this.player.name, spawnPoint.position, Quaternion.identity);
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<Health>().IsLocalPlayer = true;
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void SpawnPlayer(Vector3 spawnLocation)
    {
        GameObject player = PhotonNetwork.Instantiate(this.player.name, spawnLocation, Quaternion.identity);
        player.GetComponent<PlayerSetup>().IsLocalPlayer();
        player.GetComponent<Health>().IsLocalPlayer = true;
        player.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void SpawnPlane()
    {
        Debug.Log("Spawning plane");
        Transform spawnPlanePoint = spawnPlanePoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        PhotonNetwork.Instantiate(plane.name, spawnPlanePoint.position, Quaternion.identity);
    }

    public void SetHashes()
    {
        try
        {
            Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

            hash["kills"] = kills;
            hash["deaths"] = deaths;

            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
        catch
        {
            //Do nothing
        }
    }

    void GenerateAssets()
    {
        //The true way to do this Gojo would be proud
        int numSpawned = 0;
        int caseBomb = 0;
        int seed = PhotonNetwork.CurrentRoom.Name.GetHashCode();
        int assetIndex = 0;

        Random.InitState(seed); // Initialize the random number generator with the seed

        for(int y = 0; y < detailScale; y++)
        {
            for(int x = 0; x < detailScale; x++)
            {
                if(Random.Range(0f, 1f) < chanceOfSpawn && spawnAssets)
                {
                    float xPos = -875 + (1 / detailScale) * 1750 * x;
                    float zPos = 875 - (1 / detailScale) * 1750 * y;
                    float randomRotation = Random.Range(0f, 360f);
                        // Spawn assets using the client's PhotonNetwork.Instantiate method
                        //PhotonNetwork.Instantiate(assets[Random.Range(3, assets.Length)].name, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                        assetIndex = Random.Range(0, assets.Length);
                        GameObject assetPrefab = assets[assetIndex]; // Assuming assets[] is an array of GameObjects (prefabs)

                        if(assetIndex == 14)
                        {
                            if(Random.Range(0f, 1f) < chanceofBombSpawn)
                            {
                                // Spawn a case bomb
                                Instantiate(assetPrefab, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                                numSpawned++;
                                caseBomb++;
                            }
                            else 
                            {
                                assetIndex = Random.Range(0, assets.Length - 1);
                                assetPrefab = assets[assetIndex];
                                Instantiate(assetPrefab, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                                numSpawned++;
                            }
                        }
                        else 
                        {
                            Instantiate(assetPrefab, new Vector3(xPos + Random.Range(-offSet, offSet), 0, zPos + Random.Range(-offSet, offSet)), Quaternion.Euler(0, randomRotation, 0));
                            numSpawned++;
                        }
                    }
                }
            }

            Debug.Log("Spawned " + numSpawned + " assets");
            Debug.Log("Spawned " + caseBomb + " case bombs");
            spawnAssets = false;
    }
}
