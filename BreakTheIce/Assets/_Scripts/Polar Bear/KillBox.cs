using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillBox : MonoBehaviour
{
    public GameObject bear;
    public GameObject canvas;

    private GameTimer gameTimer;
    // Start is called before the first frame update
    void Start()
    {
        gameTimer = canvas.GetComponent<GameTimer>();
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
        if (collision.gameObject.name.Equals("Penguin"))
        {
            gameTimer.GameOverScreen();
        }
        Destroy(collision.gameObject);
    }

}
