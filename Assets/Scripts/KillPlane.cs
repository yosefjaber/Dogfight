using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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
            photonView.RPC("RequestBlowUp", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RequestBlowUp()
    {
        BlowUpPlane();
    }

    public void BlowUpPlane()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Blow Up Plane called on Non Master Client");
            return;
        }

        Debug.Log("Time to blow up");
        Vector3 linearVelocity = rb.linearVelocity;
        Vector3 angularVelocity = rb.angularVelocity;
        plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(false);
        planeLogic.enabled = false;
        plane.GetComponent<Rigidbody>().useGravity = true;
        flames = PhotonNetwork.InstantiateRoomObject(Flames.name, EngineLocation.transform.position, EngineLocation.transform.rotation);
        flames.transform.SetParent(EngineLocation.transform);
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.linearVelocity = linearVelocity;
        rb.angularVelocity = angularVelocity;

        Invoke(nameof(ExplodePlane), 5f);
    }

    // Set off explosion and kill all players onboard
    private void ExplodePlane()
    {
        if (riderInfo.IsDriver())
        {
            riderInfo.photonView.RPC("PopDriver", RpcTarget.All);
        }

        GameObject planeExplosion = PhotonNetwork.InstantiateRoomObject(PlaneExplosion.name, ExplosionCenter.position, ExplosionCenter.rotation);
        PhotonNetwork.Destroy(PlaneObject);
        PhotonNetwork.Destroy(flames);

        ActionScheduler.Instance.InvokeAction(EnablePilot, 5f);
    }

    private void EnablePilot()
    {
        NetworkDestroyer.Instance.RequestEnable(planeLogic.Pilot);
        if (planeLogic.Pilot.GetComponent<Health>().IsLocalPlayer)
        {
            RoomManager.instance.SpawnPlayer();
            RoomManager.instance.deaths++;
            RoomManager.instance.SetHashes();
            PhotonNetwork.LocalPlayer.AddScore(-100);
        }
    }
}
