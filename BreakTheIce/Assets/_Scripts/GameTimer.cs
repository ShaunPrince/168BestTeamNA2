using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    private Text gameOverScreen;
    private Text timer;
    private float time;
    private bool gameRunning;
    // Start is called before the first frame update
    void Start()
    {
        gameOverScreen = this.transform.GetChild(0).GetComponent<Text>();
        timer = this.transform.GetChild(1).GetComponent<Text>();
        time = 0.0f;
        timer.text = "0:0.0";
        gameRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            time += Time.deltaTime;
            if (time % 60.0f < 10.0f)
            {
                timer.text = ((int)(time / 60.0f)).ToString() + ":0" + (time % 60.0f).ToString("0.00");
            }
            else
            {
                timer.text = ((int)(time / 60.0f)).ToString() + ":" + (time % 60.0f).ToString("0.00");
            }
        }
    }

    public void GameOverScreen()
    {
        gameRunning = false;
        gameOverScreen.text = "Game Over!\nPenguin Survived: " + timer.text;
        this.transform.GetChild(0).gameObject.SetActive(true);
    }
}
