using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class AirplaneBulletLogic : MonoBehaviour
{
    public GameObject bulletExplosion;
    public int damage = 10;
    public float radius = 2f;
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
        if(GetComponent<PhotonView>().IsMine && !other.transform.gameObject.GetComponent<DealDamagePlaneGun>())
        {
            PhotonNetwork.Instantiate(bulletExplosion.name, transform.position, Quaternion.identity);
            DamageUtility.CalculateExplosionDamage(transform.position, radius, damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
