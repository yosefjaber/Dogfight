using UnityEngine;
using Photon.Pun;

public class GunnerMouseLogic : MonoBehaviour
{
    public GameObject gun;
    public float sens = 1f;
    private RoomManager roomManager;
    public Transform playerPoint;
    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;

        Vector3 currentRotation = gun.transform.eulerAngles;

        currentRotation.y += mouseX;
        currentRotation.x -= mouseY;

        gun.transform.eulerAngles = currentRotation;
        if(Input.GetKeyDown(KeyCode.E))
        {
            ExitGunner();
        }
    }
    void ExitGunner()
    {
        roomManager.SpawnPlayer(playerPoint.position);
        this.gameObject.SetActive(false);
    }
}
