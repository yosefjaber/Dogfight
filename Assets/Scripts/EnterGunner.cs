using UnityEngine;
using Photon.Pun;
using PhotonNetwork = Photon.Pun.PhotonNetwork;

public class EnterGunner : MonoBehaviour
{
    [Header("References")]
    public GameObject gunCamera;
    public GameObject bulletSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterGunnerLogic(GameObject player)
    {
        PhotonNetwork.Destroy(player);
        gunCamera.SetActive(true);
        bulletSpawn.SetActive(true);
    }
}
