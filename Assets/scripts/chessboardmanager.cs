using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class chessboardmanager : MonoBehaviour
{
    
    public static chessboardmanager instance{set;get;}
    // public pieceinformation [,] boardpieces;
    public GameObject[,] pieces;
    public bool whiteturn=true;
    private Camera cam;
    public Camera cam1;
    public Camera cam2;
    public Camera cam3;
    public Camera cam4;
    public List<GameObject>  chesspieceprefab;
    public GameObject SelectedArea;
    public GameObject move;
    //judge check and checkmate
    public bool check=false;
    public bool checkmate=false;
    private List<Vector3> newmoveplace;
    // public List<GameObject> activechesspieces;
    private float mousexPos=-10;
    private float mousezPos=-10;
    private GameObject bKing;
    private GameObject wKing;
    private GameObject selectedpiece;
    public bool pvp=false;
    public bool pve=false;
    public bool eve=false;
    private bool moving=false;
    public bool bonus=false;
    public GameObject promotionpiece;
    public AudioSource checkaudio;
    public AudioSource checkmateaudio;
    //private bool finishpromotion=true;
    public int level;
    private void Start()
    {
        cam=cam1;
        //call that function to place the chess pieces at chess board
        spawnall();
        instance=this;
        //test code
        foreach (GameObject go in pieces)
        {
            if(go!=null&&go.GetComponent<chesspiece>().type==6)
            {
                wKing=go;
            }
            if(go!=null&&go.GetComponent<chesspiece>().type==-6)
            {
                bKing=go;
            }

        }
    }
    private void switchcam()
    {   
        if(cam1.gameObject.activeSelf)
        {
            cam=cam1;
        }
        if(cam2.gameObject.activeSelf)
        {
            cam=cam2;
        }
        if(cam3.gameObject.activeSelf)
        {
            cam=cam3;
        }
        if(cam4.gameObject.activeSelf)
        {
            cam=cam4;
        }
    }

    public bool checkpromotion()
    {
        foreach(GameObject go in pieces)
        {
            if(go!=null&&Mathf.Abs(go.GetComponent<chesspiece>().type)==1)
            {
                if(go.GetComponent<chesspiece>().boardplacey==7||go.GetComponent<chesspiece>().boardplacey==0)
                {
                    promotionpiece=go;
                    return true;
                }
            }
        }
        promotionpiece=null;
        return false;
    }
    private void Update()
    {
        switchcam();
        if(pvp)
        {
            if(promotionpiece==null){
                UpdateSelection();
                switchturn(1);  
                }         
        }
        else if (pve)
        {
            if(promotionpiece==null){
                UpdateSelection();
                switchturn(2);           
                if(!whiteturn&&moving==false){
                    AImove(Minimaxroot(getinformation(),true,level));
                    moving=true;
                }
            }
        }
        else if(eve)
        {
            if(whiteturn&&moving==false)
            {
                AImove(Minimaxroot(getinformation(),false,1));
                moving=true;
            }
            switchturn(3);
            if(!whiteturn&&moving==false){

                AImove(Minimaxroot(getinformation(),true,3));
                moving=true;
            }
            
        }
        
    }
    private void switchturn(int t)
    {
        if(moving)
        {           
            if(allpiecesatdestination())
            {
                if(checkpromotion()&&t==1)
                {
                    promotionpiece.GetComponent<pawn>().promotionoptions.SetActive(true);
                }
                else if(checkpromotion()&&(!whiteturn)&&t==2)
                {
                    promotionpiece.GetComponent<pawn>().Promotion(5);
                }
                else if(checkpromotion()&&t==3)
                {
                    promotionpiece.GetComponent<pawn>().Promotion(5);

                }
                else{
                    whiteturn=!whiteturn;
                    setenpassantstatus();
                    moving=false;
                    if(gamestatus()==1)
                    {
                        check=true;
                        checkaudio.Play();
                    }
                    else if(gamestatus()==2)
                    {
                        checkmate=true;
                        checkmateaudio.Play();
                    }
                    else
                    {
                        check=false;
                    }
                }
            }
        } 
    }


    public void setenpassantstatus()
    {
        if(whiteturn)
        {
            foreach(GameObject go in pieces)
            {
                if(go!=null&&go.GetComponent<chesspiece>().type==1)
                {
                    go.GetComponent<chesspiece>().enpassantstatus=false;
                }
            }
        }
        else
        {
            foreach(GameObject go in pieces)
            {
                if(go!=null&&go.GetComponent<chesspiece>().type==-1)
                {
                    go.GetComponent<chesspiece>().enpassantstatus=false;
                }
            }
        }
    }
    //reference: https://www.youtube.com/watch?v=CzImJk7ZesI&t=1s
    private void UpdateSelection() 
    {
        if(!cam)
            return;
        RaycastHit hit;
        //get mouse position on chess board,we only need the mouse position in chessboard
        if(Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition),out hit,1000.0f,LayerMask.GetMask("chessplane")))
        {
            mousexPos=hit.point.x;
            mousezPos=hit.point.z;
           // Debug.Log(mousexPos+","+mousezPos);
            if(hit.point.x>=-8&&hit.point.x<=8&&hit.point.z>=-8&&hit.point.z<=8)
            {
                mousexPos=hit.point.x;
                mousezPos=hit.point.z;
                //Debug.Log(mousexPos+","+mousezPos);
            }
            else
            {
                //put the mouse position out of the chess board
                mousexPos=-10;
                mousezPos=-10;
            }
        }
         else
        {
            //put the mouse position out of the chess board
            mousexPos=-10;
            mousezPos=-10;
        }
        //we want show the grid we selected, we plan create a gameobject when mouse at that place(particle system)
        // if mouse leave it will be destroy and it will move with the mouse;
        Vector3 pos; //create a vector variable to store the chess piece place
        //because our chessboard's size equal to  16*16, and the center of our chess board
        //is (0,1.8,0), so there is four conditions to find the chess piece
        if(mousexPos>-8&&mousexPos<=0&&mousezPos>-8&&mousezPos<=0)
        {
            pos=new Vector3(((int)mousexPos/2)*2-1,1.85f,((int)mousezPos/2)*2-1);
            // Debug.Log(pos);
        }
        else if(mousexPos>-8&&mousexPos<=0&&mousezPos>0&&mousezPos<8)
        {
            pos=new Vector3(((int)mousexPos/2)*2-1,1.85f,((int)mousezPos/2)*2+1);
            // Debug.Log(pos);
        }
        else if(mousexPos>0&&mousexPos<8&&mousezPos>-8&&mousezPos<=0)
        {
            pos=new Vector3(((int)mousexPos/2)*2+1,1.85f,((int)mousezPos/2)*2-1);
            // Debug.Log(pos);
        }
        else if(mousexPos>0&&mousexPos<8&&mousezPos>0&&mousezPos<8)
        {
            pos=new Vector3(((int)mousexPos/2)*2+1,1.85f,((int)mousezPos/2)*2+1);
            // Debug.Log(pos);
        }
        else
        {
            pos=new Vector3(-10,1.9f,-10);
        }
        //use create /delete the effect when player's mouse on some place in chessboard
        GameObject selectedplace= GameObject.FindWithTag("select");
        GameObject[] moveplace= GameObject.FindGameObjectsWithTag("move");
        //find all the chesspiece in chessboard;
        GameObject[] allpieces=GameObject.FindGameObjectsWithTag("chesspiece");
        //when mouse place in chessboard,we update the place of the effect and also 
        //detect the operation of player
        if(pos.x>-8)
        {
            if(selectedplace==null)
            {
                GameObject newselectplace=Instantiate(SelectedArea,pos,Quaternion.identity);
            }
            else
            {   
                if(selectedplace.transform.position==pos)
                {
                    //do not need do anything;
                }
                else
                {
                    selectedplace.transform.position=pos;
                }
            }
            //press left mouse to select chess pieces and move or cancel the move
            if(!moving){
                if(Input.GetMouseButtonDown(0)){
                    //if no chess piece selected, we select a chess piece
                    if(selectedpiece==null)
                    {
                        foreach(GameObject p in allpieces)
                        {
                            //Debug.Log(p);
                            //Debug.Log(pos.x+","+pos.z);
                            if(p!=null&&p.GetComponent<chesspiece>().boardplacex==(pos.x+7)/2&&p.GetComponent<chesspiece>().boardplacey==(pos.z+7)/2)
                            {
                                //Debug.Log((pos.x+7)/2+","+(pos.x+7)/2);
                                selectedpiece=p;
                               // Debug.Log(selectedpiece);
                                selectedpiece.GetComponent<chesspiece>().selected();
                                if(whiteturn)
                                {
                                    //Debug.Log("test");
                                    if(selectedpiece.GetComponent<chesspiece>().type<0)
                                    {
                                        selectedpiece.GetComponent<chesspiece>().unselected();
                                        selectedpiece=null;
                                        //Debug.Log("test1");
                                        break;
                                    }
                                    else{break;}
                                }
                                else
                                {
                                    if(selectedpiece.GetComponent<chesspiece>().type>0)
                                    {
                                        //Debug.Log("test2");
                                        selectedpiece.GetComponent<chesspiece>().unselected();
                                        selectedpiece=null;
                                        break;
                                    }
                                    else{break;}
                                }
                                //need add animation variable control
                                //like anim.setbool("select",true)
                            }
                        }
                        if(selectedpiece!=null)
                        {
                            //create move effect
                            //maybe we can reuse this list when we want move the piece
                            newmoveplace=selectedpiece.GetComponent<chesspiece>().newavaliablemove(getinformation()); //new method
                            //List<Vector3> newmoveplace=selectedpiece.GetComponent<chesspiece>().avaliablemove();//old method
                            foreach(Vector3 place in newmoveplace)
                            {
                                GameObject newplace=Instantiate(move,new Vector3(2*place.x-7,1.9f,2*place.z-7),Quaternion.identity); //new move
                               
                            }
                        }
                        else
                        {
                            newmoveplace=null;
                        }
                    }
                    //there is one chess piece selected, try to move it or cancel the operation
                    //and we also want a function to record the chess piece place in every turn
                    //then we can create a undo function(if we have time)
                    else
                    {
                        //should add animation control
                        //selectedpiece.getcomponent.anim.setbool("selected",false);
                        
                        //move chess piece,will use NavMeshAgent, it will be complex,maybe create another function to do,leave it here now
                        //first remove other pieces near to this piece(setactive,after reach destination, setactive) and destroy destination enemy
                        //and finish rotation
                        //now we create an easy version;

                        //move chess
                        selectedpiece.GetComponent<chesspiece>().unselected();
                        int move_status=selectedpiece.GetComponent<chesspiece>().newmove((int)((pos.x+7)/2),(int)((pos.z+7)/2),newmoveplace);//new
                        if(move_status>0)
                        {   moving=true;
                            // //check promotion
                            // if(selectedpiece.GetComponent<chesspiece>().type==1&&selectedpiece.GetComponent<pawn>().boardplacey==7)
                            // {
                            //     selectedpiece.GetComponent<pawn>().Promotion(5);
                            // }                          
                            
                            
                            // // switchturn();
                            // // setenpassantstatus();
                        }
                        selectedpiece=null;
                        //destroy the move-effect
                        foreach(GameObject g in moveplace)
                        {
                            if (g!=null)
                            {
                                Destroy(g);
                            }
                        }                       
                    }
                }
            }
        }
        else
        {
            if(selectedplace!=null)
            {
                Destroy(selectedplace);
            }
        } 
    }
    
    private bool allpiecesatdestination()
    {
        if(bonus){
            foreach(GameObject go in pieces)
            {
                if(go!=null){
                    if(bonus){
                        if(!go.GetComponent<chesspiece>().updatearmyatplace())
                        {
                            return false;
                        }
                    }
                    if(Mathf.Abs(go.transform.position.x-2*go.GetComponent<chesspiece>().boardplacex+7)<0.1f&&Mathf.Abs(go.transform.position.z-2*go.GetComponent<chesspiece>().boardplacey+7)<0.1f)
                    {
                        
                    }
                    else
                    {
                        return false;   
                    }
                    // GameObject[] go1=go.GetComponent<chesspiece>().army1;
                    // for(int i=0;i<4;i++)
                    // {
                    //     Debug.Log(Mathf.Abs(go1[i].transform.position.x-2*go1[i].GetComponent<army>().boardplacex+7-go1[i].GetComponent<army>().offsetx));
                    //     if(Mathf.Abs(go1[i].transform.position.x-2*go1[i].GetComponent<army>().boardplacex+7-go1[i].GetComponent<army>().offsetx)>0.1f&&Mathf.Abs(go1[i].transform.position.z-2*go1[i].GetComponent<army>().boardplacey+7-go1[i].GetComponent<army>().offsety)>0.1f)
                    //     {return false;}
                    // }    
                }
            }
            return true;
        }
        else{
            foreach(GameObject go in pieces)
            {
                if(go!=null){
                    if(Mathf.Abs(go.transform.position.x-2*go.GetComponent<chesspiece>().boardplacex+7)<0.2f&&Mathf.Abs(go.transform.position.z-2*go.GetComponent<chesspiece>().boardplacey+7)<0.2f)
                    {}
                    else{return false;}
                }
            }
            return true;
        }
    }
    
    public int gamestatus() //1:check 2:checkmate  0:nothing 
    {
        pieceinformation [,] info =getinformation();
        List<Vector3> allmove=new List<Vector3>();
        if(whiteturn&&wKing.GetComponent<chesspiece>().checkstatus(info,1))
        {
            foreach(GameObject cp in pieces)
            {
                if(cp!=null){
                    if(cp.GetComponent<chesspiece>().type>0)
                    {
                        List<Vector3> available=cp.GetComponent<chesspiece>().newavaliablemove(info);
                        if(available.Count>0)
                        {                        
                            foreach(Vector3 nps in available)
                            {                            
                                allmove.Add(nps);                           
                            }
                        }
                    }
                }
            }
            if(allmove.Count>0){return 1;}
            else{return 2;}
        }
        else if (!whiteturn&&bKing.GetComponent<chesspiece>().checkstatus(info,-1))
        {
            foreach(GameObject cp in pieces)
            {
                if(cp!=null){
                    if(cp.GetComponent<chesspiece>().type<0)
                    {
                        List<Vector3> available=cp.GetComponent<chesspiece>().newavaliablemove(info);
                        if(available.Count>0)
                        {                        
                            foreach(Vector3 nps in available)
                            {                            
                                allmove.Add(nps);                           
                            }
                        }
                    }
                }
            }
            if(allmove.Count>0){return 1;}
            else{return 2;}
        }

        return 0;
    }
    private void spawnall()
    {   
        pieces=new GameObject[8,8];
        //spawn white pawns
        for(int i=-7;i<=7;i+=2)
        {
            spawnchesspiece(0,new Vector3(i,2f,-5f),new Vector3(0f,0f,0f));
        }
        // spawn white rooks;
        spawnchesspiece(1,new Vector3(-7f,2f,-7f),new Vector3(0f,0f,0f));
        spawnchesspiece(1,new Vector3(7f,2f,-7f),new Vector3(0f,0f,0f));
        // spawn white horses;
        spawnchesspiece(2,new Vector3(-5f,2f,-7f),new Vector3(0f,0f,0f));
        spawnchesspiece(2,new Vector3(5f,2f,-7f),new Vector3(0f,0f,0f));
        //spawn white bishops
        spawnchesspiece(3,new Vector3(-3f,2f,-7f),new Vector3(0f,0f,0f));
        spawnchesspiece(3,new Vector3(3f,2f,-7f),new Vector3(0f,0f,0f));
        //spawn white Queen
        spawnchesspiece(4,new Vector3(-1f,2f,-7f),new Vector3(0f,0f,0f));
        //spawn white king
        spawnchesspiece(5,new Vector3(1f,2f,-7f),new Vector3(0f,0f,0f));
        //spawn black pawns
        for(int i=-7;i<=7;i+=2)
        {
            spawnchesspiece(6,new Vector3(i,2f,5f),new Vector3(0f,180f,0f));
        }
        // spawn white rooks;
        spawnchesspiece(7,new Vector3(-7f,2f,7f),new Vector3(0f,180f,0f));
        spawnchesspiece(7,new Vector3(7f,2f,7f),new Vector3(0f,180f,0f));
        // spawn white horses;
        spawnchesspiece(8,new Vector3(-5f,2f,7f),new Vector3(0f,180f,0f));
        spawnchesspiece(8,new Vector3(5f,2f,7f),new Vector3(0f,180f,0f));
        //spawn white bishops
        spawnchesspiece(9,new Vector3(-3f,2f,7f),new Vector3(0f,180f,0f));
        spawnchesspiece(9,new Vector3(3f,2f,7f),new Vector3(0f,180f,0f));
        //spawn white Queen
        spawnchesspiece(10,new Vector3(-1f,2f,7f),new Vector3(0f,180f,0f));
        //spawn white king
        spawnchesspiece(11,new Vector3(1f,2f,7f),new Vector3(0f,180f,0f));

    }
    private void spawnchesspiece(int index,Vector3 position,Vector3 rotation)
    {
        // Quaternion rotation = Quaternion.Euler()
        //create chesspiece as the child of chessboard and the place is equal to position
        GameObject piece= Instantiate(chesspieceprefab[index],position, Quaternion.Euler(rotation),transform);
        piece.GetComponent<chesspiece>().updateboardplace();
        pieces[piece.GetComponent<chesspiece>().boardplacex,piece.GetComponent<chesspiece>().boardplacey]=piece;
    }
    public struct AIcontrol
    {
        public Vector3 oldpos;
        public Vector3 newpos;
    }

    private void AImove(AIcontrol aipos)
    {
         for(int i=0;i<=7;i++)
            {
                for(int j=0;j<=7;j++)
                {
                    if(pieces[i,j]!=null&&pieces[i,j].GetComponent<chesspiece>().boardplacex==aipos.oldpos.x&&pieces[i,j].GetComponent<chesspiece>().boardplacey==aipos.oldpos.z)
                    {
                        List<Vector3> temppos=new List<Vector3>();
                        temppos.Add(aipos.newpos);
                        pieces[i,j].GetComponent<chesspiece>().newmove((int)aipos.newpos.x,(int)aipos.newpos.z,temppos);
                    }
                }
            }
    }
    private int calculatevalue(pieceinformation[,] newinfo)
    {
        int totalvalue=0;
        foreach(pieceinformation pi in newinfo)
        {
            switch (pi.type)
            {
                case 1:
                    totalvalue=totalvalue+100;
                    // totalvalue=totalvalue+100+PawnPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 2:
                    totalvalue=totalvalue+500;
                    // totalvalue=totalvalue+500+RookPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 3:
                    totalvalue=totalvalue+300;
                    // totalvalue=totalvalue+300+KnightPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 4:
                    totalvalue=totalvalue+300;
                    // totalvalue=totalvalue+300+BishopPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 5:
                    totalvalue=totalvalue+900;
                    // totalvalue=totalvalue+900+QueenPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 6:
                    totalvalue=totalvalue+9000;
                    // totalvalue=totalvalue+9000+KingPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case -1:
                    totalvalue=totalvalue-100;
                    // totalvalue=totalvalue-100-PawnPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -2:
                    totalvalue=totalvalue-500;
                    // totalvalue=totalvalue-500-RookPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -3:
                    totalvalue=totalvalue-300;
                    // totalvalue=totalvalue-300-KnightPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -4:
                    totalvalue=totalvalue-300;
                    // totalvalue=totalvalue-300-BishopPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -5:
                    totalvalue=totalvalue-900;
                    // totalvalue=totalvalue-900-QueenPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -6:
                    totalvalue=totalvalue-9000;
                    // totalvalue=totalvalue-9000-KingPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case 0:
                    break;
            }
        }
        return totalvalue;
    }
    private int calculatevalue1(pieceinformation[,] newinfo)
    {
        int totalvalue=0;
        foreach(pieceinformation pi in newinfo)
        {
            switch (pi.type)
            {
                case 1:

                    totalvalue=totalvalue+100+PawnPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 2:
                    totalvalue=totalvalue+500+RookPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 3:
                    
                    totalvalue=totalvalue+300+KnightPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 4:
                   
                    totalvalue=totalvalue+300+BishopPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 5:
                  
                    totalvalue=totalvalue+900+QueenPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case 6:
                   
                    totalvalue=totalvalue+9000+KingPosEval[pi.boardplacex][pi.boardplacey];
                    break;
                case -1:
                   
                    totalvalue=totalvalue-100-PawnPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -2:
                    
                    totalvalue=totalvalue-500-RookPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -3:
                    
                    totalvalue=totalvalue-300-KnightPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -4:
                    
                    totalvalue=totalvalue-300-BishopPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -5:
                   
                    totalvalue=totalvalue-900-QueenPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case -6:
                   
                    totalvalue=totalvalue-9000-KingPosEval[pi.boardplacex][7-pi.boardplacey];
                    break;
                case 0:
                    break;
            }
        }
        return totalvalue;
    }
    
    public pieceinformation[,] getinformation()
    {
        pieceinformation [,] allpieces=new pieceinformation [8,8];
        for(int i=0;i<=7;i++)
        {
            for(int j=0;j<=7;j++)
            {
                if(pieces[i,j]!=null)
                {
                    allpieces[i,j].type=pieces[i,j].GetComponent<chesspiece>().type;
                    allpieces[i,j].boardplacex=pieces[i,j].GetComponent<chesspiece>().boardplacex;
                    allpieces[i,j].boardplacey=pieces[i,j].GetComponent<chesspiece>().boardplacey;
                    allpieces[i,j].kingchecked=pieces[i,j].GetComponent<chesspiece>().kingchecked;
                    allpieces[i,j].moved=pieces[i,j].GetComponent<chesspiece>().moved;
                    allpieces[i,j].enpassantstatus=pieces[i,j].GetComponent<chesspiece>().enpassantstatus;
                }
                else
                {
                    // pieceinformation[i,j](0,i,j,false,false,false);
                    allpieces[i,j].type=0;
                    allpieces[i,j].boardplacex=i;
                    allpieces[i,j].boardplacey=j;
                    allpieces[i,j].kingchecked=false;
                    allpieces[i,j].moved=false;
                    allpieces[i,j].enpassantstatus=false;
                }
            }       
        }

        return allpieces;
    }
