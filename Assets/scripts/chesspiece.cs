using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public struct pieceinformation
{
    public int boardplacex;
    public int boardplacey;
    public bool kingchecked;
    public bool moved;
    public bool enpassantstatus;
    public int type;
    
    public pieceinformation(int type,int boardplacex,int boardplacey,bool kingchecked=false,bool moved=false,bool enpassantstatus=false)
    {
        this.type=type;
        this.boardplacex=boardplacex;
        this.boardplacey=boardplacey;
        this.kingchecked=kingchecked;
        this.moved=moved;
        this.enpassantstatus=enpassantstatus;
    }
}   


public abstract class chesspiece : MonoBehaviour
{
    public int boardplacex;
    public int boardplacey;
    public bool kingchecked=false;
    //public bool kcheck=false;
    public bool moved=false;
    public bool enpassantstatus=false;
    //public bool iswhite;
    public int type;
    public GameObject board;
    public GameObject target=null;
    public Animator anim;
    public float lifetime=0;
    public bool death=false;
    public int health=5;
    public GameObject bKing;
    public GameObject wKing;
    public AudioSource footstep;
    public AudioSource atksound;
    public AudioSource deathsound;
    public GameObject armyprefab;
    public bool createarmy=false;
    public GameObject[] army1=new GameObject[4];
    public List<Rigidbody> rb;
    public float distance=2f;
    public void spawnarmy()
    {
        Vector3[] pos=new Vector3 [4];
        pos[0]=new Vector3(0.5f,0f,0.5f);
        pos[1]=new Vector3(-0.5f,0f,-0.5f);
        pos[2]=new Vector3(0.5f,0f,-0.5f);
        pos[3]=new Vector3(-0.5f,0f,0.5f);
        updateboardplace();
        for(int i=0;i<4;i++)
        {
            army1[i]=Instantiate(armyprefab,transform.position+pos[i],transform.rotation);
            
        }
        for(int i=0;i<4;i++)
        {
            //army1[i]=Instantiate(armyprefab,pos[i],transform.rotation);
            army1[i].GetComponent<army>().boardplacex=boardplacex;
            army1[i].GetComponent<army>().boardplacey=boardplacey;
            army1[i].GetComponent<army>().type=type;
            army1[i].GetComponent<army>().offsetx=pos[i].x;
            army1[i].GetComponent<army>().offsety=pos[i].z;
        }
    }
     public bool updatearmyatplace()
     {
         for(int i=0;i<4;i++)
         {
             if(Mathf.Abs(army1[i].transform.position.x-2*army1[i].GetComponent<army>().boardplacex+7-army1[i].GetComponent<army>().offsetx)>0.1f&&Mathf.Abs(army1[i].transform.position.z-2*army1[i].GetComponent<army>().boardplacey+7-army1[i].GetComponent<army>().offsety)>0.1f)
            {
                return false;
            }
         }
         return true;
     }
    public void updatearmytarget(GameObject aimtarget)
    {
        for(int i=0;i<4;i++)
        {
            army1[i].GetComponent<army>().target=aimtarget.GetComponent<chesspiece>().army1[i];
        }
    }
    public void updatearmydeath()
    {
        if(death){
            for(int i=0;i<4;i++)
            {
                army1[i].GetComponent<army>().death=true;
            }
        }
    }
    public void updatearmyselect()
    {
        for(int i=0;i<4;i++)
        {
            army1[i].GetComponent<army>().anim.SetBool("selected",true);
        }
    }
    public void updatearmyunselect()
    {
        for(int i=0;i<4;i++)
        {
            army1[i].GetComponent<army>().anim.SetBool("selected",false);
        }
    }
    public void updatearmyplace()
    {
        for(int i=0;i<4;i++)
        {
            army1[i].GetComponent<army>().boardplacex=boardplacex;
            army1[i].GetComponent<army>().boardplacey=boardplacey;
        }
    }
    public virtual void Start()
    {   
        
        board=GameObject.FindWithTag("board");
        if(createarmy)
        {
            spawnarmy();
        }
        anim=GetComponent<Animator>();
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
    }    
    public void selected()
    {
       // Debug.Log("selected:"+type);
        anim.SetBool("selected",true);
        if(createarmy)
        {
            updatearmyselect();
        }
    }

