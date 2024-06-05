using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CaseBomb : MonoBehaviour
{
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.transform.gameObject.GetComponent<Health>() != null && other.transform.gameObject.GetComponent<PlaneLogic>() != null)
        // {
        //     explode();
        // }
        explode();
    }

    public void explode()
    {
        PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
        // Optionally, destroy the bomb game object to simulate the explosion effect
        //Destroy(this.gameObject, 0.1f);
        Destroy(this.gameObject);
    }
}
