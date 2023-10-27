using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
     private void OnTriggerEnter(Collider other)
    {

         Debug.Log("Something entered the checkpoint");
        
        if (other.CompareTag("Player")) // Assuming your player has the tag "Player"
        {
             Debug.Log("Checkpoint Activated!");
            ActivateCheckpoint();
        }
    }

    void ActivateCheckpoint()
    {
        // Your checkpoint logic here.
        // This could be saving the game, updating a level state, etc.
        Debug.Log("Checkpoint Activated!");
    }
}
