using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public int playerType { set; get; }

    private void Start()
    {
        Instance = this;
    }
}
