using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class KillPlane : MonoBehaviour
{
    public Health health;
    public GameObject plane;
    public PlaneLogic planeLogic;
    public GameObject EngineLocation;
    public GameObject Flames;
    private PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            blowUpPlane();
        }
    }

    public void blowUpPlane()
    {
        plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(false);
        planeLogic.enabled = false;
        plane.GetComponent<Rigidbody>().useGravity = true;
        GameObject flames = PhotonNetwork.Instantiate(Flames.name, EngineLocation.transform.position, EngineLocation.transform.rotation);
        flames.transform.SetParent(EngineLocation.transform);
    }
}
