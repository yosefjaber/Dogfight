using UnityEngine;

public class StayUpright : MonoBehaviour
{
    public Vector3 UprightDirection = Vector3.up; // Ensure this is a proper direction vector
    public GameObject plane;

    void Update()
    {
        // Calculate the rotation needed to align the object's "up" with the UprightDirection
        Quaternion targetRotation = Quaternion.FromToRotation(plane.transform.up, UprightDirection) * transform.rotation;

        // Smoothly interpolate to the target rotation (optional for smooth movement)
        transform.rotation = Quaternion.Slerp(plane.transform.rotation, targetRotation, Time.deltaTime * 5.0f);
    }
}