private AIcontrol Minimaxroot(pieceinformation[,] info,bool team,int depth)
     {
        List<AIcontrol> available= new List<AIcontrol>();
        if(team){
            foreach(pieceinformation cp in info)
            {
                if(cp.type<0)
                {
                    List<Vector3> allmove=newavaliablemove(info,cp.boardplacex,cp.boardplacey);
                    if(allmove.Count>0)
                    {
                        foreach(Vector3 nps in allmove)
                        {
                            AIcontrol newvar=new AIcontrol();
                            newvar.oldpos=new Vector3(cp.boardplacex,0,cp.boardplacey);
                            newvar.newpos=nps;
                            available.Add(newvar);                                                      
                        }
                    }
                }
            }
            int score=999999;
            if(depth==1)
            {   
                int k=Random.Range(0, available.Count);
                return available[k];
            }
            else{
                AIcontrol bestmove=new AIcontrol();
                foreach(AIcontrol ac in available)
                {
                    pieceinformation[,] newinfo=switchinformation((int)ac.oldpos.x,(int)ac.oldpos.z,(int)ac.newpos.x,(int)ac.newpos.z,info);
                    int val=Minimax(newinfo,!team,depth-1,-999999,999999); 
                    
                        if(val <= score) {
                            score = val;
                            bestmove = ac;
                        }                    
                }
                return bestmove;
            }
        }
        else
        {
             foreach(pieceinformation cp in info)
            {
                if(cp.type>0)
                {
                    List<Vector3> allmove=newavaliablemove(info,cp.boardplacex,cp.boardplacey);
                    if(allmove.Count>0)
                    {
                        //newvar.newpos=allmove;
                        foreach(Vector3 nps in allmove)
                        {
                            AIcontrol newvar=new AIcontrol();
                            newvar.oldpos=new Vector3(cp.boardplacex,0,cp.boardplacey);
                            newvar.newpos=nps;
                            available.Add(newvar);                           
                        }
                    }
                }
            }
            int score=-999999;
            AIcontrol bestmove=new AIcontrol();
            foreach(AIcontrol ac in available)
            {
                pieceinformation[,] newinfo=switchinformation((int)ac.oldpos.x,(int)ac.oldpos.z,(int)ac.newpos.x,(int)ac.newpos.z,info);
                int val=Minimax(newinfo,!team,depth-1,-999999,999999); 
                if(depth==1)
                {   int k=Random.Range(0, available.Count);
                    return available[k];
                }else
                {
                    if(val >= score) {
                        score = val;
                        bestmove = ac;}
                }   
            }
            return bestmove;
        }
     }
    private int Minimax(pieceinformation[,] info,bool team,int depth,int alpha, int beta)//team=true for black team=false for white
    {
        if(depth==0)
        {  int val=calculatevalue1(info);
            return val;}
        if(team)//minnode
        {
            List<AIcontrol> available= new List<AIcontrol>();
            foreach(pieceinformation cp in info)
            {
                if(cp.type<0)
                {
                    List<Vector3> allmove=newavaliablemove(info,cp.boardplacex,cp.boardplacey);
                    if(allmove.Count>0)
                    {
                        foreach(Vector3 nps in allmove)
                        {
                            AIcontrol newvar=new AIcontrol();
                            newvar.oldpos=new Vector3(cp.boardplacex,0,cp.boardplacey);
                            newvar.newpos=nps;
                            available.Add(newvar);                           
                        }
                    }
                }
            } 
            int score=999999;
            foreach(AIcontrol ac in available)
            {
                pieceinformation[,] newinfo=switchinformation((int)ac.oldpos.x,(int)ac.oldpos.z,(int)ac.newpos.x,(int)ac.newpos.z,info);
                score=Mathf.Min(Minimax(newinfo,!team,depth-1,alpha,beta),beta);  
                if(score<beta){
                    beta=score;
                }
                if(alpha>=beta){return score;}    
            }   
            return score;
        }
        else//maxnode
        {
            List<AIcontrol> available= new List<AIcontrol>();
            foreach(pieceinformation cp in info)
            {
                if(cp.type>0)
                {
                    List<Vector3> allmove=newavaliablemove(info,cp.boardplacex,cp.boardplacey);
                    if(allmove.Count>0)
                    {
                        foreach(Vector3 nps in allmove)
                        {
                            AIcontrol newvar=new AIcontrol();
                            newvar.oldpos=new Vector3(cp.boardplacex,0,cp.boardplacey);
                            newvar.newpos=nps;
                            available.Add(newvar);                           
                        }
                    }
                }
            } 
            int score=-999999;
            foreach(AIcontrol ac in available)
            {   pieceinformation[,] newinfo=switchinformation((int)ac.oldpos.x,(int)ac.oldpos.z,(int)ac.newpos.x,(int)ac.newpos.z,info);
                score=Mathf.Max(Minimax(newinfo,!team,depth-1,alpha,beta),alpha); 
                if(score>alpha){alpha=score;}
                if(alpha>=beta){return score;}    
            }   
            return score;
        }
    }
    public List<Vector3> newavaliablemove(pieceinformation[,] info,int posx,int posy)
    {
        List<Vector3> pos=new List<Vector3>();
        // pieceinformation[,] info=getinformation();
        if(Mathf.Abs(info[posx,posy].type)==1)//pawn
        {
            Vector3 move;
            //normalmove -no enemy
            move=checkavailablemovebynormalrule(posx,posy,info,0,info[posx,posy].type);
            if(move.x!=-1&&move.y==0)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                // pos.Add(move);
            }
            //special first move
            if(!info[posx,posy].moved)
            {
                move=checkpawnfirstmove(posx,posy,info,info[posx,posy].type);
                if(move.x!=-1)
                {
                    if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                    // pos.Add(move);
                }
            }
            
            move=checkavailablemovebynormalrule(posx,posy,info,1,info[posx,posy].type);
            if(move.x!=-1)
            {
                if(move.y==2)
                {
                    if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                    // pos.Add(move);
                }     
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,info[posx,posy].type);
            if(move.x!=-1)
            {
                if(move.y==2)
                {
                    if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                    // pos.Add(move);
                }
            }
            //enpassant
            move=checkenpassantright(posx,posy,info);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkenpassantleft(posx,posy,info);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type)){pos.Add(move);}
                // pos.Add(move);
            }
        }
        else if(Mathf.Abs(info[posx,posy].type)==2)//rook
        {
            Vector3 move;
            //up
            bool up=true;
            bool down=true;
            bool left=true;
            bool right=true;
            // Debug.Log("test rook function");
            for(int i=1;i<=7;i++)
            {
                if(up)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,0,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/2)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){up=false;}
                    }
                    else{up=false;}
                }
                if(down)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,0,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/2)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){down=false;}
                    }
                    else{down=false;}
                }
                if(left)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/2)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){left=false;}
                    }
                    else{left=false;}
                }
                if(right)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/2)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){right=false;}
                    }
                    else{right=false;}
                }
                if((!up)&&(!down)&&(!left)&&(!right)){break;}
            }
        }
        else if(Mathf.Abs(info[posx,posy].type)==3)//horse
        {
            // Debug.Log("test horse function");
            Vector3 move;
            move=checkavailablemovebynormalrule(posx,posy,info,2,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-2,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-2,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,2,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,1,2);
            if(move.x!=-1)
            {
                //Debug.Log((int)info[posx,posy].type/3);
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,1,-2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,-2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/3)){pos.Add(move);}
                // pos.Add(move);
            }

        }
        else if(Mathf.Abs(info[posx,posy].type)==4)//bishop
        {
            // Debug.Log("test bishop function");
            Vector3 move;
            //up
            bool upright=true;
            bool downleft=true;
            bool upleft=true;
            bool downright=true;
            for(int i=1;i<=7;i++)
            {
                if(upright)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/4)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){upright=false;}
                    }
                    else{upright=false;}
                }
                if(downleft)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/4)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){downleft=false;}
                    }
                    else{downleft=false;}
                }
                if(upleft)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/4)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){upleft=false;}
                    }
                    else{upleft=false;}
                }
                if(downright)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/4)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){downright=false;}
                    }
                    else{downright=false;}
                }
                if((!upright)&&(!downleft)&&(!upleft)&&(!downright)){break;}
            }
        }
        else if(Mathf.Abs(info[posx,posy].type)==5)//queen
        {
            // Debug.Log("test queen function");
            Vector3 move;
            //up
            bool upright=true;
            bool downleft=true;
            bool upleft=true;
            bool downright=true;
            bool up=true;
            bool down=true;
            bool left=true;
            bool right=true;
            for(int i=1;i<=7;i++)
            {
                if(up)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,0,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){up=false;}
                    }
                    else{up=false;}
                }
                if(down)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,0,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){down=false;}
                    }
                    else{down=false;}
                }
                if(left)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){left=false;}
                    }
                    else{left=false;}
                }
                if(right)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){right=false;}
                    }
                    else{right=false;}
                }
                if(upright)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){upright=false;}
                    }
                    else{upright=false;}
                }
                if(downleft)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){downleft=false;}
                    }
                    else{downleft=false;}
                }
                if(upleft)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,-i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){upleft=false;}
                    }
                    else{upleft=false;}
                }
                if(downright)
                {
                    move=checkavailablemovebynormalrule(posx,posy,info,i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/5)){pos.Add(move);}
                        // pos.Add(move);
                        if(move.y==2){downright=false;}
                    }
                    else{downright=false;}
                }
                if((!up)&&(!down)&&(!left)&&(!right)&&(!upright)&&(!downleft)&&(!upleft)&&(!downright)){break;}
            }
            
        }
        else if(Mathf.Abs(info[posx,posy].type)==6)//king
        {
            // Debug.Log("test king function");
            Vector3 move;
            move=checkavailablemovebynormalrule(posx,posy,info,1,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,1,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,1,0);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,-1,0);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,0,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkavailablemovebynormalrule(posx,posy,info,0,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}
                // pos.Add(move);
            }
            move=checkKScastling(posx,posy,info);
            if(move.x!=-1&&posx==4)
            {
                // pos.Add(move);
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}//switchinformation(7,posy,posx+1,posy,switchinformation(posx,posy,(int)move.x,(int)move.z,info))
            }
            move=checkQScastling(posx,posy,info);
            if(move.x!=-1&&posx==4)
            {
                // pos.Add(move);
                if(!checkstatus(switchinformation(posx,posy,(int)move.x,(int)move.z,info),(int)info[posx,posy].type/6)){pos.Add(move);}//switchinformation(0,posy,posx-1,posy,switchinformation(posx,posy,(int)move.x,(int)move.z,info))
            }
        }
        return pos;
    }
     public bool checkstatus(pieceinformation[,] info,int team) //check :true not check:false
    {
        
        foreach(pieceinformation pi in info)
        {
            if(pi.type==team*6)
            {
                bool upright=true;
                bool downleft=true;
                bool upleft=true;
                bool downright=true;
                bool up=true;
                bool down=true;
                bool left=true;
                bool right=true;
               // Debug.Log("king pos:"+pi.boardplacex+","+pi.boardplacey);
                //pawn check+knight check + king check
                if(team>0){
                    if(pi.boardplacex+1<=7&&pi.boardplacey+1<=7&&info[pi.boardplacex+1,pi.boardplacey+1].type==-1*team)
                    {return true;}
                    if(pi.boardplacex-1>=0&&pi.boardplacey+1<=7&&info[pi.boardplacex-1,pi.boardplacey+1].type==-1*team)
                    {return true;}
                }else
                {
                    if(pi.boardplacex+1<=7&&pi.boardplacey-1>=0&&info[pi.boardplacex+1,pi.boardplacey-1].type==-1*team)
                    {return true;}
                    if(pi.boardplacex-1>=0&&pi.boardplacey-1>=0&&info[pi.boardplacex-1,pi.boardplacey-1].type==-1*team)
                    {return true;}
                }
                if(pi.boardplacex+1<=7&&pi.boardplacey+1<=7&&info[pi.boardplacex+1,pi.boardplacey+1].type==-6*team)
                {return true;}
                if(pi.boardplacey+1<=7&&info[pi.boardplacex,pi.boardplacey+1].type==-6*team)
                {return true;}
                if(pi.boardplacex-1>=0&&pi.boardplacey+1<=7&&info[pi.boardplacex-1,pi.boardplacey+1].type==-6*team)
                {return true;}
                if(pi.boardplacex-1>=0&&info[pi.boardplacex-1,pi.boardplacey].type==-6*team)
                {return true;}
                if(pi.boardplacex+1<=7&&info[pi.boardplacex+1,pi.boardplacey].type==-6*team)
                {return true;}
                if(pi.boardplacex-1>=0&&pi.boardplacey-1>=0&&info[pi.boardplacex-1,pi.boardplacey-1].type==-6*team)
                {return true;}
                if(pi.boardplacey-1>=0&&info[pi.boardplacex,pi.boardplacey-1].type==-6*team)
                {return true;}
                if(pi.boardplacex+1<=7&&pi.boardplacey-1>=0&&info[pi.boardplacex+1,pi.boardplacey-1].type==-6*team)
                {return true;}
                if(pi.boardplacex+2<=7&&pi.boardplacey+1<=7&&info[pi.boardplacex+2,pi.boardplacey+1].type==-3*team)
                {return true;}
                if(pi.boardplacex-2>=0&&pi.boardplacey+1<=7&&info[pi.boardplacex-2,pi.boardplacey+1].type==-3*team)
                {return true;}
                if(pi.boardplacex+2<=7&&pi.boardplacey-1>=0&&info[pi.boardplacex+2,pi.boardplacey-1].type==-3*team)
                {return true;}
                if(pi.boardplacex-2>=0&&pi.boardplacey-1>=0&&info[pi.boardplacex-2,pi.boardplacey-1].type==-3*team)
                {return true;}
                if(pi.boardplacex+1<=7&&pi.boardplacey+2<=7&&info[pi.boardplacex+1,pi.boardplacey+2].type==-3*team)
                {return true;}
                if(pi.boardplacex+1<=7&&pi.boardplacey-2>=0&&info[pi.boardplacex+1,pi.boardplacey-2].type==-3*team)
                {return true;}
                if(pi.boardplacex-1>=0&&pi.boardplacey+2<=7&&info[pi.boardplacex-1,pi.boardplacey+2].type==-3*team)
                {return true;}
                if(pi.boardplacex-1>=0&&pi.boardplacey-2>=0&&info[pi.boardplacex-1,pi.boardplacey-2].type==-3*team)
                {return true;}
               

                for(int i=1;i<=7;i++)
                {
                   
                    if(up&&(pi.boardplacey+i<=7))
                    {
                       if(info[pi.boardplacex,pi.boardplacey+i].type==5*(-team)||info[pi.boardplacex,pi.boardplacey+i].type==2*(-team)) 
                       {return true;}
                       else if(info[pi.boardplacex,pi.boardplacey+i].type!=0)
                       {up=false;}    
                    }
                    else{up=false;}
                    
                    if(down&&(pi.boardplacey-i>=0&&down))
                    {
                        if(info[pi.boardplacex,pi.boardplacey-i].type==5*(-team)||info[pi.boardplacex,pi.boardplacey-i].type==2*(-team)) 
                       {return true;}
                       else if(info[pi.boardplacex,pi.boardplacey-i].type!=0)
                       {down=false;}
                    }else{down=false;}
                   
                    if(right&&(pi.boardplacex+i<=7))
                    {
                        if(info[pi.boardplacex+i,pi.boardplacey].type==5*(-team)||info[pi.boardplacex+i,pi.boardplacey].type==2*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex+i,pi.boardplacey].type!=0)
                        {right=false;}
                    }else{right=false;}
                    if(left&&(pi.boardplacex-i>=0))
                    {
                        if(info[pi.boardplacex-i,pi.boardplacey].type==5*(-team)||info[pi.boardplacex-i,pi.boardplacey].type==2*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex-i,pi.boardplacey].type!=0)
                        {left=false;}
                    }else{left=false;}
                    if(downleft&&(pi.boardplacey-i>=0)&&(pi.boardplacex-i>=0))
                    {
                        
                        if(info[pi.boardplacex-i,pi.boardplacey-i].type==5*(-team)||info[pi.boardplacex-i,pi.boardplacey-i].type==4*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex-i,pi.boardplacey-i].type!=0)
                        {downleft=false;}
                    }else{downleft=false;}
                   // Debug.Log("value="+pi.boardplacex+","+pi.boardplacey+","+i);
                    if(upleft&&(pi.boardplacey+i<=7)&&(pi.boardplacex-i>=0))
                    {
                        
                        if(info[pi.boardplacex-i,pi.boardplacey+i].type==5*(-team)||info[pi.boardplacex-i,pi.boardplacey+i].type==4*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex-i,pi.boardplacey+i].type!=0)
                        {upleft=false;}
                    }else{upleft=false;}
                    if(upright&&(pi.boardplacey+i<=7)&&(pi.boardplacex+i<=7))
                    {
                        
                        if(info[pi.boardplacex+i,pi.boardplacey+i].type==5*(-team)||info[pi.boardplacex+i,pi.boardplacey+i].type==4*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex+i,pi.boardplacey+i].type!=0)
                        {upright=false;}
                    }else{upright=false;}
                    if(downright&&(pi.boardplacey-i>=0)&&(pi.boardplacex+i<=7))
                    {
                        
                        if(info[pi.boardplacex+i,pi.boardplacey-i].type==5*(-team)||info[pi.boardplacex+i,pi.boardplacey-i].type==4*(-team)) 
                        {return true;}
                        else if(info[pi.boardplacex+i,pi.boardplacey-i].type!=0)
                        {downright=false;}
                    }else{downright=false;}
                    if((!up)&&(!down)&&(!left)&&(!right)&&(!upright)&&(!downleft)&&(!upleft)&&(!downright)){break;}
                }
            }
        }
        return false;
    }

    public pieceinformation[,] switchinformation(int oldx,int oldy,int newx,int newy,pieceinformation[,] oldinfo)//may be add int type for pawn promotion
    {
        pieceinformation [,] allpieces=new pieceinformation [8,8];
        for(int i=0;i<=7;i++)
        {
            for(int j=0;j<=7;j++)
            {
                
                allpieces[i,j].type=oldinfo[i,j].type;
                allpieces[i,j].boardplacex=oldinfo[i,j].boardplacex;
                allpieces[i,j].boardplacey=oldinfo[i,j].boardplacey;
                allpieces[i,j].kingchecked=oldinfo[i,j].kingchecked;
                allpieces[i,j].moved=oldinfo[i,j].moved;
                allpieces[i,j].enpassantstatus=oldinfo[i,j].enpassantstatus;
                
            }       
        }
        allpieces[newx,newy].type=allpieces[oldx,oldy].type;
        allpieces[newx,newy].boardplacex=newx;
        allpieces[newx,newy].boardplacey=newy;
        allpieces[newx,newy].kingchecked=allpieces[oldx,oldy].kingchecked;
        allpieces[newx,newy].moved=true;
        allpieces[oldx,oldy].type=0;
        allpieces[oldx,oldy].boardplacex=oldx;
        allpieces[oldx,oldy].boardplacey=oldy;
        allpieces[oldx,oldy].kingchecked=false;
        allpieces[oldx,oldy].moved=false;
        allpieces[oldx,oldy].enpassantstatus=false;
        for(int i=0;i<=7;i++)
        {
            for (int j=0;j<=7;j++)
            {
                if(allpieces[i,j].type*allpieces[newx,newy].type>0)
                {
                    allpieces[i,j].enpassantstatus=false;
                } 
            }
        }
        if(Mathf.Abs(newy-oldy)==2&&Mathf.Abs(allpieces[newx,newy].type)==1)
        {allpieces[newx,newy].enpassantstatus=true;}
        else{allpieces[newx,newy].enpassantstatus=false;}
        if(Mathf.Abs(allpieces[oldx,oldy].type)==1&&allpieces[newx,oldy].enpassantstatus)
        {
            allpieces[newx,oldy].type=0;
        }
        if(Mathf.Abs(allpieces[newx,newy].type)==6&&newy==oldy&&Mathf.Abs(newx-oldx)==2)//castling
        {   
            if(newx>oldx)
            {
                allpieces[newx-1,newy].type=allpieces[7,newy].type;
                allpieces[newx-1,newy].moved=true;
                allpieces[newx-1,newy].boardplacex=newx-1;
                allpieces[newx-1,newy].boardplacey=newy;
                allpieces[7,newy].type=0;
            }
            else
            {
                allpieces[newx+1,newy].type=allpieces[0,newy].type;
                allpieces[newx+1,newy].moved=true;
                allpieces[newx+1,newy].boardplacex=newx+1;
                allpieces[newx+1,newy].boardplacey=newy;
                allpieces[0,newy].type=0;
            }
        }
        //promotion condition
        if(Mathf.Abs(oldinfo[oldx,oldy].type)==1&&(newy==7||newy==0))
        {
            allpieces[newx,newy].type=5*oldinfo[oldx,oldy].type;
        }
        return allpieces;
    }
     public Vector3 checkavailablemovebynormalrule(int boardx,int boardy,pieceinformation[,] info,int dx,int dy)//can remove chesstype?
    {
        if(boardx+dx>=0&&boardx+dx<=7&&boardy+dy>=0&&boardy+dy<=7)
        {
            if(info[boardx+dx,boardy+dy].type==0)
            {
                return new Vector3(boardx+dx,0f,boardy+dy);
            }
            else if(info[boardx+dx,boardy+dy].type<0)
            {
                if(info[boardx,boardy].type>0)
                {
                    return new Vector3(boardx+dx,2,boardy+dy);
                }
                else{return new Vector3(-1,-1,-1);}
            }
            else
            {
                if(info[boardx,boardy].type<0)
                {
                    return new Vector3(boardx+dx,2,boardy+dy);
                }
                else{return new Vector3(-1,-1,-1);}
            }
        }
        else
        {
            return new Vector3(-1,-1,-1);
        }
    }

    public Vector3 checkpawnfirstmove(int boardx,int boardy,pieceinformation[,] info,int chesstype)
    {
        if(info[boardx,boardy+chesstype].type!=0)
        {
            return new Vector3(-1,-1,-1);
        }
        else
        {
            if(info[boardx,boardy+2*chesstype].type!=0)
            {
                return new Vector3(-1,-1,-1);
            }
            else
            {
                return new Vector3(boardx,3,boardy+2*chesstype);
            }
        }
    }
    public Vector3 checkenpassantright(int boardx,int boardy,pieceinformation[,] info)
    {
        if(boardx+1<=7)
        {
            if(info[boardx+1,boardy].type+info[boardx,boardy].type==0&&info[boardx+1,boardy].enpassantstatus)
            {
                if(info[boardx,boardy].type==1)
                {
                    return new Vector3(boardx+1,1,boardy+1);
                }
                else{return new Vector3(boardx+1,1,boardy-1);}
            }
            else{return new Vector3(-1,-1,-1);}
        }
        else{return new Vector3(-1,-1,-1);}
    }
    public Vector3 checkenpassantleft(int boardx,int boardy,pieceinformation[,] info)
    {
        if(boardx-1>=0)
        {
            if(info[boardx-1,boardy].type+info[boardx,boardy].type==0&&info[boardx-1,boardy].enpassantstatus)
            {
                if(info[boardx,boardy].type==1)
                {
                    return new Vector3(boardx-1,1,boardy+1);
                }
                else{return new Vector3(boardx-1,1,boardy-1);}
            }
            else{return new Vector3(-1,-1,-1);}
        }
        else{return new Vector3(-1,-1,-1);}
    }

    public Vector3 checkKScastling(int boardx,int boardy,pieceinformation[,] info)
    {
        if(!check)
        {
            if(!info[boardx,boardy].moved)
            {                  
                if(info[7,boardy].type!=0&&info[7,boardy].moved==false)
                {
                    bool nopieces_r_k=false;
                    for(int i=5;i<=6;i++)
                    {
                        if(info[i,boardy].type==0)
                        {
                            nopieces_r_k=true;
                        }
                        else
                        {
                            nopieces_r_k=false;
                            break;
                        }
                    }
                    if(nopieces_r_k)
                    {
                        return new Vector3(boardx+2,5,boardy);
                    }
                    else{return new Vector3(-1,-1,-1);}
                }
                else{return new Vector3(-1,-1,-1);}
            }
            else{return new Vector3(-1,-1,-1);}
        }
        else{return new Vector3(-1,-1,-1);}
    }

    public Vector3 checkQScastling(int boardx,int boardy,pieceinformation[,] info)
    {
        if(!check)
        {
            if(!info[boardx,boardy].moved)
            {                  
                if(info[0,boardy].type!=0&&info[0,boardy].moved==false)
                {
                    bool nopieces_r_k=false;
                    for(int i=1;i<=3;i++)
                    {
                        if(info[i,boardy].type==0)
                        {
                            nopieces_r_k=true;
                        }
                        else
                        {
                            nopieces_r_k=false;
                            break;
                        }
                    }
                    if(nopieces_r_k)
                    {
                        return new Vector3(boardx-2,5,boardy);
                    }
                    else{return new Vector3(-1,-1,-1);}
                }
                else{return new Vector3(-1,-1,-1);}
            }
            else{return new Vector3(-1,-1,-1);}
        }
        else{return new Vector3(-1,-1,-1);}
    }

    //reference:https://www.freecodecamp.org/news/simple-chess-ai-step-by-step-1d55a9266977/
    //we multiply 10 to keep int type
    private static int [][] PawnPosEval=new int [8][]
    {
        new int[8] {0, 0, 0, 0, 0, 0, 0, 0},
        new int[8] {50, 50, 50, 50, 50, 50, 50, 50},
        new int[8] {10, 10, 20, 30, 30, 20, 10, 10},
        new int[8] {5,   5, 10, 25, 25, 10,  5,  5},
        new int[8] {0,   0,  0, 20, 20,  0,  0,  0},
        new int[8] {5,  -5,-10,  0,  0,-10, -5,  5},
        new int[8] {5,  10, 10,-20,-20, 10, 10, 5},
        new int[8] {0,   0,  0,  0,   0,  0, 0, 0}
    };
    private static int[][] RookPosEval = new int[8][]
    {
        new int[8] {0, 0, 0, 0, 0, 0, 0, 0},
        new int[8] {5, 10, 10, 10, 10, 10, 10, 5},
        new int[8] {-5, 0, 0, 0, 0, 0, 0, -5},
        new int[8] {-5, 0, 0, 0, 0, 0, 0, -5},
        new int[8] {-5, 0, 0, 0, 0, 0, 0, -5},
        new int[8] {-5, 0, 0, 0, 0, 0, 0, -5},
        new int[8] {-5, 0, 0, 0, 0, 0, 0, -5},
        new int[8] {0, 0, 0, 5, 5, 0, 0, 0}
    };
    private static int[][] KnightPosEval = new int[8][]
    {
        new int[8] {-50, -40, -30, -30, -30, -30, -40, -50},
        new int[8] {-40, -20, 0, 0, 0, 0, -20, -40},
        new int[8] {-30, 0, 10, 15, 15, 10, 0, -30},
        new int[8] {-30, 5, 15, 20, 20, 15, 5, -30},
        new int[8] {-30, 0, 15, 20, 20, 15, 0, -30},
        new int[8] {-30, 5, 10, 15, 15, 10, 5, -30},
        new int[8] {-40, -20, 0, 5, 5, 0, -20, -40},
        new int[8] {-50, -40, -30, -30, -30, -30, -40, -50}
    };
    private static int[][] BishopPosEval = new int[8][]
    {
        new int[8] {-20, -10, -10, -10, -10, -10, -10, -20},
        new int[8] {-10, 0, 0, 0, 0, 0, 0, -10},
        new int[8] {-10, 0, 5, 10, 10, 5, 0, -10},
        new int[8] {-10, 5, 5, 10, 10, 5, 5, -10},
        new int[8] {-10, 0, 10, 10, 10, 10, 0, -10},
        new int[8] {-10, 10, 10, 10, 10, 10, 10, -10},
        new int[8] {-10, 5, 0, 0, 0, 0, 5, -10},
        new int[8] {-20, -10, -10, -10, -10, -10, -10, -20}
    };
    private static int[][] KingPosEval = new int[8][]
    {
        new int[8] {-30, -40, -40, -50, -50, -40, -40, -30},
        new int[8] {-30, -40, -40, -50, -50, -40, -40, -30},
        new int[8] {-30, -40, -40, -50, -50, -40, -40, -30},
        new int[8] {-30, -40, -40, -50, -50, -40, -40, -30},
        new int[8] {-20, -30, -30, -40, -40, -30, -30, -20},
        new int[8] {-10, -20, -20, -20, -20, -20, -20, -10},
        new int[8] {20, 20, 0, 0, 0, 0, 20, 20},
        new int[8] {20, 30, 10, 0, 0, 10, 30, 20}
    };
    private static int[][] QueenPosEval = new int[8][]
    {
        new int[8] {-20, -10, -10, -5, -5, -10, -10, -20},
        new int[8] {-10, 0, 0, 0, 0, 0, 0, -10},
        new int[8] {-10, 0, 5, 5, 5, 5, 0, -10},
        new int[8] {-5, 0, 5, 5, 5, 5, 0, -5},
        new int[8] {0, 0, 5, 5, 5, 5, 0, -5},
        new int[8] {-10, 5, 5, 5, 5, 5, 0, -10},
        new int[8] {-10, 0, 5, 0, 0, 0, 0, -10},
        new int[8] {-20, -10, -10, -5, -5, -10, -10, -20}
    };
}
