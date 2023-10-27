using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictorySceneManager : MonoBehaviour
{
    public TextMeshProUGUI victoryText;

    private void Start()
    {
        if (GameManager.instance != null)
        {
            victoryText.text = GameManager.instance.winnerName + " Wins!";
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
