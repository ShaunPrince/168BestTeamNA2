using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public GameObject bear;

    private void Awake()
    {
        bear = GameObject.FindGameObjectWithTag("Bear");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Floor"))
        {
            Debug.Log(collision.gameObject);
            Destroy(collision.gameObject);
            if(bear.GetComponent<BearController>().ActivePiece == this.gameObject)
            {
                bear.GetComponent<BearController>().SpawnPiece();
            }

            Destroy(this.gameObject);
        }
    }

}
