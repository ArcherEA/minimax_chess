using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class promotionmanager : MonoBehaviour
{
    public GameObject board;
    public GameObject pawnobject;
    // Start is called before the first frame update
    void Start()
    {
        board=GameObject.FindWithTag("board");
    }

    public void promotionqueen()
    {
        pawnobject.GetComponent<pawn>().Promotion(5);
        this.gameObject.SetActive(false);
        board.GetComponent<chessboardmanager>().promotionpiece=null;
    }
    public void promotionknight()
    {
        pawnobject.GetComponent<pawn>().Promotion(3);
        this.gameObject.SetActive(false);
        board.GetComponent<chessboardmanager>().promotionpiece=null;
    }
    public void promotionrook()
    {
        pawnobject.GetComponent<pawn>().Promotion(2);
        this.gameObject.SetActive(false);
        board.GetComponent<chessboardmanager>().promotionpiece=null;
    }
    public void promotionbishop()
    {
        pawnobject.GetComponent<pawn>().Promotion(4);
        this.gameObject.SetActive(false);
        board.GetComponent<chessboardmanager>().promotionpiece=null;
    }
    
}
