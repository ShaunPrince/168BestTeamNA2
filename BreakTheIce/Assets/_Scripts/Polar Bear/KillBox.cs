using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillBox : MonoBehaviour
{
    public GameObject bear;
    public GameObject canvas;
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
        if (collision.gameObject.name.Equals("Penguin"))
        {
            canvas.transform.GetChild(0).gameObject.SetActive(true);
            GameObject timer1 = canvas.transform.GetChild(1).gameObject;
            GameObject timer2 = canvas.transform.GetChild(2).gameObject;
            timer2.GetComponent<Text>().text = timer1.GetComponent<Text>().text;
            timer1.SetActive(false);
            timer2.SetActive(true);
        }
        Destroy(collision.gameObject);
    }

}
