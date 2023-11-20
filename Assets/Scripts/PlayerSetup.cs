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

    public void IsLocalPlayer(){
        movement.enabled = true;
        camera.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string name){
        nickname = name;
        
        nicknameText.text = nickname;
    }
}
