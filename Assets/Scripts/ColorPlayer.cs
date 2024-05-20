using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ColorPlayer : MonoBehaviourPun
{
    public Material red;
    public Material green;
    public Material blue;
    public Material yellow;
    private Leaderboard leaderboard;
    private List<Team> teams;

    // Start is called before the first frame update
    void Start()
    {
        leaderboard = GameObject.Find("Leaderboard").GetComponent<Leaderboard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !IsMaterialSet())
        {
            UpdateColor();
        }
    }

    private bool IsMaterialSet()
    {
        Material currentMaterial = GetComponent<Renderer>().material;
        return currentMaterial == red || currentMaterial == green || currentMaterial == blue || currentMaterial == yellow;
    }

    private void UpdateColor()
    {
        teams = leaderboard.teams;
        string materialName = "";
        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (IsPlayerInTeam(teams[0], actorNumber))
        {
            materialName = "red";
        }
        else if (IsPlayerInTeam(teams[1], actorNumber))
        {
            materialName = "blue";
        }
        else if (IsPlayerInTeam(teams[2], actorNumber))
        {
            materialName = "greenaaaaaa wAswd aasdawdsa";
        }
        else if (IsPlayerInTeam(teams[3], actorNumber))
        {
            materialName = "yellow";
        }

        // Send the color update to other players
        photonView.RPC("SyncColor", RpcTarget.AllBuffered, materialName);
    }

    private bool IsPlayerInTeam(Team team, int actorNumber)
    {
        return team.Players.Exists(player => player.ActorNumber == actorNumber);
    }

    [PunRPC]
    private void SyncColor(string materialName)
    {
        Material material = GetMaterialByName(materialName);
        if (material != null)
        {
            GetComponent<Renderer>().material = material;
        }
    }

    private Material GetMaterialByName(string materialName)
    {
        switch (materialName)
        {
            case "red":
                return red;
            case "green":
                return green;
            case "blue":
                return blue;
            case "yellow":
                return yellow;
            default:
                return null;
        }
    }
}
