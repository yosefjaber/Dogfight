using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    #region Fields
    public static RoomManager instance;
    public RoomList roomList;

    [Header("Player")]
    public GameObject player;
    public Transform[] spawnPoints;
    public Transform[] spawnPlanePoints;

    [Header("UI")]
    public GameObject roomCam;
    public GameObject nameUI;
    public GameObject connectingUI;

    [Header("Player Stats")]
    private string nickname = "anonymous";
    public string roomNameToJoin = "test";
    public bool ClientRender = false;
    public bool joiningRoom = true;
    [HideInInspector] public int kills = 0;
    [HideInInspector] public int deaths = 0;
    [HideInInspector] public bool isAddedToTeam = false;

    [Header("Plane")] 
    public GameObject plane;

    [Header("Map Generation")]
    public GameObject test;
    public GameObject[] assets;
    public float detailScale = 20f;
    [Range(0f, 1f)] public float chanceOfSpawn = 0.5f;
    [Range(0f, 1f)] public float chanceofBombSpawn = 0.1f;
    public float offSet = 5f;
    public bool spawnAssets = true;
    #endregion

    private void Awake() => instance = this;

    public void ChangeNickname(string name) => nickname = name;

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");
        if(joiningRoom)
        {
            Debug.Log("Joining room");
            PhotonNetwork.JoinOrCreateRoom(roomNameToJoin, null, null);
        }
        else
        {
            Debug.Log("Creating room");
            CreateRoom();
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

    private void SetupPlayer(GameObject playerInstance)
    {
        playerInstance.GetComponent<PlayerSetup>().IsLocalPlayer();
        playerInstance.GetComponent<Health>().IsLocalPlayer = true;
        playerInstance.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        GameObject playerInstance = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        SetupPlayer(playerInstance);
    }

    public void SpawnPlayer(Vector3 spawnLocation)
    {
        GameObject playerInstance = PhotonNetwork.Instantiate(player.name, spawnLocation, Quaternion.identity);
        SetupPlayer(playerInstance);
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
        catch { /* Ignore */ }
    }

    private Vector3 CalculateAssetPosition(int x, int y, float randomOffset)
    {
        float xPos = -875 + (1 / detailScale) * 1750 * x;
        float zPos = 875 - (1 / detailScale) * 1750 * y;
        return new Vector3(
            xPos + Random.Range(-randomOffset, randomOffset),
            0,
            zPos + Random.Range(-randomOffset, randomOffset)
        );
    }

    private void SpawnAsset(GameObject prefab, Vector3 position, float rotation, ref int numSpawned, ref int caseBomb)
    {
        if(prefab == assets[14])
        {
            PhotonNetwork.Instantiate(assets[14].name, position, Quaternion.Euler(0, rotation, 0));
            numSpawned++;
            caseBomb++;
        }
        else
        {
            Instantiate(prefab, position, Quaternion.Euler(0, rotation, 0));
            numSpawned++;
        }
    }

    void GenerateAssets()
    {
        int numSpawned = 0;
        int caseBomb = 0;
        Random.InitState(PhotonNetwork.CurrentRoom.Name.GetHashCode());

        for(int y = 0; y < detailScale; y++)
        {
            for(int x = 0; x < detailScale; x++)
            {
                if(Random.Range(0f, 1f) < chanceOfSpawn && spawnAssets)
                {
                    float randomRotation = Random.Range(0f, 360f);
                    int assetIndex = Random.Range(0, assets.Length);
                    GameObject assetPrefab = assets[assetIndex];
                    Vector3 spawnPosition = CalculateAssetPosition(x, y, offSet);

                    if(assetIndex == 14 && Random.Range(0f, 1f) >= chanceofBombSpawn)
                    {
                        assetIndex = Random.Range(0, assets.Length - 1);
                        assetPrefab = assets[assetIndex];
                    }

                    SpawnAsset(assetPrefab, spawnPosition, randomRotation, ref numSpawned, ref caseBomb);
                }
            }
        }

        Debug.Log($"Spawned {numSpawned} assets");
        Debug.Log($"Spawned {caseBomb} case bombs");
        spawnAssets = false;
    }

    void CreateRoom()
    {
        int roomsWithSameName = roomList.checkRoomAgainst(roomNameToJoin);
        Debug.Log($"Rooms with same name: {roomsWithSameName}");
        string finalRoomName = roomsWithSameName == 0 ? roomNameToJoin : $"{roomNameToJoin} ({roomsWithSameName})";
        PhotonNetwork.JoinOrCreateRoom(finalRoomName, null, null);
    }
}
