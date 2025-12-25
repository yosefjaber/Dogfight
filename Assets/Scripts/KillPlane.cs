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
    private PhotonView photonView;
    private Rigidbody rb;
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
            blowUpPlane();
        }
    }

    public void blowUpPlane()
    {
        Debug.Log("Time to blow up");
        Vector3 linearVelocity = rb.linearVelocity;
        Vector3 angularVelocity = rb.angularVelocity;
        plane.GetComponent<MFlight.Demo.Plane>().SetEnabledState(false);
        planeLogic.enabled = false;
        plane.GetComponent<Rigidbody>().useGravity = true;
        GameObject flames = PhotonNetwork.Instantiate(Flames.name, EngineLocation.transform.position, EngineLocation.transform.rotation);
        flames.transform.SetParent(EngineLocation.transform);
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
        rb.linearVelocity = linearVelocity;
        rb.angularVelocity = angularVelocity;
        // Destroy(PlaneObject);
    }
}
