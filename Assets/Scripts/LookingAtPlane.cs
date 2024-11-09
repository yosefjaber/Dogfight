using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LookingAtPlane : MonoBehaviour
{
    public float lookDistance = 8f;
    public Camera playerCamera;
    public Material highlightColor;
    public Material normalColor;
    public LayerMask Enterbutton;
    public LayerMask Exitbutton;
    public LayerMask ReloadButton;
    private GameObject hitObject = null;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            CheckForPlane();
        }
        CheckForButton();
    }

    void CheckForPlane()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        RaycastHit hit;

        int PlayerMask = 1 << LayerMask.NameToLayer("LocalPlayer");
        //Invert the bitmask to ignore the specified layer
        PlayerMask = ~PlayerMask;

        if (Physics.Raycast(ray, out hit, lookDistance, PlayerMask))
        {
            if (hit.transform.gameObject.GetComponent<EnterPlaneRoom>())
            {
                this.transform.position = hit.transform.gameObject.GetComponent<EnterPlaneRoom>().PlaneRoom.transform.position;
            }
        }
    }

    void CheckForButton()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        RaycastHit hit;

        //ENTER BUTTON
        if (Physics.Raycast(ray, out hit, lookDistance, Enterbutton))
        {
            hitObject = hit.transform.gameObject;
            hit.transform.gameObject.GetComponent<Renderer>().material = highlightColor;
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.gameObject.GetComponent<EnterPlaneLogic>().EnterPlane(this.gameObject);
            }
        }
        //EXIT BUTTON
        else if (Physics.Raycast(ray, out hit, lookDistance, Exitbutton))
        {
            hitObject = hit.transform.gameObject;
            hit.transform.gameObject.GetComponent<Renderer>().material = highlightColor;
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.gameObject.GetComponent<ExitPlaneLogic>().ExitPlane(this.gameObject);
            }
        }
        //RELOAD BUTTON
        else if (Physics.Raycast(ray, out hit, lookDistance, ReloadButton))
        {
            hitObject = hit.transform.gameObject;
            hit.transform.gameObject.GetComponent<Renderer>().material = highlightColor;
            if (Input.GetKeyDown(KeyCode.E))
            {
                hit.transform.gameObject.GetComponent<ReloadPlaneLogic>().ReloadPlane(this.gameObject);
            }
        }
        else
        {
            if (hitObject != null) // Check if hitObject is not null
            {
                hitObject.GetComponent<Renderer>().material = normalColor;
                hitObject = null; // Reset hitObject for the next frame
            }
        }
    }
}
