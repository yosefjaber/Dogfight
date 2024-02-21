using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class Weapon : MonoBehaviour
{

    public int damage;

    public float fireRate;

    public Camera camera;

    [Header("VFX")]
    public GameObject hitVFX;

    [Header("Ammo")]
    //how many mags you have
    public int mag = 5;
    //How much ammo you start with
    public int ammo = 30; 
    //How many ammo in each mag
    public int magAmmo = 30;

    [Header("UI")]
    public TextMeshProUGUI magText;
    public TextMeshProUGUI ammoText;
    private float nextFire;

    [Header("Animation")]
    public Animation animation;
    public AnimationClip reload;

    [Header("Recoil Settings")]
    // [Range(0, 1)]
    // public float recoilPercent = 0.3f;
    [Range(0, 2)]
    public float recoverPercent = 0.7f;
    [Space]
    public float recoilUp = 0.02f;
    public float recoilBack = 0f;

    private Vector3 originalPosition;
    private Vector3 recoilVelocity = Vector3.zero;

    private float recoilLength;
    private float recoverLength;

    private bool recoiling;
    private bool recovering;


    void Start()
    {
        magText.text = mag.ToString();
        ammoText.text = ammo.ToString() + "/" + magAmmo.ToString();

        originalPosition = transform.localPosition;

        recoilLength = 0;
        recoverLength = (1/fireRate) * recoverPercent;
    }

    // Update is called once per frame
    void Update()
    {
        if(nextFire > 0)
        {
            nextFire -= Time.deltaTime;
        }


        if(Input.GetButton("Fire1") && nextFire <= 0 && ammo > 0 && !animation.isPlaying)
        {
            nextFire = 1 / fireRate;

            ammo--;
            ammoText.text = ammo.ToString() + "/" + magAmmo.ToString();

            Fire();
        }

        if(Input.GetKeyDown(KeyCode.R) && mag > 0 && ammo < magAmmo && !animation.isPlaying)
        {
            Reload();
        }

        if(recoiling)
        {
            Recoil();
        }

        if(recovering)
        {
            Recovering();
        }
    }

    void Reload()
    {
        animation.Play(reload.name);
        if(mag > 0)
        {
            mag--;
            ammo = magAmmo;
        }

        magText.text = mag.ToString();
        ammoText.text = ammo.ToString() + "/" + magAmmo.ToString();
    }

    void Fire()
    {
        recoiling = true;
        recovering = false;
        Ray ray = new Ray(camera.transform.position, camera.transform.forward);

        RaycastHit hit; 

        int PlayerMask = 1 << LayerMask.NameToLayer("LocalPlayer");
        //Invert the bitmask to ignore the specified layer
        PlayerMask = ~PlayerMask;

        if(Physics.Raycast(ray.origin, ray.direction, out hit, 100f, PlayerMask))
        {
            PhotonNetwork.Instantiate(hitVFX.name, hit.point, Quaternion.identity);

            if(hit.transform.gameObject.GetComponent<Health>())
            {
                if(damage >= hit.transform.gameObject.GetComponent<Health>().health)
                {
                    //kill
                    RoomManager.instance.kills++;
                    RoomManager.instance.SetHashes();
                    PhotonNetwork.LocalPlayer.AddScore(100);
                }

                hit.transform.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage);
            }

            if(hit.transform.gameObject.GetComponent<CaseBomb>())
            {
                CaseBomb caseBomb = hit.transform.gameObject.GetComponent<CaseBomb>();

                caseBomb.explode();
            }
        }
    }

    void Recoil()
    {
        Vector3 finalPosition = new Vector3(originalPosition.x, originalPosition.y + recoilUp, originalPosition.z - recoilBack);

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoilLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = true;
        }
    }

    void Recovering()
    {
        Vector3 finalPosition = originalPosition;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref recoilVelocity, recoverLength);

        if(transform.localPosition == finalPosition)
        {
            recoiling = false;
            recovering = false;
        }
    }
}
