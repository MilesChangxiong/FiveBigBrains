using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCheckpoint : MonoBehaviour
{


    public TutoralSceneManager tutoralSceneManagerReference;
    
    void Start()
    {
        
    }

  
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           Debug.Log("Player entered the movecheckpoint!");
           ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        tutoralSceneManagerReference.isTrigger1 = true;
        Debug.Log("Checkpoint1"+ tutoralSceneManagerReference.isTrigger1);
    }
}
