using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private int PolarBearConnID; // 0
    private int PenguinConnID;   // 1
    private int PlayerCount = 0;
    private bool NewGame = true;    // set to false if player has left game but not all players have left

    private void Start()
    {
        Instance = this;
    }

    public int addPlayer()
    {
        // returns 1 on success, 0 on failur
        if (NewGame && PlayerCount < 2) {
            PlayerCount += 1;
            return 1;
        }
        else
        {
            return 0;
        }
    }

    public int removePlayer()
    {
        if (PlayerCount > 0)
        {
            NewGame = false;
            PlayerCount -= 1;
            // if all players are gone, can start a new game
            if (PlayerCount == 0)
            {
                NewGame = true;
                return 2;
            }
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
