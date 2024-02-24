using System.Security.Cryptography;
using UnityEngine;
using Photon.Pun;

public class AirplaneGun : MonoBehaviour
{
    public GameObject gunBullet;
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
        GameObject bullet = PhotonNetwork.Instantiate(gunBullet.name, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.AddForce(Vector3.up * 1000);
        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;

        Destroy(bullet, 5f);

        Debug.Log("Gun Fired");
    }
}
