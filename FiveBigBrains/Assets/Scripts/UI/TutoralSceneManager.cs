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

    
    public float blinkInterval = 0.5f;
    public Color blinkColor1=Color.yellow ;
    public Color blinkColor2 = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        textToDisplay.enabled = false;
        moveCheckPoint1.SetActive(true);
        moveCheckPoint2.SetActive(true);
        jumpCheckPoint.SetActive(false);
        StartCoroutine(BlinkCheckpoints());
        
    }

    // Update is called once per frame
    void Update()
    {
         if(!isTrigger1)
        {
            movementInstructionText.text = "A/D  Move  Arrows";
        }
        else if(isTrigger1&&!isTrigger2)
        {
            moveCheckPoint1.SetActive(false);
            moveCheckPoint2.SetActive(false);
            jumpCheckPoint.SetActive(true);
            movementInstructionText.text = "Space  Jump  Enter";
        }else {
            jumpCheckPoint.SetActive(false);
            movementInstructionText.text = "R  Attack  L";
        }
        
        if (!hasShownText && GameManager.instance != null && GameManager.instance.showLifeLayerText)
        {
            movementInstructionText.text = "Q  Taunt  Shift";
            StartCoroutine(ShowTextTemporarily(2f)); // Show text for 5 seconds
            hasShownText = true;
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
    private IEnumerator ShowTextTemporarily(float duration)
    {
        textToDisplay.enabled = true;
        textToDisplay.text = "Each balloon layer is a life"; 
        yield return new WaitForSeconds(duration);
        textToDisplay.enabled = false;
        GameManager.instance.showLifeLayerText = false;
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
