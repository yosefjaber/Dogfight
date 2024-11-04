using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine.SocialPlatforms;

public static class DamageUtility
{
    public static void CalculateExplosionDamage(Vector3 explosionPosition, float explosionRadius, int explosionDamage)
    {
        GameObject[] allObjectsWithHealth = Object.FindObjectsOfType<GameObject>().Where(obj => obj.GetComponent<Health>() != null).ToArray();
        foreach (GameObject obj in allObjectsWithHealth)
        {
            if (Vector3.Distance(obj.transform.position, explosionPosition) < explosionRadius)
            {
                if (obj.GetComponent<Health>().health <= explosionDamage)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    PhotonNetwork.LocalPlayer.AddScore(100); 
                }
                obj.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, explosionDamage);
                Debug.Log("Explosion Damage: " + explosionDamage);
            }
        }
    }
}

