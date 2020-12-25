using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class pawn : chesspiece
{
    // public bool moved=false;
    // public bool enpassantstatus=false;//it should update every turn?if white it will update in next white turn
    // public GameObject board;

    // private void Start()
    // {
    //     board=GameObject.FindWithTag("board");
    // }
    public GameObject pawnobj;
    public GameObject rook;
    public GameObject horse;
    public GameObject bishop;
    public GameObject queen;
    public GameObject promotionoptions;
    public override void Start()
    {
        if(createarmy)
        {
            spawnarmy();
        }
        board=GameObject.FindWithTag("board");
        foreach(GameObject go in board.GetComponent<chessboardmanager>().pieces)
        {
            if(go!=null){
                if(go.GetComponent<chesspiece>().type==6)
                {
                    wKing=go;
                }
                if(go.GetComponent<chesspiece>().type==-6)
                {
                    bKing=go;
                }
            }
        }
        if(pawnobj.activeSelf){anim=pawnobj.GetComponent<Animator>();}
        else if(rook.activeSelf){anim=rook.GetComponent<Animator>();}
        else if(horse.activeSelf){anim=horse.GetComponent<Animator>();}
        else if(bishop.activeSelf){anim=bishop.GetComponent<Animator>();}
        else if(queen.activeSelf){anim=queen.GetComponent<Animator>();}
    }
    
    public override int newmove(int x,int y,List<Vector3> pos) //0 represent failure to move 1:success move with out kill enemy 2:kill enemy at destination place 3:enpassant kill 4:specialfirst move 5:castling
    {
        
        //List<Vector3> pos=newavaliablemove();
        //need set moved as true after first move

        //need check enpassant at somewhere,maybe in chessboardmanager
        
        foreach(Vector3 p in pos)
        {
            if(x==p.x&&y==p.z&&p.y==0f)//normal move
            {
                if(moved==false)
                {
                    moved=true;
                }
                //
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
            else if(x==p.x&&y==p.z&&p.y==1)//enpassant
            {
                if(moved==false)
                {
                    moved=true;
                }
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                //first set boardplace
                //destroy enemy piece
                target=board.GetComponent<chessboardmanager>().pieces[x,boardplacey];
                if(createarmy)
                {updatearmytarget(target);}
                board.GetComponent<chessboardmanager>().pieces[x,boardplacey].GetComponent<chesspiece>().death=true;
                if(createarmy)
                {board.GetComponent<chessboardmanager>().pieces[x,boardplacey].GetComponent<chesspiece>().updatearmydeath();}
                //Destroy(board.GetComponent<chessboardmanager>().pieces[x,boardplacey]);
                boardplacex=x;
                boardplacey=y;
                if(createarmy)
                {
                    updatearmyplace();
                }
                moved=true;
                return 3;
            }
            else if(x==p.x&&y==p.z&&p.y==2)//have enemy
            {
                if(moved==false)
                {
                    moved=true;
                }
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                //Destroy(board.GetComponent<chessboardmanager>().pieces[x,y]);
                target=board.GetComponent<chessboardmanager>().pieces[x,y];
                if(createarmy)
                {updatearmytarget(target);}
                board.GetComponent<chessboardmanager>().pieces[x,y].GetComponent<chesspiece>().death=true;
                if(createarmy)
                {board.GetComponent<chessboardmanager>().pieces[x,y].GetComponent<chesspiece>().updatearmydeath();}
                if(board.GetComponent<chessboardmanager>().pieces[x,y]==null){Debug.Log("success");}
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                //first set boardplace
                //destroy enemy piece
                
                boardplacex=x;
                boardplacey=y;
                if(createarmy)
                {
                    updatearmyplace();
                }
                moved=true;
                return 2;
            }
            else if(x==p.x&&y==p.z&&p.y==3)//special first move
            {   
                if(moved==false)
                {
                    moved=true;
                }
                //
                board.GetComponent<chessboardmanager>().pieces[boardplacex,boardplacey]=null;
                board.GetComponent<chessboardmanager>().pieces[x,y]=this.gameObject;
                //first set boardplace
                boardplacex=x;
                boardplacey=y;
                if(createarmy)
                {
                    updatearmyplace();
                }
                //transform.position=new Vector3((int)2*(boardplacex)-7,1.8f,(int)2*(boardplacey)-7);
                this.gameObject.GetComponent<chesspiece>().enpassantstatus=true;
                moved=true;

                return 4;
            }
            else
            {
                continue;
            }
        }
        return 0;

    }
    public void Promotion(int t)
    {
        if(this.type>0){
            this.type=t;
        }
        else{this.type=-t;}
        pawnobj.SetActive(false);
        if(Mathf.Abs(t)==2)
        {rook.SetActive(true);
        anim=rook.GetComponent<Animator>();
        }
        else if(Mathf.Abs(t)==3)
        {horse.SetActive(true);
        anim=horse.GetComponent<Animator>();}
        else if(Mathf.Abs(t)==4)
        {bishop.SetActive(true);
        anim=bishop.GetComponent<Animator>();}
        else if(Mathf.Abs(t)==5)
        {queen.SetActive(true);
        anim=queen.GetComponent<Animator>();} 

        // pieceinformation[,] info=getinformation();
        // List<Vector3> pos=newavaliablemove(info);
 
    }
}
