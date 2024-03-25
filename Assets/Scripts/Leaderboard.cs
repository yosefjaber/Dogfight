using UnityEngine;
using System.Linq;
using TMPro;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;

public class Leaderboard : MonoBehaviour
{
    public GameObject playersHolder;
    public List<Team> teams = new List<Team>();

    [Header("Options")] 
    public float refreshRate = 1f;

    [Header("UI")] 
    public GameObject[] slots; // 4 slots for teams
    public TextMeshProUGUI[] teamScoreTexts;
    public TextMeshProUGUI[] playerScoreTexts;
    public TextMeshProUGUI[] teamNameTexts;
    public TextMeshProUGUI[] playerNameTexts;
    public TextMeshProUGUI[] teamKdTexts;
    public TextMeshProUGUI[] playerKdTexts;
    public GameObject[] playerHolder; // Individual player UI elements

    private void Awake() 
    {
        // Initialize teams list with empty teams
        teams = Enumerable.Range(0, slots.Length).Select(_ => new Team()).ToList();
    }

    private void Start() 
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        // Reset all UI elements to inactive
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        foreach (var player in playerHolder)
        {
            player.SetActive(false);
        }

        // Clear existing player assignments
        foreach (var team in teams)
        {
            team.ClearPlayers();
        }

        // Assign players to teams based on current player list
        AssignPlayersToTeams();

        // Update UI
        UpdateUI();
    }

    private void AssignPlayersToTeams()
    {
        var players = PhotonNetwork.PlayerList;
        
        foreach (var player in players)
        {
            bool playerAlreadyAssigned = false;
            foreach (var team in teams)
            {
                if (team.Players.Contains(player))
                {
                    playerAlreadyAssigned = true;
                    break;
                }
            }

            if (!playerAlreadyAssigned)
            {
                foreach (var team in teams)
                {
                    if (team.TryAddPlayer(player))
                    {
                        break;
                    }
                }
            }
        }
    }

    private void UpdateUI()
    {
        int playerIndex = 0;
        int teamIndex = 0;

        Team highestScoreTeam = teams.OrderByDescending(t => t.TotalScore).FirstOrDefault();

        foreach (var team in teams.Where(t => t.HasPlayers))
        {
            // Update team slot UI
            if (team == highestScoreTeam && highestScoreTeam.TotalScore > 0 && teamNameTexts[teamIndex].text.Substring(teamNameTexts[teamIndex].text.Length - 3) != "!!!")
            {
                teamNameTexts[teamIndex].text = teamNameTexts[teamIndex].text + "!!!";
                Debug.Log("Highest score team: " + teamNameTexts[teamIndex].text);
            }
            else if (team != highestScoreTeam && teamNameTexts[teamIndex].text.Substring(teamNameTexts[teamIndex].text.Length - 3) == "!!!")
            {
                teamNameTexts[teamIndex].text = teamNameTexts[teamIndex].text.Substring(0, teamNameTexts[teamIndex].text.Length - 3);
            }
            slots[teamIndex].SetActive(true);
            teamScoreTexts[teamIndex].text = team.TotalScore.ToString();
            teamKdTexts[teamIndex].text = $"{team.TotalKills}/{team.TotalDeaths}";
            teamIndex++;

            // Update individual player UI
            foreach (var player in team.Players)
            {
                playerHolder[playerIndex].SetActive(true);
                playerNameTexts[playerIndex].text = string.IsNullOrEmpty(player.NickName) ? "anonymous" : player.NickName;
                playerScoreTexts[playerIndex].text = player.GetScore().ToString();
                playerKdTexts[playerIndex].text = $"{player.CustomProperties["kills"] ?? "0"}/{player.CustomProperties["deaths"] ?? "0"}";
                playerIndex++;
            }
        }
    }

    private void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}