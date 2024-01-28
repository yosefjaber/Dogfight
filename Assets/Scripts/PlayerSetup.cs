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

    public void IsLocalPlayer(){
        movement.enabled = true;
        camera.SetActive(true);
        SetLayer(player, "LocalPlayer");
        SetLayer(playerTag, "LocalTag");
        SetLayer(playerEyes, "LocalEyes");
    }

    [PunRPC]
    public void SetNickname(string name){
        nickname = name;
        
        nicknameText.text = nickname;
    }

    // Assign the GameObject to a layer
    public void SetLayer(GameObject obj, string layerName)
    {
        // Check if the layer exists
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
