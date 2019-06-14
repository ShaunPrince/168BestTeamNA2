using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassedIPvalue : MonoBehaviour
{
    public static PassedIPvalue Instance { set; get; }
    public string Server_IP { set; get; }

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Instance = this;
    }
}
