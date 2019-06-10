using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private int PolarBearConnID; // 0
    private int PenguinConnID;   // 1
    private int PlayerCount = 0;

    private void Start()
    {
        Instance = this;
    }

    public int addPlayer()
    {
        // returns 1 on success, 0 on failur
        if (PlayerCount < 2) {
            PlayerCount += 1;
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
