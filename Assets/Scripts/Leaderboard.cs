using UnityEngine;
using System.Linq;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;
using System;

public class Leaderboard : MonoBehaviour
{
    //Using roomManager to get the team list roomManager.teams
    //public RoomManager roomManager;
    public GameObject playersHolder;
    public List<Team> teams = new List<Team>();

    [Header("Options")] 
    public float refreshRate = 1f;

    [Header("UI")] 
    public GameObject[] slots; //4 slots of team with 2 players each

    [Space]
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] kdTexts;
    public GameObject[] playerHolder;
    public GameObject Playerholder2;

    private void Start() 
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
        for(int i = 0; i < slots.Length; i++)
        {
            Team team = new Team(null, null);
            teams.Add(team);
        }
    }

    public void Refresh()
    {
        foreach(var slot in slots)
        {
            slot.SetActive(false);
        }

        var sortedPlayerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();

        //Populate teams
        foreach(var player in PhotonNetwork.PlayerList)
        {
            foreach(var team in teams)
            {
                if(team.Player1 == null)
                {
                    team.Player1 = player;
                    break;
                }
                else if(team.Player2 == null)
                {
                    team.Player2 = player;
                    break;
                }
            }
        }

        int i = 0; //Index of position
        int j = 0; //Index of player
        Boolean player1active = false;
        Boolean player2active = false;

        foreach(var team in teams)
        {
            if(team.Player1 != null)
            {
                player1active = true;
            }

            if(team.Player2 != null)
            {
                player2active = true;
            }

            if(player1active && player2active)
            {
                //Debug.Log("Both players active");
                slots[i].SetActive(true);
                scoreTexts[i].text = (team.Player1.GetScore() + team.Player2.GetScore()).ToString();
                kdTexts[i].text = ((int)team.Player1.CustomProperties["kills"] + (int)team.Player2.CustomProperties["kills"]).ToString() + "/" + ((int)team.Player1.CustomProperties["deaths"] + (int)team.Player2.CustomProperties["death"]).ToString();
                i++;
                
                playerHolder[j].SetActive(true);
                if(team.Player1.NickName == "")
                {
                    team.Player1.NickName = "anonymous";
                }
                else
                {
                    nameTexts[i].text = team.Player1.NickName;
                }

                scoreTexts[i].text = team.Player1.GetScore().ToString();
                kdTexts[i].text = team.Player1.CustomProperties["kills"].ToString() + "/" + team.Player1.CustomProperties["deaths"].ToString();
                i++;
                j++;

                playerHolder[j].SetActive(true);
                if(team.Player2.NickName == "")
                {
                    team.Player2.NickName = "anonymous";
                }
                else
                {
                    nameTexts[i].text = team.Player2.NickName;
                }

                scoreTexts[i].text = team.Player2.GetScore().ToString();
                kdTexts[i].text = team.Player2.CustomProperties["kills"].ToString() + "/" + team.Player2.CustomProperties["deaths"].ToString();
            }
            else if(player1active)
            {
                Debug.Log("Player 1 active");
                slots[i].SetActive(true);
                scoreTexts[i].text = team.Player1.GetScore().ToString();
                kdTexts[i].text = team.Player1.CustomProperties["kills"].ToString() + "/" + team.Player1.CustomProperties["deaths"].ToString();
                i++;

                playerHolder[j].SetActive(true);
                if(team.Player1.NickName == "")
                {
                    team.Player1.NickName = "anonymous";
                }
                else
                {
                    nameTexts[i].text = team.Player1.NickName;
                }
                scoreTexts[i].text = team.Player1.GetScore().ToString();
                kdTexts[i].text = team.Player1.CustomProperties["kills"].ToString() + "/" + team.Player1.CustomProperties["deaths"].ToString();
                i++;
                j++;

                playerHolder[j].SetActive(false);
            }
        }

        // int i = 0; 

        // foreach(var player in sortedPlayerList)
        // {
        //     slots[i].SetActive(true);

        //     if(player.NickName == "")
        //     {
        //         player.NickName = "anonymous";
        //     }

        //     nameTexts[i].text = player.NickName;
        //     scoreTexts[i].text = player.GetScore().ToString();

        //     if(player.CustomProperties["kills"] != null)
        //     {
        //         kdTexts[i].text = player.CustomProperties["kills"].ToString() + "/" + player.CustomProperties["deaths"].ToString();
        //     }
        //     else
        //     {
        //         kdTexts[i].text = "0/0";
        //     }

        //     i++;
        // }
    }

    private void Update() {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}
