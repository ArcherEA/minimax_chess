using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rook : chesspiece
{
    


     public override int newmove(int x,int y,List<Vector3> pos)//0 represent failure to move 1:success move with out kill enemy 2:kill enemy at destination place 3:enpassant kill
    {
        foreach(Vector3 p in pos)
        {
            if(x==p.x&&y==p.z&&p.y==0f)//normal move
            {
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                //first set boardplace
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
                target= board.GetComponent<chessboardmanager>().pieces[x,y];
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
            else
            {
                continue;
            }
        }
        return 0;
    }
    
}