    public void unselected()
    {
        anim.SetBool("selected",false);
        if(createarmy)
        {
            updatearmyunselect();
        }
    }
    private void Update()
    {
        agentmove();
        if(health<=0){
            if(!deathsound.isPlaying)
            {deathsound.Play();}
            anim.enabled =false;
            ragdollactive();
            if(type>0){              
                lifetime=lifetime+Time.deltaTime;               
                if(lifetime>2)
                {
                    this.gameObject.SetActive(false);
                }
            }
            else{
                
                lifetime=lifetime+Time.deltaTime;
                
                if(lifetime>2)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
    public void ragdollactive()
    {
        if(rb.Count>0){
         foreach(Rigidbody limb in rb)
         {
             if(limb!=null){
            limb.isKinematic = false;}
         }
        }
    }
    public virtual void agentmove()
    {
        NavMeshAgent agent=GetComponent<NavMeshAgent>();
        if(target!=null&&target.activeSelf)
        {
            if(Vector3.Distance(target.transform.position,transform.position)<distance)
            {
                agent.isStopped=true;
                Vector3 direction = (target.transform.position -transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
                transform.rotation=Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime*5f);
                anim.SetBool("attack",true);
                if(atksound.isPlaying){}else{atksound.PlayDelayed(1);}
               
                anim.SetBool("walk",false);
                footstep.Stop();
                // if(target.GetComponent<chesspiece>().health<=0)
                // {
                    
                //     anim.SetBool("attack",false);
                // }
            }
            else
            {
                agent.isStopped=false;
                anim.SetBool("attack",false);
                atksound.Stop();
                anim.SetBool("walk",true);
                if(!footstep.isPlaying){footstep.Play();}
                agent.SetDestination(target.transform.position);
            }
        }
        else
        {  if(death){}
        else
        {
            
        
            anim.SetBool("attack",false);
            atksound.Stop();
            if(Mathf.Abs(transform.position.x-2*boardplacex+7)<0.1&&Mathf.Abs(transform.position.z-2*boardplacey+7)<0.1)
            {
                footstep.Stop();
                anim.SetBool("walk",false);
                if(type>0){
                    if (Mathf.Abs(transform.rotation.y)<0.1)
                    {
                        transform.rotation=Quaternion.Euler(0,0,0);
                    }else{
                        
                        transform.rotation= Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,0),  Time.deltaTime*100 );}
                    
                    }
                else{
                    if (Mathf.Abs(transform.rotation.y-180)<0.1)
                    {
                        transform.rotation=Quaternion.Euler(0,180,0);
                    }else{
                    transform.rotation= Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,180,0),  Time.deltaTime*100 );}
                }
            }
            else{//Debug.Log("footstep");
                if(!footstep.isPlaying){footstep.Play();}
                anim.SetBool("walk",true);agent.isStopped=false;agent.SetDestination(new Vector3(2*boardplacex-7,transform.position.y,2*boardplacey-7));}
        }
        }
    }
    // private void OnCollisionEnter(Collision other)  //OnTriggerEnter(Collider other) OnCollisionEnter(Collision other) 
    // {
        
    //     if (other.collider.tag=="white_weapon")
    //     // if (other.tag=="white_weapon")
    //     {
            
    //         if(type<0&&death)
    //         {
    //             gethurtsound.Play();
    //            // Debug.Log("hurtsound");
    //             health=health-2;
    //             this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead1",true);
    //             if(health<=0)
    //             {
    //                this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead2",true);
    //             }
    //         }
    //     }
    //     else if(other.collider.tag=="black_weapon")
    //     // else if(other.tag=="black_weapon")
    //     {
            
    //         if(type>0&&death)
    //         {
    //             gethurtsound.Play();
    //             //Debug.Log("hurtsound");
    //             health=health-2;
    //             this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead2",true);
    //             if(health<=0)
    //             {
    //                 this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead1",true);
    //             }
    //         }
    //     }
    // }
    // private void OnCollisionExit(Collision other)  //OnTriggerEnter(Collider other) OnCollisionEnter(Collision other) 
    // {
    //     //Debug.Log("enter");
    //     if (other.collider.tag=="white_weapon")
    //     // if (other.tag=="white_weapon")
    //     {
    //         //Debug.Log("enter");
    //         if(type<0&&death)
    //         {
    //             this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead1",false);
    //         }
    //     }
    //     else if(other.collider.tag=="black_weapon")
    //     // else if(other.tag=="black_weapon")
    //     {
    //         if(type>0&&death)
    //         {   
    //             this.gameObject.GetComponent<chesspiece>().anim.SetBool("dead2",false);
                
    //         }
    //     }
    // }
    public virtual List<Vector3> avaliablemove()
    {
        List<Vector3> pos=new List<Vector3>(); 
        //pos.Add(new Vector3(1,1,1));
        return pos;
    }
    public void updateboardplace()
    {
        boardplacex=(int)(((transform.position.x+7)/2));
        boardplacey=(int)(((transform.position.z+7)/2));
    }
    public virtual int move(int x,int y)
    {
        // Vector3 place=new Vector3(1,1.8f,1);
        return 0;
    }
    public virtual int newmove(int x,int y,List<Vector3> move)
    {
        
        return 0;
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
                //pawn check+knight check
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

    public pieceinformation[,] getinformation()
    {
        pieceinformation [,] allpieces=new pieceinformation [8,8];
        for(int i=0;i<=7;i++)
        {
            for(int j=0;j<=7;j++)
            {
                if(board.GetComponent<chessboardmanager>().pieces[i,j]!=null)
                {
                    allpieces[i,j].type=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().type;
                    allpieces[i,j].boardplacex=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().boardplacex;
                    allpieces[i,j].boardplacey=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().boardplacey;
                    allpieces[i,j].kingchecked=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().kingchecked;
                    allpieces[i,j].moved=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().moved;
                    allpieces[i,j].enpassantstatus=board.GetComponent<chessboardmanager>().pieces[i,j].GetComponent<chesspiece>().enpassantstatus;
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
    //need change checkstatus enpassant below
    public pieceinformation[,] switchinformation(int oldx,int oldy,int newx,int newy,pieceinformation[,] oldinfo)//may be add int type for pawn promotion
    {
        pieceinformation [,] allpieces=new pieceinformation [8,8];
        for(int i=0;i<=7;i++)
        {
            for(int j=0;j<=7;j++)
            {
                if(board.GetComponent<chessboardmanager>().pieces[i,j]!=null)
                {
                    allpieces[i,j].type=oldinfo[i,j].type;
                    allpieces[i,j].boardplacex=oldinfo[i,j].boardplacex;
                    allpieces[i,j].boardplacey=oldinfo[i,j].boardplacey;
                    allpieces[i,j].kingchecked=oldinfo[i,j].kingchecked;
                    allpieces[i,j].moved=oldinfo[i,j].moved;
                    allpieces[i,j].enpassantstatus=oldinfo[i,j].enpassantstatus;
                }
                else
                {
                    allpieces[i,j].type=0;
                    allpieces[i,j].boardplacex=i;
                    allpieces[i,j].boardplacey=j;
                    allpieces[i,j].kingchecked=false;
                    allpieces[i,j].moved=false;
                    allpieces[i,j].enpassantstatus=false;
                }
            }       
        }
        allpieces[newx,newy].type=allpieces[oldx,oldy].type;
        allpieces[newx,newy].boardplacex=newx;
        allpieces[newx,newy].boardplacey=newy;
        allpieces[newx,newy].kingchecked=kingchecked;
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
    // Need to check checkmate situation after moving
    public virtual List<Vector3> newavaliablemove(pieceinformation[,] info)
    {
        List<Vector3> pos=new List<Vector3>();
        // pieceinformation[,] info=getinformation();
        if(Mathf.Abs(info[boardplacex,boardplacey].type)==1)//pawn
        {
            Vector3 move;
            //normalmove -no enemy
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,info[boardplacex,boardplacey].type);
            if(move.x!=-1&&move.y==0)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
            }
            //special first move
            if(!info[boardplacex,boardplacey].moved)
            {
                move=checkpawnfirstmove(boardplacex,boardplacey,info,info[boardplacex,boardplacey].type);
                if(move.x!=-1)
                {
                    if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
                }
            }
            
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,info[boardplacex,boardplacey].type);
            if(move.x!=-1)
            {
                if(move.y==2)
                {
                    if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
                }     
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,info[boardplacex,boardplacey].type);
            if(move.x!=-1)
            {
                if(move.y==2)
                {
                    if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
                }
            }
            //enpassant
            move=checkenpassantright(boardplacex,boardplacey,info);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
            }
            move=checkenpassantleft(boardplacex,boardplacey,info);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type)){pos.Add(move);}
            }
        }
        else if(Mathf.Abs(info[boardplacex,boardplacey].type)==2)//rook
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
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/2)){pos.Add(move);}
                        if(move.y==2){up=false;}
                    }
                    else{up=false;}
                }
                if(down)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/2)){pos.Add(move);}
                        if(move.y==2){down=false;}
                    }
                    else{down=false;}
                }
                if(left)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/2)){pos.Add(move);}
                        if(move.y==2){left=false;}
                    }
                    else{left=false;}
                }
                if(right)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/2)){pos.Add(move);}
                        if(move.y==2){right=false;}
                    }
                    else{right=false;}
                }
                if((!up)&&(!down)&&(!left)&&(!right)){break;}
            }
        }
        else if(Mathf.Abs(info[boardplacex,boardplacey].type)==3)//horse
        {
            // Debug.Log("test horse function");
            Vector3 move;
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,2,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-2,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-2,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,2,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,2);
            if(move.x!=-1)
            {
                //Debug.Log((int)info[boardplacex,boardplacey].type/3);
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,-2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,-2);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/3)){pos.Add(move);}
            }

        }
        else if(Mathf.Abs(info[boardplacex,boardplacey].type)==4)//bishop
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
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/4)){pos.Add(move);}
                        if(move.y==2){upright=false;}
                    }
                    else{upright=false;}
                }
                if(downleft)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/4)){pos.Add(move);}
                        if(move.y==2){downleft=false;}
                    }
                    else{downleft=false;}
                }
                if(upleft)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/4)){pos.Add(move);}
                        if(move.y==2){upleft=false;}
                    }
                    else{upleft=false;}
                }
                if(downright)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/4)){pos.Add(move);}
                        if(move.y==2){downright=false;}
                    }
                    else{downright=false;}
                }
                if((!upright)&&(!downleft)&&(!upleft)&&(!downright)){break;}
            }
        }
        else if(Mathf.Abs(info[boardplacex,boardplacey].type)==5)//queen
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
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){up=false;}
                    }
                    else{up=false;}
                }
                if(down)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){down=false;}
                    }
                    else{down=false;}
                }
                if(left)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){left=false;}
                    }
                    else{left=false;}
                }
                if(right)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,0);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){right=false;}
                    }
                    else{right=false;}
                }
                if(upright)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){upright=false;}
                    }
                    else{upright=false;}
                }
                if(downleft)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){downleft=false;}
                    }
                    else{downleft=false;}
                }
                if(upleft)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-i,i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){upleft=false;}
                    }
                    else{upleft=false;}
                }
                if(downright)
                {
                    move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,i,-i);
                    if(move.x!=-1)
                    {
                        if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/5)){pos.Add(move);}
                        if(move.y==2){downright=false;}
                    }
                    else{downright=false;}
                }
                if((!up)&&(!down)&&(!left)&&(!right)&&(!upright)&&(!downleft)&&(!upleft)&&(!downright)){break;}
            }
            
        }
        else if(Mathf.Abs(info[boardplacex,boardplacey].type)==6)//king
        {
            // Debug.Log("test king function");
            Vector3 move;
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,1);
            if(move.x!=-1)
            {
                //change to kingself check
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,1,0);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,-1,0);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkavailablemovebynormalrule(boardplacex,boardplacey,info,0,-1);
            if(move.x!=-1)
            {
                if(!checkstatus(switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkKScastling(boardplacex,boardplacey,info);
            if(move.x!=-1&&boardplacex==4)
            {
                if(!checkstatus(switchinformation(7,boardplacey,boardplacex+1,boardplacey,switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info)),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
            move=checkQScastling(boardplacex,boardplacey,info);
            if(move.x!=-1&&boardplacex==4)
            {
                if(!checkstatus(switchinformation(0,boardplacey,boardplacex-1,boardplacey,switchinformation(boardplacex,boardplacey,(int)move.x,(int)move.z,info)),(int)info[boardplacex,boardplacey].type/6)){pos.Add(move);}
            }
        }
        return pos;
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
        if(!board.GetComponent<chessboardmanager>().check)
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
        if(!board.GetComponent<chessboardmanager>().check)
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
}
