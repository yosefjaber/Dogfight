using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PhotonNetwork = Photon.Pun.PhotonNetwork;

public class EnterPlaneLogic : MonoBehaviour
{
    [Header("References")]
    private RoomManager roomManager;
    public GameObject planeRoom;
    public GameObject plane;
    public GameObject planeCamera;
    public GameObject MosuseFlightRig;
    public GameObject MouseFlightHud;
    public GameObject backCamera;
    public PlaneLogic planeLogic;
    public AirplaneGun leftAirplaneGun;
    public AirplaneGun rightAirplaneGun;

    //private bool onPlane = false;
    [Space]
    [Header("Time Thresholds")]
    private float timeSinceEntered = 0f;
    private float timeThreshold = 0.1f;

    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterPlane(GameObject player)
    {
        PhotonNetwork.Destroy(player);
        MouseFlightHud.SetActive(true);
        MosuseFlightRig.SetActive(true);
        planeCamera.SetActive(true);
        backCamera.SetActive(false);
        plane.GetComponent<PlaneLogic>().enabled = true;
        plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(true);
        plane.GetComponent<Rigidbody>().useGravity = false;
        plane.GetComponent<Rigidbody>().linearDamping = 2f;
        plane.GetComponent<Rigidbody>().angularDamping = 5f;
        planeLogic.updateText(leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo);

        PhotonView planePhotonView = plane.GetComponent<PhotonView>();
        if (planePhotonView != null)
        {
            // Make sure we don't already own the plane
            if (!planePhotonView.IsMine)
            {
                // Request ownership of the PhotonView
                planePhotonView.RequestOwnership();
            }
        }
        else
        {
            Debug.LogError("No PhotonView found on the plane GameObject!");
        }
    }
}
