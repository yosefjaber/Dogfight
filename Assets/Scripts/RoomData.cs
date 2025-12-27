using UnityEngine;

public class RoomData : MonoBehaviour
{
    //Room
    public string roomName;
    public string roomCode;
    public int roomSize;
    public string roomInfo;

    //Map
    public int mapSize;
    public bool fallOffMap;

    //Teams
    public bool teams;
    public int numTeams;
    public int numPlanes;

    //Terrain
    public string terrainSeed;
    public string objectSeed;
    public bool[] environmentObjects;
    public int numAbilities;

    //Planes & Players
    public int planeHealth;
    public int planeSpeed;
    public int planeAmmo;
    public int planeDamage;
    public int planeShootSpeed;

    public bool passanger;
    public int passangerDamage;
    public int passangerShootSpeed;

    public int gunDamage;
    public int gunSpeed;
    public int meleeDamage;

}
