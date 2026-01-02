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
        if (Input.GetKeyDown(KeyCode.J))
        {
            RequestBlowUp();
        }
    }

    public void RequestBlowUp()
    {
        if (!photonView.IsMine){
            Debug.LogError("Phton view of plane is not mine");
            return;
        }
        photonView.RPC("Rpc_RequestBlowUp", RpcTarget.MasterClient);
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
        // Disable flight
        PhotonView pv = riderInfo.Driver.GetComponent<PhotonView>();
        Photon.Realtime.Player pilotPlayer = pv.Owner;
        photonView.RPC("StopPlane", pilotPlayer);

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
    }

    [PunRPC]
    private void Rpc_ExplodePlane()
    {
        // Kill driver if present
        if (riderInfo != null && riderInfo.IsDriver())
        {
            riderInfo.photonView.RPC("PopDriver", RpcTarget.All);
        }

        PhotonNetwork.InstantiateRoomObject(
            PlaneExplosion.name,
            ExplosionCenter.position,
            ExplosionCenter.rotation
        );

        if (flames != null)
            PhotonNetwork.Destroy(flames);

        // IMPORTANT: destroy the object with the PhotonView
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(photonView.gameObject);
    }






    // public void RequestBlowUp()
    // {
    //     photonView.RPC("BlowUpPlane", RpcTarget.All);
    //     photonView.RPC("BlowUpPlaneMaster", RpcTarget.MasterClient);
    // }

    // [PunRPC]
    // private void BlowUpPlane()
    // {
    //     Debug.Log("Time to blow up");
    // }

    // [PunRPC]
    // private void BlowUpPlaneMaster()
    // {
    //     if (!PhotonNetwork.IsMasterClient)
    //     {
    //         Debug.LogError("Blow Up Plane called on Non Master Client");
    //         return;
    //     }
    //     Vector3 linearVelocity = rb.linearVelocity;
    //     Vector3 angularVelocity = rb.angularVelocity;
    //     plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(false);
    //     planeLogic.enabled = false;
    //     rb.useGravity = true;
    //     rb.linearDamping = 0f;
    //     rb.angularDamping = 0f;
    //     rb.linearVelocity = linearVelocity;
    //     rb.angularVelocity = angularVelocity;

    //     flames = PhotonNetwork.InstantiateRoomObject(Flames.name, EngineLocation.transform.position, EngineLocation.transform.rotation);
    //     flames.transform.SetParent(EngineLocation.transform);

    //     ActionScheduler.Instance.InvokeAction(ExplodePlane, 5f);
    // }

    // // Set off explosion and kill all players onboard
    // private void ExplodePlane()
    // {
    //     if (riderInfo.IsDriver())
    //     {
    //         riderInfo.photonView.RPC("PopDriver", RpcTarget.All);
    //     }

    //     GameObject planeExplosion = PhotonNetwork.InstantiateRoomObject(PlaneExplosion.name, ExplosionCenter.position, ExplosionCenter.rotation);
    //     PhotonNetwork.Destroy(photonView.gameObject);
    //     PhotonNetwork.Destroy(PlaneObject);
    //     PhotonNetwork.Destroy(flames);

    //     ActionScheduler.Instance.InvokeAction(EnablePilot, 5f);
    // }

    // private void EnablePilot()
    // {
    //     NetworkDestroyer.Instance.RequestEnable(planeLogic.Pilot);
    //     if (planeLogic.Pilot.GetComponent<Health>().IsLocalPlayer)
    //     {
    //         RoomManager.instance.SpawnPlayer();
    //         RoomManager.instance.deaths++;
    //         RoomManager.instance.SetHashes();
    //         PhotonNetwork.LocalPlayer.AddScore(-100);
    //     }
    // }

    // // This function sounds so messed up im so sorry
    // private void KillRiders()
    // {
    //     foreach (GameObject rider in riderInfo.riders)
    //     {
    //         rider.GetComponent<Health>().health = 0;
    //     }
    // }
}