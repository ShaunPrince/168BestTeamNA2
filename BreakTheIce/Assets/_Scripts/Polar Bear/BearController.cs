using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public GameObject ActivePiece;
    public GameObject nextPiece;
    public GameObject ps;

    public float gridSize;

    private float deltaZ;
    private float deltaX;

    private bool canDropPiece = false;

    public bool pieceFalling;

    private void Awake()
    {
        deltaZ = 0;
        deltaX = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        nextPiece = ps.GetComponent<PieceSpawner>().GenerateRandomPiece();
        nextPiece.transform.position = new Vector3(1000, 1000, 1000);
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
        if(ActivePiece != null)
        {
            CheckGameStatus();
            CheckInput();

        }


    }

    private void FixedUpdate()
    {
        MoveActivePiece();
        ResetInput();
    }

    private void CheckGameStatus()
    {
        if (GameManager.Instance.gameStarted) canDropPiece = true;
        if (GameManager.Instance.gameEnded) canDropPiece = false;
    }

    private void CheckInput()
    {
        if(!pieceFalling && ActivePiece != null)
        {
            if(canDropPiece && Input.GetKeyDown(KeyCode.Return))
            {
                DropPiece();
                return;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                deltaZ += gridSize;
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                deltaZ -= gridSize;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                deltaX += gridSize;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                deltaX -= gridSize;
            }
        }

        
    }


    public void DropPiece()
    {
        pieceFalling = true;
        ActivePiece.GetComponent<Rigidbody>().useGravity = true;

        // Notify server that a peice has been dropped
        Client.Instance.SendPieceDropped(ActivePiece.GetComponent<Piece>().GetPieceType(), ActivePiece.transform.position.x, ActivePiece.transform.position.z);

    }

    public void MoveActivePiece()
    {
        //possibly need to add buffer time to prevent too fast movement
        //also need to check bounds
        if(ActivePiece != null)
        {
            ActivePiece.GetComponent<Rigidbody>().MovePosition(ActivePiece.transform.position + new Vector3(deltaX, 0, deltaZ));

        }
    }

    public void ResetInput()
    {
        deltaZ = 0f;
        deltaX = 0f;
    }

    public void SpawnPiece()
    {
        nextPiece.transform.position = new Vector3(0, 10, 0);
        ActivePiece = nextPiece;
        
        nextPiece = ps.GetComponent<PieceSpawner>().GenerateRandomPiece();
        nextPiece.transform.position = new Vector3(1000, 1000, 1000);

        pieceFalling = false;
    }




}
