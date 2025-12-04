using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerSetup : MonoBehaviour
{
    public Movement movement;
    public GameObject camera;
    public string nickname;
    public TextMeshPro nicknameText;
    public GameObject player;
    public GameObject playerTag;
    public GameObject playerEyes;

    void Awake()
    {
        // Immediately disable AudioListener for non-local players
        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (listener != null)
        {
            // Disable by default, will be re-enabled in IsLocalPlayer() if needed
            listener.enabled = false;
        }
    }

    void Start()
    {
        // Double check AudioListener is disabled for non-local players
        if (!GetComponent<PhotonView>().IsMine)
        {
            AudioListener listener = GetComponentInChildren<AudioListener>();
            if (listener != null)
            {
                listener.enabled = false;
            }
        }
    }

    public void Initialize()
    {
        Debug.Log($"Player {nickname} initialized");
    }

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        camera.SetActive(true);
        
        // Enable AudioListener ONLY for local player
        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (listener != null)
        {
            listener.enabled = true;
            Debug.Log("AudioListener enabled for local player");
        }
        
        SetLayer(player, "LocalPlayer");
        SetLayer(playerTag, "LocalTag");
        SetLayer(playerEyes, "LocalEyes");
    }

    [PunRPC]
    public void SetNickname(string name)
    {
        nickname = name;
        nicknameText.text = nickname;
    }

    public void SetLayer(GameObject obj, string layerName)
    {
        if (LayerMask.NameToLayer(layerName) != -1)
        {
            obj.layer = LayerMask.NameToLayer(layerName);
        }
        else
        {
            Debug.LogError("Layer not found: " + layerName);
        }
    }
}