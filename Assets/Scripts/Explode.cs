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
        PhotonNetwork.Destroy(gameObject);
    }
}
