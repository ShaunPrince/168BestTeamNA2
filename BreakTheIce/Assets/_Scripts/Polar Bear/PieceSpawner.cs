using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject[] piecePrefabs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GenerateRandomPiece()
    {
        GameObject temp = piecePrefabs[Random.Range(0, piecePrefabs.Length)];
        temp = GameObject.Instantiate(temp,Vector3.zero,Quaternion.identity);
        return temp;
    }

}
