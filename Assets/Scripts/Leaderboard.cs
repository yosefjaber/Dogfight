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
    public TextMeshProUGUI[] scoreTexts;
    public TextMeshProUGUI[] nameTexts;
    public TextMeshProUGUI[] kdTexts;
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
            // Try to add player to an existing team
            bool addedToTeam = false;
            foreach (var team in teams)
            {
                if (team.TryAddPlayer(player))
                {
                    addedToTeam = true;
                    break;
                }
            }

            if (!addedToTeam)
            {
                Debug.LogWarning("Failed to add player to any team. Consider increasing the number of teams.");
            }
        }
    }

    private void UpdateUI()
    {
        int slotIndex = 0;
        int playerIndex = 0;
        int index = 0;
        Team highestScoreTeam = teams.OrderByDescending(t => t.TotalScore).FirstOrDefault();

        foreach (var team in teams.Where(t => t.HasPlayers))
        {
            // Update team slot UI
            if (team == highestScoreTeam && highestScoreTeam.TotalScore > 0 && nameTexts[slotIndex].text.Substring(nameTexts[slotIndex].text.Length - 3) != "!!!")
            {
                nameTexts[slotIndex].text = nameTexts[slotIndex].text + "!!!";
            }
            else if (team != highestScoreTeam && nameTexts[slotIndex].text.Substring(nameTexts[slotIndex].text.Length - 3) == "!!!")
            {
                nameTexts[slotIndex].text = nameTexts[slotIndex].text.Substring(0, nameTexts[slotIndex].text.Length - 3);
            }
            slots[slotIndex].SetActive(true);
            scoreTexts[index].text = team.TotalScore.ToString();
            kdTexts[index].text = $"{team.TotalKills}/{team.TotalDeaths}";
            slotIndex++;
            index++;

            // Update individual player UI
            foreach (var player in team.Players)
            {
                playerHolder[playerIndex].SetActive(true);
                nameTexts[index].text = string.IsNullOrEmpty(player.NickName) ? "anonymous" : player.NickName;
                scoreTexts[index].text = player.GetScore().ToString();
                kdTexts[playerIndex].text = $"{player.CustomProperties["kills"] ?? "0"}/{player.CustomProperties["deaths"] ?? "0"}";
                playerIndex++;
                index++;
            }
        }
    }

    private void Update()
    {
        playersHolder.SetActive(Input.GetKey(KeyCode.Tab));
    }
}