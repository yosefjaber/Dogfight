using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Numerics;
using System;

public class BombLogic : MonoBehaviour
{
    public Transform bombShoot;
    public GameObject bomb;
    public GameObject plane;
    public GameObject leftGear;
    public GameObject rightGear;
    public GameObject planeBody;

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
        GameObject bombInstance = PhotonNetwork.Instantiate(this.bomb.name, bombShoot.position, UnityEngine.Quaternion.Euler(90, 0, 0), 0);
        
        Explode explodeComponent = bombInstance.GetComponent<Explode>();
        if (explodeComponent != null)
        {
            explodeComponent.plane = this.plane;
            explodeComponent.leftGear = this.leftGear;
            explodeComponent.rightGear = this.rightGear;
            explodeComponent.planeBody = this.planeBody;
        }
    }
}
