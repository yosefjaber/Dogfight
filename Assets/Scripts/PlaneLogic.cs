using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlaneLogic : MonoBehaviour
{
    //Plane Variables
    public GameObject planeCamera;
    public GameObject backCamera;
    public Transform playerPoint;
    private Rigidbody rb;
    private RoomManager roomManager;
    private BombLogic bombLogic;
    
    //Mouse Flight
    public GameObject MouseFlightRig;
    public GameObject MouseFlightHud;
    public GameObject plane;

    //Gun Variables
    public BulletsLeftText bulletsLeftText;
    public AirplaneGun leftAirplaneGun;
    public AirplaneGun rightAirplaneGun; 
    public TextMeshProUGUI planeAmmoText;
    private int maxAmmo;
    public float reloadTime = 2f;
    private float reloadTimeCounter = 0f;
    public PhotonView photonView;

    private void Start() 
    {
        maxAmmo = leftAirplaneGun.maxAmmo + rightAirplaneGun.maxAmmo;
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        bombLogic = GameObject.Find("BombShoot").GetComponent<BombLogic>();
        rb = GetComponent<Rigidbody>(); 
        planeAmmoText.text = (leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo).ToString() + "/" + maxAmmo.ToString();
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

        //Shoot
        if(Input.GetMouseButton(0) && (leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo) > 0)
        {
            //Timer in shoot so no need to worry about rate of fire
            leftAirplaneGun.Shoot();
            rightAirplaneGun.Shoot();
            planeAmmoText.text = (leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo).ToString() + "/" + maxAmmo.ToString();
        }
    }
    
    public void ReloadPlane()
    {
        if (Input.GetKey(KeyCode.E))
        {
            reloadTimeCounter += Time.deltaTime;
            if (reloadTimeCounter >= reloadTime)
            {
                photonView.RPC("Reload", RpcTarget.All);
                reloadTimeCounter = 0f;
                Debug.Log("Reloaded");
            }
            Debug.Log(reloadTimeCounter);
        }
    }
    
    [PunRPC]
    private void Reload()
    {
        leftAirplaneGun.Reload();
        rightAirplaneGun.Reload();
        planeAmmoText.text = (leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo).ToString() + "/" + maxAmmo.ToString();
        Debug.Log("Reload");
        //bulletsLeftText.UpdateBulletsLeft(leftAirplaneGun.currentAmmo + rightAirplaneGun.currentAmmo);
    }
    
    public void updateText(int ammo)
    {
        planeAmmoText.text = ammo.ToString() + "/" + maxAmmo.ToString();
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
        plane.GetComponent<Rigidbody>().linearDamping = 0.1f;
        plane.GetComponent<Rigidbody>().angularDamping = 0.1f;
    }
}
