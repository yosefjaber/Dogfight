using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneLogic : MonoBehaviour
{
    public GameObject planeCamera;
    public Transform playerPoint;
    private Rigidbody rb;
    private RoomManager roomManager;

    public GameObject MouseFlightRig;
    public GameObject MouseFlightHud;
    public GameObject plane;
    // public Animation animation;
    // public AnimationClip propeller;

    private void Start() 
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ExitPilot();
        }

        // if(plane.GetComponent<MFlight.Demo.Plane>().isEnabled)
        // {
        //     Debug.Log("Plane is enabled");
        //     if(!animation.isPlaying)
        //     {
        //         animation.Play(propeller.name);
        //     }
        // }
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
