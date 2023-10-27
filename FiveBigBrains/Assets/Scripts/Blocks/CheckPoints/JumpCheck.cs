using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCheck : MonoBehaviour
{
    public TutoralSceneManager tutoralSceneManagerReference;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           Debug.Log("Player entered the jumpcheckpoint!");
           ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        tutoralSceneManagerReference.isTrigger2 = true;
        Debug.Log("Checkpoint2"+ tutoralSceneManagerReference.isTrigger2);
    }
}
