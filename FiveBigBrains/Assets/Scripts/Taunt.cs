using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taunt : MonoBehaviour

{
     // Assuming you already have a reference to Player 2's object.
    public Transform player2Transform;

    // Scale factor by which Player 2 will increase when taunted.
    public float tauntScaleFactor = 1.1f; // Adjust as necessary
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleTaunt();
    }
    void HandleTaunt()
    {
        // Check if the "T" key is pressed.
        if (Input.GetKeyDown(KeyCode.T))
        {
                //Debug.Log("T key pressed!");
                //Debug.Log("Before scale: " + player2Transform.localScale);
                player2Transform.localScale *= tauntScaleFactor;
                //Debug.Log("After scale: " + player2Transform.localScale);
            
            // Increase the size of Player 2.
            //player2Transform.localScale *= tauntScaleFactor;
            // Vector3 newScale = player2Transform.localScale * tauntScaleFactor;
            // player2Transform.localScale = newScale;
        }
    }

}
