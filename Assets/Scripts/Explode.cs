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
    public int damage = 100;
    public float radius = 100f;
    private PhotonView photonView;
    
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    
    void Start()
    {
        // Check if the MasterClient is not already the owner
        if (photonView.Owner != PhotonNetwork.MasterClient)
        {
            // Transfer ownership of this PhotonView to the MasterClient
             photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != plane && other.gameObject != leftGear && other.gameObject != rightGear && other.gameObject != planeBody)
        {
            PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
            DamageUtility.CalculateExplosionDamage(transform.position, radius, damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }
    
    public void Explosion()
    {
        PhotonNetwork.Instantiate(explosion.name, transform.position, Quaternion.identity);
        DamageUtility.CalculateExplosionDamage(transform.position, radius, damage);
        
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        else
        {
            photonView.RPC("DestroyObject", RpcTarget.MasterClient, photonView.ViewID);
        }
    }
    
    [PunRPC]
    public void DestroyObject(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID).gameObject);
    }
}
