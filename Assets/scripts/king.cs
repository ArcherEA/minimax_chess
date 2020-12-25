using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class king : chesspiece
{
       public override int newmove(int x,int y,List<Vector3> pos)
    {
        unselected();
        foreach(Vector3 p in pos)
        {
            if(x==p.x&&y==p.z&&p.y==0f)//normal move
            {
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                boardplacex=x;
                boardplacey=y;
                if(createarmy)
                {
                    updatearmyplace();
                }
                moved=true;
                return 1;
            }
            else if(x==p.x&&y==p.z&&p.y==2)//have enemy
            {
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                //Destroy(board.GetComponent<chessboardmanager>().pieces[x,y]);
                target=board.GetComponent<chessboardmanager>().pieces[x,y];
                if(createarmy){
                updatearmytarget(target);}
                board.GetComponent<chessboardmanager>().pieces[x,y].GetComponent<chesspiece>().death=true;
                if(createarmy){
                board.GetComponent<chessboardmanager>().pieces[x,y].GetComponent<chesspiece>().updatearmydeath();}
                if(board.GetComponent<chessboardmanager>().pieces[x,y]==null){Debug.Log("success");}
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                boardplacex=x;
                boardplacey=y;
                if(createarmy)
                {
                    updatearmyplace();
                }
                moved=true;
                return 2;
            }
            else if(x==p.x&&y==p.z&&p.y==5)//castling
            {
                if(x>boardplacex)
                {
                    board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                    board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                    board.GetComponent<chessboardmanager>().pieces[x-1,boardplacey]=board.GetComponent<chessboardmanager>().pieces[7,boardplacey];
                    Debug.Log(board.GetComponent<chessboardmanager>().pieces[x-1,boardplacey]);
                    board.GetComponent<chessboardmanager>().pieces[7,boardplacey]=null;
                    board.GetComponent<chessboardmanager>().pieces[x-1,boardplacey].GetComponent<chesspiece>().boardplacex=x-1;
                    board.GetComponent<chessboardmanager>().pieces[x-1,boardplacey].GetComponent<chesspiece>().boardplacey=boardplacey;
                    boardplacex=x;
                    boardplacey=y; 
                    if(createarmy)
                    {
                        updatearmyplace();
                        board.GetComponent<chessboardmanager>().pieces[x-1,boardplacey].GetComponent<chesspiece>().updatearmyplace();
                    }                  
                    moved=true;

                }
                else
                {
                    board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                    board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                    board.GetComponent<chessboardmanager>().pieces[x+1,boardplacey]=board.GetComponent<chessboardmanager>().pieces[0,boardplacey];
                    board.GetComponent<chessboardmanager>().pieces[0,boardplacey]=null;
                    board.GetComponent<chessboardmanager>().pieces[x+1,boardplacey].GetComponent<chesspiece>().boardplacex=x+1;
                    board.GetComponent<chessboardmanager>().pieces[x+1,boardplacey].GetComponent<chesspiece>().boardplacey=boardplacey;
                    boardplacex=x;
                    boardplacey=y;
                    if(createarmy)
                    {
                        updatearmyplace();
                        board.GetComponent<chessboardmanager>().pieces[x+1,boardplacey].GetComponent<chesspiece>().updatearmyplace();
                    }     
                }
                moved=true;
                return 5;
            }
            else
            {
                continue;
            }
        }
        return 0;
    }
}
