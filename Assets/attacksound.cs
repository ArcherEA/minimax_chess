using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attacksound : MonoBehaviour
{
    public GameObject parent;
    public AudioSource attack;
    private void OnCollisionEnter(Collision other)  //OnTriggerEnter(Collider other) OnCollisionEnter(Collision other) 
    {
        //Debug.Log(other.gameObject);
        if (other.gameObject==parent.GetComponent<chesspiece>().target)
        {
            attack.Play();
        } 
    }
   
}
