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
    public TextMeshProUGUI textToDisplay;
    public GameObject moveCheckPoint1;
    public GameObject moveCheckPoint2;
    public GameObject jumpCheckPoint; 
    private bool hasShownText = false;
    
    public GameObject moveIcon;
    public GameObject jumpIcon;
    public GameObject attackIcon;
    public GameObject tauntIcon;

    public float blinkInterval = 0.5f;
    public Color blinkColor1=Color.yellow ;
    public Color blinkColor2 = Color.white;

    public bool everTaunt=false;

    bool hasClearedEverTaunt = false;

    // Start is called before the first frame update
    void Start()
    {
        textToDisplay.enabled = false;
        moveCheckPoint1.SetActive(true);
        moveCheckPoint2.SetActive(true);
        jumpCheckPoint.SetActive(false);
        moveIcon.SetActive(true);
        jumpIcon.SetActive(false);
        attackIcon.SetActive(false);
        tauntIcon.SetActive(false);
        StartCoroutine(BlinkCheckpoints());
        
    }

    // Update is called once per frame
    void Update()
    {
         if(!isTrigger1)
        {
            movementInstructionText.text = "Move";
        }
        else if(isTrigger1&&!isTrigger2)
        {
            moveCheckPoint1.SetActive(false);
            moveCheckPoint2.SetActive(false);
            jumpCheckPoint.SetActive(true);
            moveIcon.SetActive(false);
            jumpIcon.SetActive(true);
            movementInstructionText.text = "Jump";
            textToDisplay.text = "Try double jump to arrive at the stage.";
            textToDisplay.enabled = true;
        }else if( isTrigger1&&isTrigger2&&GameManager.instance != null &&( !GameManager.instance.showLifeLayerText)){
            jumpCheckPoint.SetActive(false);
            jumpIcon.SetActive(false);
            attackIcon.SetActive(true);
            movementInstructionText.text = "Attack";
            textToDisplay.text = "Attack your opponent's head!";
            textToDisplay.enabled = true;
        }else{
            if (!hasClearedEverTaunt)
            {
                GameManager.player1Instance.everTaunt = false;
                GameManager.player2Instance.everTaunt = false;
                hasClearedEverTaunt = true;
            }
            
            movementInstructionText.text = "Taunt";
            attackIcon.SetActive(false);
            tauntIcon.SetActive(true);

            textToDisplay.text = "Taunt your opponent one or more times while facing away for an easier attack!"; 
            textToDisplay.enabled = true;
            if(everTaunt==false){
                GameManager.player1Instance.canAttack = false;
                GameManager.player2Instance.canAttack = false;
            }
            if(GameManager.player1Instance.everTaunt || GameManager.player2Instance.everTaunt)
            {
                textToDisplay.text = "Each of you has 3 lives. Continue attack to win!";
                textToDisplay.enabled = true;
                GameManager.player1Instance.canAttack = true;
                GameManager.player2Instance.canAttack = true;
                everTaunt = true;
            }
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
    
    private IEnumerator BlinkCheckpoints()
    {
        SpriteRenderer moveCheckPoint1Renderer = moveCheckPoint1.GetComponent<SpriteRenderer>();
        SpriteRenderer moveCheckPoint2Renderer = moveCheckPoint2.GetComponent<SpriteRenderer>();
        SpriteRenderer jumpCheckPointRenderer = jumpCheckPoint.GetComponent<SpriteRenderer>();
        //blinkColor1=moveCheckPoint1Renderer.material.color;
        while (true) 
        {
            // Toggle between blinkColor1 and blinkColor2
            moveCheckPoint1Renderer.color = moveCheckPoint1Renderer.color == blinkColor1 ? blinkColor2 : blinkColor1;
            moveCheckPoint2Renderer.color = moveCheckPoint2Renderer.color == blinkColor1 ? blinkColor2 : blinkColor1;
            jumpCheckPointRenderer.color = jumpCheckPointRenderer.color == blinkColor1 ? blinkColor2 : blinkColor1;
            yield return new WaitForSeconds(blinkInterval);
        }
    }

}
