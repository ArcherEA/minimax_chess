using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameendscript : MonoBehaviour
{

    public GameObject panel;
    public GameObject button;
    public GameObject whitewin;
    public GameObject blackwin;
    public GameObject board;
    public GameObject normal;
    public GameObject bonus;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;
    public void Update()
    {
        if(board==null)
        {
            if(normal.activeSelf)
            {
                board=normal;
            }
            else if(bonus.activeSelf)
            {
                board=bonus;
            }
        }
        endgame();
    }
    public void view1()
    {
        cam1.gameObject.SetActive(true);
        cam2.gameObject.SetActive(false);
        cam3.gameObject.SetActive(false);
        cam4.gameObject.SetActive(false);
    }
    public void view2()
    {
        cam1.gameObject.SetActive(false);
        cam2.gameObject.SetActive(true);
        cam3.gameObject.SetActive(false);
        cam4.gameObject.SetActive(false);
    }
    public void view3()
    {
        cam1.gameObject.SetActive(false);
        cam2.gameObject.SetActive(false);
        cam3.gameObject.SetActive(true);
        cam4.gameObject.SetActive(false);
    }
    public void view4()
    {
        cam1.gameObject.SetActive(false);
        cam2.gameObject.SetActive(false);
        cam3.gameObject.SetActive(false);
        cam4.gameObject.SetActive(true);
    }
    public void endgame()
    {
        if(board.GetComponent<chessboardmanager>().checkmate)
        {
            board.GetComponent<chessboardmanager>().pvp=false;
            board.GetComponent<chessboardmanager>().pve=false;
            board.GetComponent<chessboardmanager>().eve=false;
            if(board.GetComponent<chessboardmanager>().whiteturn)
            {
                //panel.SetActive(true);
                button.SetActive(true);
                blackwin.SetActive(true);
            }
            else
            {
                //panel.SetActive(true);
                button.SetActive(true);
                whitewin.SetActive(true);
            }
        }
    }
    public void mainmenu()
    {
        Debug.Log("callmenu");
        SceneManager.LoadScene(0);//SceneManager.GetActiveScene().buildIndex-1
    }

}
