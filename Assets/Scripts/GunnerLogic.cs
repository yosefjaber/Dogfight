using UnityEngine;

public class GunnerLogic : MonoBehaviour
{
    public GameObject rotatePoint;
    public GameObject gunCamera;
    public GameObject bulletSpawn;
    public float sens = 1f;
    private RoomManager roomManager;
    public Transform playerPoint;

    float xRoation = 0f; 
    float yRotation = 0f;
    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;

        yRotation += mouseX;
        xRoation -= mouseY;

        // Clamp pitch to avoid flipping the camera
        xRoation = Mathf.Clamp(xRoation, -89f, 89f);

        rotatePoint.transform.rotation = Quaternion.Euler(xRoation, yRotation, 0f);
        if(Input.GetKeyDown(KeyCode.E))
        {
            ExitGunner();
        }
    }
    void ExitGunner()
    {
        roomManager.SpawnPlayer(playerPoint.position);
        this.gameObject.GetComponent<GunnerLogic>().enabled = false;
        gunCamera.SetActive(false);
        bulletSpawn.SetActive(false);
    }
}
