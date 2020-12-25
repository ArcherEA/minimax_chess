using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class army : MonoBehaviour
{
    public int type;
    public float boardplacex;
    public float boardplacey;
    public float offsetx=0;
    public float offsety=0;
    public Animator anim;
    public float lifetime=0;
    public bool death=false;
    public int health=5;
    public AudioSource footstep;
    public AudioSource atksound;
    public AudioSource deathsound;
    public GameObject target=null;
    private GameObject board;
    public float distance=2f;
    public List<Rigidbody> rb;
    
    // Start is called before the first frame update
    void Start()
    {
        board=GameObject.FindWithTag("board");
        transform.SetParent(board.transform);
        
    }

    // Update is called once per frame
    private void Update()
    {
        agentmove();
        if(health<=0){
            if(!deathsound.isPlaying){
            deathsound.Play();}
            anim.enabled =false;
            ragdollactive();
            //gethurtsound.Stop();
            
                
            lifetime=lifetime+Time.deltaTime;
            
            if(lifetime>2)
            {
                this.gameObject.SetActive(false);
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
        else{
            anim.SetBool("attack",false);
            atksound.Stop();
            if(Mathf.Abs(transform.position.x-2*boardplacex+7-offsetx)<0.1&&Mathf.Abs(transform.position.z-2*boardplacey+7-offsety)<0.1)
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
                anim.SetBool("walk",true);agent.isStopped=false;agent.SetDestination(new Vector3(2*boardplacex-7+offsetx,transform.position.y,2*boardplacey-7+offsety));}
        }
        }
    }
}
