using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolarBearUI : MonoBehaviour
{
    public GameObject bear;
    public Sprite[] piecePics;
    public GameObject nextPieceUI;

    public PieceType.PType currentShowing;
    public PieceType.PType currentNext;

    // Start is called before the first frame update
    void Start()
    {
        currentNext = bear.GetComponent<BearController>().nextPiece.GetComponent<Piece>().pieceType;
        currentShowing = bear.GetComponent<BearController>().nextPiece.GetComponent<Piece>().pieceType;
        nextPieceUI.GetComponent<Image>().sprite = piecePics[(int)currentNext];
    }

    // Update is called once per frame
    void Update()
    {
        currentNext = bear.GetComponent<BearController>().nextPiece.GetComponent<Piece>().pieceType;
        if (currentShowing != currentNext)
        {
            nextPieceUI.GetComponent<Image>().sprite = piecePics[(int)currentNext];
            currentShowing = currentNext;
        }
    }
}
