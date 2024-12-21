using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;

public class AirplaneGun : MonoBehaviour
{
    public GameObject gunBullet;
    public GameObject plane;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 1000f;
    public float shootingRate = 0.5f;
    private float shootingTimer = 0.5f;
    public int maxAmmo = 40;
    public int currentAmmo = 40;
    public PhotonView photonView;

    void Start()
    {
        Debug.Log("Airplane script has started");
    }

    void Update()
    {
        shootingTimer -= Time.deltaTime;
    }

    public void Shoot()
    {
        if(shootingTimer <= 0)
        {
            shootingTimer = shootingRate;
            photonView.RPC("updateAmmo", RpcTarget.All, currentAmmo - 1);
            createBullet();
        }
    }
    
    public void createBullet()
    {
        // Correct instantiation with Quaternion multiplication for the desired rotation
        GameObject bullet = PhotonNetwork.Instantiate(gunBullet.name, bulletSpawnPoint.position, plane.transform.rotation * Quaternion.Euler(90, 0, 0));
    
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
    
        // Set velocity directly to move the bullet forward, assuming bulletSpawnPoint is correctly oriented, also add the plane velocity to the rb
        Vector3 planeVelocity = plane.GetComponent<Rigidbody>().linearVelocity;
        rb.linearVelocity = (bulletSpawnPoint.forward * bulletSpeed) + planeVelocity;
    
        Destroy(bullet, 5f); // Cleanup to avoid excessive GameObjects in the scene
    }
    
    [PunRPC]
    private void updateAmmo(int ammo)
    {
        currentAmmo = ammo;
        Debug.Log("Current Ammo: " + currentAmmo);
    }
    
    public void Reload()
    {
        photonView.RPC("updateAmmo", RpcTarget.All, maxAmmo);
        Debug.Log("Reloaded");
    }
}
