using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public GameObject ActivePiece;

    public float gridSize;

    private float deltaZ;
    private float deltaX;

    private void Awake()
    {
        deltaZ = 0;
        deltaX = 0;

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ActivePiece != null)
        {
            CheckInput();
            MoveActivePiece();
        }

        ResetInput();
    }


    private void CheckInput()
    {
        if(Input.GetKeyDown(KeyCode.W))
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

    public void MoveActivePiece()
    {
        //possibly need to add buffer time to prevent too fast movement
        //also need to check bounds
        
        ActivePiece.GetComponent<Rigidbody>().MovePosition(ActivePiece.transform.position + new Vector3(deltaX, 0, deltaZ));
    }

    public void ResetInput()
    {
        deltaZ = 0f;
        deltaX = 0f;
    }

}
