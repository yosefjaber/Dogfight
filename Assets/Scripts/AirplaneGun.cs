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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Airplane script has started");
    }

    // Update is called once per frame
    void Update()
    {
        shootingTimer -= Time.deltaTime;

        if (shootingTimer <= 0 && Input.GetMouseButton(0)) 
        {
            Shoot();
            shootingTimer = shootingRate;
        }


    }

    void Shoot()
    {
        // Correct instantiation with Quaternion multiplication for the desired rotation
        GameObject bullet = PhotonNetwork.Instantiate(gunBullet.name, bulletSpawnPoint.position, plane.transform.rotation * Quaternion.Euler(90, 0, 0));

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // Set velocity directly to move the bullet forward, assuming bulletSpawnPoint is correctly oriented
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;

        Destroy(bullet, 5f); // Cleanup to avoid excessive GameObjects in the scene

        Debug.Log("Gun Fired");
    }
}
