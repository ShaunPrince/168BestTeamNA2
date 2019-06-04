using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public GameObject ActivePiece;
    public GameObject ps;

    public float gridSize;

    private float deltaZ;
    private float deltaX;

    public bool pieceFalling;

    private void Awake()
    {
        deltaZ = 0;
        deltaX = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
        if(ActivePiece != null)
        {
            CheckInput();

        }


    }

    private void FixedUpdate()
    {
        MoveActivePiece();
        ResetInput();
    }

    private void CheckInput()
    {
        if(!pieceFalling && ActivePiece != null)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                DropPiece();
                return;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                deltaZ += gridSize;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                deltaZ -= gridSize;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                deltaX += gridSize;
            }
            if (Input.GetKeyDown(KeyCode.A))
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
        Client.Instance.SendPieceDropped(ActivePiece.GetComponent<Piece>().GetPieceType(), ActivePiece.transform.position.x, ActivePiece.transform.position.y);

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
        
        ActivePiece = ps.GetComponent<PieceSpawner>().GenerateRandomPiece();
        pieceFalling = false;
    }




}
