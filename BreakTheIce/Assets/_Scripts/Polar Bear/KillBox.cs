using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    public GameObject bear;
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

        if (bear.GetComponent<BearController>().ActivePiece == collision.gameObject)
        {
            bear.GetComponent<BearController>().SpawnPiece();
        }
        Destroy(collision.gameObject);
    }

}
