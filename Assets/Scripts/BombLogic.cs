using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombLogic : MonoBehaviour
{
    public Transform bombShoot;
    public GameObject bomb;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void dropBomb()
    {
        // PhotonNetwork.Instantiate(plane.name, spawnPlanePoint.position, Quaternion.identity);
        PhotonNetwork.Instantiate(bomb.name, bombShoot.position, Quaternion.identity);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Bomb hit something");
        //Destroy(this.gameObject);
    }
}
