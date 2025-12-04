using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject Player;
    public float PlaceX;
    public float PlaceZ;

    public GameObject Plane;
    public float PlaneX;
    public float PlaneZ;

    void Start()
    {
        PlaceX = Random.Range(300, 316);
        PlaceZ = Random.Range(-15, 15);
        Player.transform.position = new Vector3(PlaceX, 90, PlaceZ);

        PlaneX = Random.Range(284, 300);
        PlaneZ = Random.Range(-15, 15);
        Plane.transform.position = new Vector3(PlaneX, 90, PlaneZ);
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    public GameObject Player;
    public float Blue_PlaceX;
    public float Blue_PlaceZ;

    public float Red_PlaceX;
    public float Red_PlaceZ;

    void Start()
    {
        if (Player.name == "Blue_Player")
        {
            Blue_PlaceX = Random.Range(300, 316);
            Blue_PlaceZ = Random.Range(-15, 15);
            Player.transform.position = new Vector3(Blue_PlaceX, 90, Blue_PlaceZ);
        } else if (Player.name == "Red_Player")
        {
            Red_PlaceX = Random.Range(-300, -284);
            Red_PlaceZ = Random.Range(-15, 15);
            Player.transform.position = new Vector3(Red_PlaceX, , Red_PlaceZ);
        }
        
    }
}
*/