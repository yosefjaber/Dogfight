using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class Explode : MonoBehaviour
{
    [Header("Explode effect")]
    public GameObject explosion; 
    public GameObject plane;
    public GameObject leftGear;
    public GameObject rightGear;
    public GameObject planeBody;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != plane && other.gameObject != leftGear && other.gameObject != rightGear && other.gameObject != planeBody)
        {
            Debug.Log(other.gameObject.name);
            PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            // Optionally, destroy the bomb game object to simulate the explosion effect
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
