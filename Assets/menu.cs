using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menu : MonoBehaviour
{
    public GameObject board;
    public GameObject normal;
    public GameObject bonus;
    public GameObject mainmenu;
    public GameObject panel;
    public GameObject choice;
    public GameObject canvas1;
    public GameObject canvas2;
    public GameObject menucam;
    public GameObject cam1;
    public GameObject examples;
    public void choosebonus()
    {
        bonus.SetActive(true);
        normal.SetActive(false);
        mainmenu.SetActive(true);
        choice.SetActive(false);
        examples.SetActive(false);
        board=bonus;
    }
    public void exitgame()
    {
        Application.Quit();
    }
    public void choosenormal()
    {
        normal.SetActive(true);
        bonus.SetActive(false);
        mainmenu.SetActive(true);
        choice.SetActive(false);
        examples.SetActive(false);
        board=normal;
    }
    public void pvp()
    {
        board.GetComponent<chessboardmanager>().pvp=true;
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        menucam.SetActive(false);
        cam1.SetActive(true);
    }
    public void pve()
    {
        panel.SetActive(true);
        mainmenu.SetActive(false);

        // canvas1.SetActive(true);
        // canvas2.SetActive(false);
    }
    public void eve()
    {
        board.GetComponent<chessboardmanager>().pvp=false;
        board.GetComponent<chessboardmanager>().pve=false;
        board.GetComponent<chessboardmanager>().eve=true;
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        menucam.SetActive(false);
        cam1.SetActive(true);
    }
    public void lv1()
    {
        board.GetComponent<chessboardmanager>().level=1;
        board.GetComponent<chessboardmanager>().pvp=false;
        board.GetComponent<chessboardmanager>().pve=true;
        board.GetComponent<chessboardmanager>().eve=false;
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        menucam.SetActive(false);
        cam1.SetActive(true);
    }
    public void lv2()
    {
        board.GetComponent<chessboardmanager>().level=2;
        board.GetComponent<chessboardmanager>().pvp=false;
        board.GetComponent<chessboardmanager>().pve=true;
        board.GetComponent<chessboardmanager>().eve=false;
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        menucam.SetActive(false);
        cam1.SetActive(true);
    }
    public void lv3()
    {
        board.GetComponent<chessboardmanager>().level=3;
        board.GetComponent<chessboardmanager>().pvp=false;
        board.GetComponent<chessboardmanager>().pve=true;
        board.GetComponent<chessboardmanager>().eve=false;
        canvas1.SetActive(false);
        canvas2.SetActive(true);
        menucam.SetActive(false);
        cam1.SetActive(true);
    }
    public void returnbutton()
    {
        panel.SetActive(false);
        mainmenu.SetActive(true);
    }
}
