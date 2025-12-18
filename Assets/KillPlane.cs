using UnityEngine;

public class KillPlane : MonoBehaviour
{
    public Health health;
    public GameObject Plane;
    public Plane planeScript;
    public PlaneLogic planeLogic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void blowUpPlane()
    {
        planeLogic.enabled = false;
        planeScript.isEnabled = false;
        Plane.GetComponent<Rigidbody>().useGravity = true;
    }
}
