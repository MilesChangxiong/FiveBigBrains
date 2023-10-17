using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Player player1Prefab;
    public Player player2Prefab;
    
    //change to public to be used in Taunt()
    public static Player player1Instance;
    public static Player player2Instance;

    public TextMeshProUGUI winText;
    public Button restartButton;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        SpawnPlayers();
        winText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false); 
    }

    private void SpawnPlayers()
    {
        player1Instance = Instantiate(player1Prefab, new Vector3(-10, 2, 0), Quaternion.identity);
        player2Instance = Instantiate(player2Prefab, new Vector3(10, 2, 0), Quaternion.identity);

        player1Instance.controlType = Player.PlayerControlType.WASD;
        player1Instance.playerColor = new Color(33 / 255f, 147 / 255f, 255 / 255f, 255 / 255f); // Blue
        player1Instance.opponent = player2Instance;

        player2Instance.controlType = Player.PlayerControlType.ARROW_KEYS;
        player2Instance.playerColor = new Color(255 / 255f, 67 / 255f, 40 / 255f, 255 / 255f); // Red
        player2Instance.opponent = player1Instance;

        player1Instance.OnPlayerLivesChanged += CheckForGameOver;
        player2Instance.OnPlayerLivesChanged += CheckForGameOver;
    }

    private void OnDestroy() 
    {
        if (player1Instance != null)
            player1Instance.OnPlayerLivesChanged -= CheckForGameOver;

        if (player2Instance != null)
            player2Instance.OnPlayerLivesChanged -= CheckForGameOver;
    }

    private void CheckForGameOver(Player player)
    {
        if (player1Instance.remainingLives == 0)
        {
            EndGame("Red Wins");
        }
        else if (player2Instance.remainingLives == 0)
        {
            EndGame("Blue Wins");
        }
    }

    private void EndGame(string message)
    {
        winText.gameObject.SetActive(true);
        winText.text = message;
        restartButton.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnRestartButtonClick() 
    {
        Time.timeScale = 1;
        Destroy(player1Instance.gameObject);
        Destroy(player2Instance.gameObject);
        StartGame();
    }
}
