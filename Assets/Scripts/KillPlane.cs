using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;


public class KillPlane : MonoBehaviour
{
    public Health health;
    public GameObject plane;
    public GameObject PlaneObject;
    public PlaneLogic planeLogic;
    public GameObject EngineLocation;
    public GameObject Flames;
    public GameObject PlaneExplosion;
    public Transform ExplosionCenter;
    private PhotonView photonView;
    private Rigidbody rb;
    public RiderInfo riderInfo;
    public GameObject MouseFlightRig;
    public GameObject MouseFlightHud;
    private GameObject pilot;
    private GameObject flames;
    private bool blownUp;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Velocity: " + rb.linearVelocity);
        if (Input.GetKeyDown(KeyCode.J) && !blownUp)
        {
            RequestBlowUp();
            blownUp = true;
        }
    }

    public void RequestBlowUp()
    {
        if (!photonView.IsMine){
            Debug.LogError("Phton view of plane is not mine");
            return;
        }
        //photonView.RPC("TurnOnPlaneLogic", RpcTarget.All);
        photonView.RPC("Rpc_RequestBlowUp", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void TurnOnPlaneLogic()
    {
        planeLogic.enabled = true;
        planeLogic.blownUp = true;
    }

    [PunRPC]
    private void Rpc_RequestBlowUp()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        // Spawn flames (networked)
        photonView.RPC("SpawnFlames", RpcTarget.MasterClient);

        Vector3 linVel = rb.linearVelocity;
        Vector3 angVel = rb.angularVelocity;

        // Disable flight
        if(riderInfo.Driver != null)
        {
            PhotonView pv = riderInfo.Driver.GetComponent<PhotonView>();
            Photon.Realtime.Player pilotPlayer = pv.Owner;
            photonView.RPC("StopPlane", pilotPlayer);   
        }

        // Tell everyone to start burning
        photonView.RPC("Rpc_StartBurning", RpcTarget.All, PhotonNetwork.Time);
    }

    [PunRPC]
    private void SpawnFlames()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject f = PhotonNetwork.InstantiateRoomObject(
                Flames.name,
                EngineLocation.transform.position,
                EngineLocation.transform.rotation
            );

            photonView.RPC(
                "Rpc_SetFlameParent",
                RpcTarget.All,
                f.GetComponent<PhotonView>().ViewID
            );
        }
        else
        {
            Debug.LogError("What?");
        }
    }

    [PunRPC]
    private void Rpc_SetFlameParent(int flameViewId)
    {
        PhotonView flameView = PhotonView.Find(flameViewId);
        if (flameView == null){
            Debug.LogError("Flames View is not found");
            return;
        }
        flames = flameView.gameObject;
        flames.transform.SetParent(EngineLocation.transform, true);
        flames.transform.position = EngineLocation.transform.position;
        flames.transform.rotation = EngineLocation.transform.rotation;
    }


    [PunRPC]
    private void Rpc_StartBurning(double startTime)
    {
        // Calculate remaining time using Photon time
        double elapsed = PhotonNetwork.Time - startTime;
        float remaining = Mathf.Max(0f, 5f - (float)elapsed);

        StartCoroutine(ExplodeAfterDelay(remaining));
    }

    [PunRPC]
    private void StopPlane()
    {
        planeLogic.DisableFlight();
    }

    private IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        PhotonNetwork.InstantiateRoomObject(
            PlaneExplosion.name,
            ExplosionCenter.position,
            ExplosionCenter.rotation
        );

        Destroy(flames);
        Destroy(PlaneObject);

        KillAllPassangers();
    }

    private void KillAllPassangers()
    {
        foreach (GameObject Rider in riderInfo.riders)
        {
            Rider.SetActive(true);
            Rider.GetComponent<Health>().Die();
        }
    }
}