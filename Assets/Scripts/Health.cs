using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Pun.UtilityScripts;

public class Health : MonoBehaviour
{
    public int health;
    public bool IsLocalPlayer;
    public bool testPlayer;
    public Material hurtColor;
    private Material originalColor;
    public bool isPlane;

    private KillPlane killPlane;

    [Header("UI")]
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        if(isPlane)
        {
            killPlane = this.gameObject.GetComponent<KillPlane>();
        }
    }


    private void Start()
    {
        originalColor = GetComponent<Renderer>().material;
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if(testPlayer)
        {
            Debug.Log("Health: " + health);
            GetComponent<Renderer>().material = hurtColor;
            Invoke("ResetColor", 0.3f);
        }
        else if (!isPlane)
        {

            healthText.text = health.ToString();
        }

        if (health <= 0)
        {
            if (isPlane)
            {
                killPlane.blowUpPlane();
            }
            else
            {
                if (IsLocalPlayer)
                {
                    RoomManager.instance.SpawnPlayer();
                    RoomManager.instance.deaths++;
                    RoomManager.instance.SetHashes();
                    PhotonNetwork.LocalPlayer.AddScore(-100);
                }
                
                if(!testPlayer)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void ResetColor()
    {
        GetComponent<Renderer>().material = originalColor;
    }
}
