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
    public float PlaceX;
    public float PlaceZ;
    public float PlaneX;
    public float PlaneZ;

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

    [Header("Terrain Generation")]
    public TerrainGenerator terrainGenerator;

    private GameObject localPlayerInstance;
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
        
        Debug.Log("About to start terrain generation...");
        StartTerrainGeneration();

        SpawnPlane();
    }

    private void StartTerrainGeneration()
    {
        Debug.Log($"StartTerrainGeneration called - TerrainGen null? {terrainGenerator == null}, Player null? {localPlayerInstance == null}");
        
        if (terrainGenerator != null && localPlayerInstance != null)
        {
            Random.InitState(PhotonNetwork.CurrentRoom.Name.GetHashCode());
            Debug.Log($"Setting viewer to: {localPlayerInstance.name}");
            terrainGenerator.viewer = localPlayerInstance.transform;
            Debug.Log("Calling InitializeTerrain...");
            terrainGenerator.InitializeTerrain();
            Debug.Log("Terrain generation started with player as viewer");
        }
        else
        {
            Debug.LogWarning("Cannot start terrain generation - missing TerrainGenerator or Player");
        }
    }

    private void SetupPlayer(GameObject playerInstance)
    {
        playerInstance.GetComponent<PlayerSetup>().IsLocalPlayer();
        playerInstance.GetComponent<Health>().IsLocalPlayer = true;
        playerInstance.GetComponent<PhotonView>().RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        PhotonNetwork.LocalPlayer.NickName = nickname;

        localPlayerInstance = playerInstance;
    }

    public void SpawnPlayer()
    {
        PlaceX = Random.Range(300, 316);
        PlaceZ = Random.Range(-15, 15);
        Vector3 randomPlayerSpawnPoint = new Vector3(PlaceX, 90, PlaceZ);
        GameObject playerInstance = PhotonNetwork.Instantiate(player.name, randomPlayerSpawnPoint, Quaternion.identity);
        SetupPlayer(playerInstance);
    }

    public void SpawnPlayer(Vector3 spawnLocation)
    {
        GameObject playerInstance = PhotonNetwork.Instantiate(player.name, spawnLocation, Quaternion.identity);
        SetupPlayer(playerInstance);
    }

    public void SpawnPlane()
    {
        PlaneX = Random.Range(284, 300);
        PlaneZ = Random.Range(-15, 15);
        Vector3 randomPlaneSpawnPoint = new Vector3(PlaneX, 90, PlaneZ);
        PhotonNetwork.Instantiate(plane.name, randomPlaneSpawnPoint, Quaternion.identity);
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

    void CreateRoom()
    {
        int roomsWithSameName = roomList.checkRoomAgainst(roomNameToJoin);
        Debug.Log($"Rooms with same name: {roomsWithSameName}");
        string finalRoomName = roomsWithSameName == 0 ? roomNameToJoin : $"{roomNameToJoin} ({roomsWithSameName})";
        PhotonNetwork.JoinOrCreateRoom(finalRoomName, null, null);
    }
}