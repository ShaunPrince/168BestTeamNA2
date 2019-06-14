using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyScene : MonoBehaviour
{

    public void LoadGame()
    {
        PassedIPvalue.Instance.Server_IP = GameObject.FindWithTag("ServerIP").GetComponent<TMP_InputField>().text;
        SceneManager.LoadScene(1);
    }
}
