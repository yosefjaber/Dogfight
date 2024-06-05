using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public static class DamageUtility
{
    public static void CalculateExplosionDamage(Vector3 explosionPosition, float explosionRadius, int explosionDamage)
    {
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Health healthComponent = obj.GetComponent<Health>();
            if (healthComponent && Vector3.Distance(obj.transform.position, explosionPosition) < explosionRadius)
            {
                if (healthComponent.health <= explosionDamage)
                {
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    PhotonNetwork.LocalPlayer.AddScore(100);
                }
                obj.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, explosionDamage);
            }
        }
    }
}

