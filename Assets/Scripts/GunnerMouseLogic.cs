using UnityEngine;

public class GunnerMouseLogic : MonoBehaviour
{
    public GameObject gun;
    public float sens = 1f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sens;
        float mouseY = Input.GetAxis("Mouse Y") * sens;

        Vector3 currentRotation = gun.transform.eulerAngles;

        currentRotation.y += mouseX;
        currentRotation.x -= mouseY;

        gun.transform.eulerAngles = currentRotation;
    }
}
