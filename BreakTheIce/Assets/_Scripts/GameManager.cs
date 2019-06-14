using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public int playerType { set; get; }

    public GameObject Bear;
    public GameObject Penguin;

    public Camera polarBearCamera;
    public Camera penguinCamera;

    public Canvas PolarBearUI;
    //public Canvas PenguinUI;

    public bool gameStarted = false; // 0=not started, 1=started
    public bool gameEnded = false;   // 0=not ended, 1=ended

    #region SetUpFunctions
    private void Awake()
    {
        Bear.SetActive(false);
        //GameObject.Find("Penguin").SetActive(false);
    }
    private void Start()
    {
        Instance = this;

        // no active camera until player recieves role
        polarBearCamera.enabled = false;
        penguinCamera.enabled = false;
        PolarBearUI.enabled = false;
        //PenguinUI.enabled = false;
    }
    // set player camera dependednt on player type
    public void SetCamera()
    {
        if (playerType == PlayerType.PolarBear)
        {
            Bear.SetActive(true);   // move this at some point
            polarBearCamera.enabled = true;
            PolarBearUI.enabled = true;
        }
        else if (playerType == PlayerType.Penguin)
        {
            penguinCamera.enabled = true;
            //PenguinUI.enabled = true;
        }
        else
        {
            Debug.Log("Error in SetCamera() in GameManager.cs");
        }
    }
    #endregion

    private void Update()
    {
        CheckGameStart();
    }

    public void UpdateDropForPenguin(PieceType.PType pieceType, float xPos, float yPos)
    {
        GameObject temp = GameObject.Instantiate(PieceSpawner.Instance.piecePrefabs[(int)pieceType], new Vector3(xPos, 10f, yPos), Quaternion.identity);
        Debug.Log(temp);
        temp.GetComponent<Rigidbody>().useGravity = true;
        //UnityEditor.EditorApplication.isPaused = true;
    }
    public void UpdatePenguinMovement(float xPos, float yPos, float zPos)
    {
        Penguin.transform.position = new Vector3(xPos, yPos, zPos);
    }

    #region GameStart/ EndGame
    public void CheckGameStart()
    {
        // if the game has not yet started and only the polar bear may start the game
        if(!gameStarted && !gameEnded && playerType == PlayerType.PolarBear)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameStarted = true;
                Client.Instance.SendGameStarted(gameStarted);
                Debug.Log("Starting Game!");
                return;
            }
        }
    }
    public void SetStartGame(bool start)
    {
        gameStarted = start;
    }
    public void SendGameEnded()
    {
        gameEnded = true;
        Client.Instance.SendGameEnded(gameEnded);
        Debug.Log("Ending Game!");
        return;
    }
    public void SetGameEnded(bool end)
    {
        gameEnded = end;
    }
    #endregion
}
