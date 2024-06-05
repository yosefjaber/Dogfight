using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class DealDamageBomb : MonoBehaviour
{
    public int damage = 30;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.gameObject.GetComponent<Health>())
        {
            if(damage >= other.transform.gameObject.GetComponent<Health>().health)
            {
                //kill
                RoomManager.instance.kills++;
                RoomManager.instance.SetHashes();
                PhotonNetwork.LocalPlayer.AddScore(100);
            }

            other.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
        }
    }
}
