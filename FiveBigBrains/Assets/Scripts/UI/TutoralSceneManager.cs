using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TutoralSceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGameAfterVictory();
        }
    }

     private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " has entered the checkpoint.");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Checkpoint Activated!");

        }
    }
}
