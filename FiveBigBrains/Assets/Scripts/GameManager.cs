using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player1Prefab;
    public Player player2Prefab;
    
    public static Player player1Instance;
    public static Player player2Instance;

    private int player1Score = 0;
    private int player2Score = 0;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    public int WinningScore = 2; // TODO: changable in MainMenu
    public string winnerName; // this is used in Victory scene.

    private DropManager dropManager;

    private void Start()
    {
        dropManager = FindObjectOfType<DropManager>();
    }

    private void StartGame()
    {
        SpawnPlayers();
        dropManager.SpawnPistolPowerUp();
    }

    public void SwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SwitchToVictoryScene()
    {
        SceneManager.LoadScene("Victory");
        SceneManager.sceneLoaded += OnVictorySceneLoaded;
    }

    void OnVictorySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player1Score == WinningScore)
        {
            SpawnPlayer1();
        }
        else
        {
            SpawnPlayer2();
        }
            
        SceneManager.sceneLoaded -= OnVictorySceneLoaded;
    }

    public void RestartGameAfterVictory()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SpawnPlayers()
    {
        SpawnPlayer1();
        SpawnPlayer2();

        player1Instance.opponent = player2Instance;
        player2Instance.opponent = player1Instance;
    }

    void SpawnPlayer1()
    {
        player1Instance = Instantiate(player1Prefab, new Vector3(-10, 2, 0), Quaternion.identity);

        player1Instance.controlType = Player.PlayerControlType.WASD;
        player1Instance.playerColor = new Color(33 / 255f, 147 / 255f, 255 / 255f, 255 / 255f); // Blue
        player1Instance.OnPlayerDied += OnPlayerDies;
    }

    void SpawnPlayer2()
    {
        player2Instance = Instantiate(player2Prefab, new Vector3(10, 2, 0), Quaternion.identity);

        player2Instance.controlType = Player.PlayerControlType.ARROW_KEYS;
        player2Instance.playerColor = new Color(255 / 255f, 67 / 255f, 40 / 255f, 255 / 255f); // Red
        player2Instance.OnPlayerDied += OnPlayerDies;
    }

    private void OnDestroy() 
    {
        if (player1Instance != null)
            player1Instance.OnPlayerDied -= OnPlayerDies;

        if (player2Instance != null)
            player2Instance.OnPlayerDied -= OnPlayerDies;
    }

    private void OnPlayerDies(Player player)
    {
        if (player1Instance.remainingLives <= 0)
        {
            player2Score += 1;
        }
        else if (player2Instance.remainingLives <= 0)
        {
            player1Score += 1;
        }

        SetPlayerScoreTexts();

        if (player1Score == WinningScore)
        {
            winnerName = "Player 1";
            SwitchToVictoryScene();
            return;
        }
        if (player2Score == WinningScore)
        {
            winnerName = "Player 2";
            SwitchToVictoryScene();
            return;
        }

        SwitchScene("Map1"); // TODO: change to next random map.
    }

    void SetPlayerScoreTexts()
    {
        player1ScoreText.text = "Score: " + player1Score;
        player2ScoreText.text = "Score: " + player2Score;
    }

    /// <summary>
    /// Ensures a single instance of GameManager persists across scenes.
    /// If a new instance is detected (when returning to the main menu),
    /// the old instance's relevant data will be copied to the new instance before the old one is destroyed.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // Copy relevant data from the old GameManager instance
            player1ScoreText = instance.player1ScoreText;
            player2ScoreText = instance.player2ScoreText;

            // Update ScoreText.text
            SetPlayerScoreTexts();

            // Destroy the old GameManager instance and set this as the new singleton instance
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
