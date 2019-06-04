using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public int playerType { set; get; }

    public Camera polarBearCamera;
    public Camera penguinCamera;

    private void Start()
    {
        Instance = this;

        // no active camera until player recieves role
        polarBearCamera.enabled = false;
        penguinCamera.enabled = false;
    }

    // set player camera dependednt on player type
    public void SetCamera()
    {
        if (playerType == PlayerType.PolarBear)
        {
            polarBearCamera.enabled = true;
        }
        else if (playerType == PlayerType.Penguin)
        {
            penguinCamera.enabled = true;
        }
        else
        {
            Debug.Log("Error in SetCamera() in GameManager.cs");
        }
    }
}
