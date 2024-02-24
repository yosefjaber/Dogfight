using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaneLogic : MonoBehaviour
{
    public GameObject planeCamera;
    public GameObject backCamera;
    public Transform playerPoint;
    private Rigidbody rb;
    private RoomManager roomManager;
    private BombLogic bombLogic;
    public AirplaneGun leftAirplaneGun;
    public AirplaneGun rightAirplaneGun; 
    

    public GameObject MouseFlightRig;
    public GameObject MouseFlightHud;
    public GameObject plane;
    // public Animation animation;
    // public AnimationClip propeller;

    private void Start() 
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        bombLogic = GameObject.Find("BombShoot").GetComponent<BombLogic>();
        rb = GetComponent<Rigidbody>(); 
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ExitPilot();
        }

        //Just a test to see if bomb drops
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Drop Bomb");
            bombLogic.dropBomb();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(backCamera.activeSelf)
            {
                backCamera.SetActive(false);
                planeCamera.SetActive(true);
            }
            else
            {
                backCamera.SetActive(true);
                planeCamera.SetActive(false);
            }
        }

        if(Input.GetMouseButton(0))
        {
            leftAirplaneGun.Shoot();
            rightAirplaneGun.Shoot();
        }
    }

    private void FixedUpdate() 
    {

    }

    public void ExitPilot()
    {
        planeCamera.SetActive(false);
        roomManager.SpawnPlayer(playerPoint.position);
        this.enabled = false;
        MouseFlightHud.SetActive(false);
        MouseFlightRig.SetActive(false);
        plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(false);
        plane.GetComponent<Rigidbody>().useGravity = true;
        plane.GetComponent<Rigidbody>().drag = 0.1f;
        plane.GetComponent<Rigidbody>().angularDrag = 0.1f;
    }
}
