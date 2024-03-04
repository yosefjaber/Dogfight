using UnityEngine;
using System.Linq;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections.Generic;

public class Team
{
    public List<Player> Players = new List<Player>();

    public bool TryAddPlayer(Player player)
    {
        if (Players.Count < 2) // Assuming each team can have 2 players
        {
            Players.Add(player);
            return true;
        }
        return false;
    }

    public void ClearPlayers()
    {
        Players.Clear();
    }

    public bool HasPlayers => Players.Any();

    public int TotalScore => Players.Sum(player => player.GetScore());
    public int TotalKills => Players.Where(player => player != null && player.CustomProperties.ContainsKey("kills")).Sum(player => (int)player.CustomProperties["kills"]);
    public int TotalDeaths => Players.Where(player => player != null && player.CustomProperties.ContainsKey("deaths")).Sum(player => (int)player.CustomProperties["deaths"]);
}
