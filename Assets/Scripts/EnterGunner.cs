using UnityEngine;
using Photon.Pun;
using PhotonNetwork = Photon.Pun.PhotonNetwork;

public class EnterGunner : MonoBehaviour
{
    [Header("References")]
    public GameObject gunCamera;
    public GameObject bulletSpawn;
    public GameObject rotatePoint;

    public GameObject GunnerObject;
    public RiderInfo riderInfo;
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
        riderInfo.photonView.RPC("SetGunner", RpcTarget.All, player.GetComponent<PhotonView>().ViewID);
        NetworkDestroyer.Instance.RequestDisable(player);
        gunCamera.SetActive(true);
        bulletSpawn.SetActive(true);
        GunnerLogic gunnerLogic = rotatePoint.GetComponent<GunnerLogic>();
        gunnerLogic.enabled = true;
        gunnerLogic.gunnerUser = player;
    }
}
