using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPropeller : MonoBehaviour
{
    public GameObject plane;
    public GameObject propeller;
    public float rotationSpeed = 1000f;

    // Update is called once per frame
    void Update()
    {
        if(plane.GetComponent<MFlight.Demo.Plane>().isEnabled)
        {
            propeller.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
