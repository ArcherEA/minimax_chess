using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class gethurt : MonoBehaviour
{
    public GameObject parent;
    public AudioSource gethurtsound;
    public AudioSource attacksound;
    private Transform target;
   private void  OnCollisionEnter(Collision other) //OnTriggerEnter(Collider other) OnCollisionEnter(Collision other) 
    {
        //Debug.Log(other.collider.tag);
        //if (other.collider.tag=="white_weapon")
        if (other.collider.tag=="white_weapon")
        {
            //Debug.Log("enter");
            if(parent.GetComponent<chesspiece>().type<0&&parent.GetComponent<chesspiece>().death)
            {
                target=other.collider.attachedRigidbody.transform;
                //parent.GetComponent<chesspiece>().anim.SetBool("attack",true);
                if(parent.GetComponent<chesspiece>().health>=0){
                    gethurtsound.Play();
                    attacksound.Play();
                    StartCoroutine("GetHit");}
               
               // Debug.Log("hurtsound");
                parent.GetComponent<chesspiece>().health=parent.GetComponent<chesspiece>().health-2;
                //StartCoroutine("GetHit");
                // parent.GetComponent<chesspiece>().anim.SetBool("dead1",true);
                // if(parent.GetComponent<chesspiece>().health<=0)
                // {
                //    parent.GetComponent<chesspiece>().anim.SetBool("dead2",true);
                // }
            }
        }
        //else if(other.collider.tag=="black_weapon")
        else if(other.collider.tag=="black_weapon")
        {
            
            if(parent.GetComponent<chesspiece>().type>0&&parent.GetComponent<chesspiece>().death)
            {
                target=other.collider.attachedRigidbody.transform;
                //parent.GetComponent<chesspiece>().anim.SetBool("attack",true);
                if(parent.GetComponent<chesspiece>().health>=0){
                    gethurtsound.Play();
                    attacksound.Play();
                    StartCoroutine("GetHit");
                    }               
                //Debug.Log("hurtsound");
                parent.GetComponent<chesspiece>().health=parent.GetComponent<chesspiece>().health-2;
                //StartCoroutine("GetHit");
                // parent.GetComponent<chesspiece>().anim.SetBool("dead2",true);
                // if(parent.GetComponent<chesspiece>().health<=0)
                // {
                //     parent.GetComponent<chesspiece>().anim.SetBool("dead1",true);
                // }
            }
        }
    }
    IEnumerator GetHit()
    {
        parent.GetComponent<chesspiece>().anim.SetBool("atk",false);
        parent.GetComponent<chesspiece>().anim.SetBool("dead1",true);
        Vector3 pos=new Vector3(transform.position.x+0.2f*(transform.position.x-target.position.x),transform.position.y,transform.position.z+0.2f*(transform.position.z-target.position.z));
        //Quaternion lookRotation = Quaternion.LookRotation(new Vector3(target.position.x-transform.position.x,0,target.position.z-transform.position.z));
        for (int i =0;i<20;i++)
        {
            float interpolationRatio=(float) i/20;
            if(parent.GetComponent<chesspiece>().health>0)
            {
                Vector3 interpolatedPosition=Vector3.Lerp(transform.position, pos,interpolationRatio);
                parent.GetComponent<NavMeshAgent>().nextPosition=interpolatedPosition;
                //transform.rotation=lookRotation;
                yield return new WaitForSeconds(0.01f);
            } 
            
        }
        parent.GetComponent<chesspiece>().anim.SetBool("atk",true);
        parent.GetComponent<chesspiece>().anim.SetBool("dead1",false);
        
        
    }

    // private void OnCollisionExit(Collision other)  //OnTriggerEnter(Collider other) OnCollisionEnter(Collision other) 
    // {
    //     //Debug.Log("enterexit");
    //     //if (other.collider.tag=="white_weapon")
    //     if (other.collider.tag=="white_weapon")
    //     {
    //         Debug.Log("enterexit");
    //         //Debug.Log("enter");
    //         if(parent.GetComponent<chesspiece>().type<0&&parent.GetComponent<chesspiece>().death)
    //         {
    //             parent.GetComponent<chesspiece>().anim.SetBool("dead1",false);
    //         }
    //     }
    //     //else if(other.collider.tag=="black_weapon")
    //     else if(other.collider.tag=="black_weapon")
    //     {
    //         if(parent.GetComponent<chesspiece>().type>0&&parent.GetComponent<chesspiece>().death)
    //         {   
    //             parent.GetComponent<chesspiece>().anim.SetBool("dead2",false);
                
    //         }
    //     }
    // }
}
