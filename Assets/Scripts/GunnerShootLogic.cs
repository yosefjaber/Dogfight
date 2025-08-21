using UnityEngine;
using Photon.Pun;

public class GunnerShootLogic : MonoBehaviour
{
    public GameObject gunBullet;
    public GameObject plane;
    public GameObject gunBarrel;
    public float bulletSpeed = 1000f;
    public Transform bulletSpawnPoint;
    public float shootingRate = 0.5f;
    private float timer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (timer >= shootingRate)
        {
            timer = 0f;
            createBullet();
        }
    }
    
    public void createBullet()
    {
        // Correct instantiation with Quaternion multiplication for the desired rotation
        GameObject bullet = PhotonNetwork.Instantiate(gunBullet.name, bulletSpawnPoint.position, gunBarrel.transform.rotation * Quaternion.Euler(0, 0, 0));
    
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
    
        // Set velocity directly to move the bullet forward, assuming bulletSpawnPoint is correctly oriented, also add the plane velocity to the rb
        Vector3 planeVelocity = plane.GetComponent<Rigidbody>().linearVelocity;
        rb.linearVelocity = (bulletSpawnPoint.forward * bulletSpeed) + planeVelocity;
    
        Destroy(bullet, 5f); // Cleanup to avoid excessive GameObjects in the scene
    }
}
