using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public int playerType { set; get; }

    public GameObject Bear;
    public Camera polarBearCamera;
    public Camera penguinCamera;

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
    }
    // set player camera dependednt on player type
    public void SetCamera()
    {
        if (playerType == PlayerType.PolarBear)
        {
            Bear.SetActive(true);   // move this at some point
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
    #endregion

    public void UpdateDropForPenguin(PieceType.PType pieceType, float xPos, float yPos)
    {
        GameObject temp = GameObject.Instantiate(PieceSpawner.Instance.piecePrefabs[(int)pieceType], new Vector3(xPos, 10f, yPos), Quaternion.identity);
        Debug.Log(temp);
        temp.GetComponent<Rigidbody>().useGravity = true;
        //UnityEditor.EditorApplication.isPaused = true;
    }
}
