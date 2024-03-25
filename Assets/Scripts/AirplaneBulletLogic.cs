using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AirplaneBulletLogic : MonoBehaviour
{
    public GameObject bulletExplosion;
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
        if(GetComponent<PhotonView>().IsMine)
        {
            PhotonNetwork.Instantiate(bulletExplosion.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
