using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class Team 
{
    private Player player1;
    private Player player2;

    public Team(Player player1, Player player2)
    {
        this.player1 = player1;
        this.player2 = player2;
    }

    public Player Player1
    {
        get { return player1; }
        set { player1 = value; }
    }

    public Player Player2
    {
        get { return player2; }
        set { player2 = value; }
    }
}
