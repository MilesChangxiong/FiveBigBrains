using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TutoralSceneManager : MonoBehaviour
{
    public bool isTrigger1=false;
    public bool isTrigger2=false;
    public TextMeshProUGUI movementInstructionText; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if(!isTrigger1)
        {
            movementInstructionText.text = "Use A/D or arrows to Move";
        }
        else if(isTrigger1&&!isTrigger2)
        {
            movementInstructionText.text = "use Space or Enter to jump";
        }else {
            movementInstructionText.text = "use R or L to attack";
        }
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu...");
        if (GameManager.instance != null)
        {
            GameManager.instance.RestartGameAfterVictory();
        }
    }

}
