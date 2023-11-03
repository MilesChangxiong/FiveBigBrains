using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using TMPro;

[System.Serializable]
public class MapStatistics
{
    public float MovementDistance = 0f;
    public int AttackCount = 0;
    public int JumpCount = 0;
    public int PowerupPickupCount = 0;
    public int TauntCount = 0;


    public int Player1PowerupPickupCount = 0;
    public int Player2PowerupPickupCount = 0;
    public int Player1Wins = 0;
    public int Player2Wins = 0;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private List<string> allMaps = new List<string>()
        {
            "FiregunAndIce",
            "Laser",
            "WindRopeBallBox",
        };
    public MapStatistics currentMapStats = new MapStatistics();

    public Player player1Prefab;
    public Player player2Prefab;
    
    public static Player player1Instance;
    public static Player player2Instance;

    public Vector3 defaultSpawnPoint1 = new Vector3(-10, 2, 0);
    public Vector3 defaultSpawnPoint2 = new Vector3(10, 2, 0);

    private int player1Score = 0;
    private int player2Score = 0;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;

    public int WinningScore = 2; // TODO: changable in MainMenu
    public string winnerName; // this is used in Victory scene.

    public string currScene; 
    public bool showLifeLayerText; //if its true, will display"layers=life" text 

    public TextMeshProUGUI sceneInstruction;
    public Image instructionBg;
    private Coroutine instructionCoroutine; 

    private void Start()
    {

    }

    private void StartGame()
    {
        SpawnPlayers();
    }
    
    private void Update(){
        CheckShowLifeText();
    }

    void reportMapStatisticsAndReset()
    {
        MapEvent mapEvent = new MapEvent(currScene, currentMapStats);
        GameReport.Instance.PostDataToFirebase("mapData", mapEvent);
        currentMapStats = new MapStatistics();
    }

    public void SwitchScene(string sceneName)
    {
        reportMapStatisticsAndReset();
        currScene = sceneName;
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
        if (instructionCoroutine != null)
        {
           StopCoroutine(instructionCoroutine);
           instructionCoroutine = null;
        }
        if(currScene=="WindRopeBallBox"){
            instructionCoroutine = StartCoroutine(ShowInstruction("Fire at pumpkins and boxes to increase their size and control yourself not to fall", 3f));
        }
        else if(currScene=="Laser"){
           instructionCoroutine = StartCoroutine(ShowInstruction("Mirror can be destroyed to change lazer", 3f));
        } else if(currScene=="FiregunAndIce"){
            instructionCoroutine = StartCoroutine(ShowInstruction("Only fire can melt ice", 3f));
        } else{    
            if (instructionBg != null) instructionBg.enabled = false;
            if (sceneInstruction != null) sceneInstruction.enabled = false;
        }
    }
    private IEnumerator ShowInstruction(string text, float delay) {
        if((instructionBg != null)&&(sceneInstruction != null)){
            instructionBg.enabled=true;
            sceneInstruction.enabled = true;
            sceneInstruction.text = text;
            yield return new WaitForSeconds(delay);
            sceneInstruction.enabled = false;
            instructionBg.enabled=false; 

        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SwitchToVictoryScene()
    {
        reportMapStatisticsAndReset();
        SceneManager.LoadScene("Victory");
        SceneManager.sceneLoaded += OnVictorySceneLoaded;
    }

    void OnVictorySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player1Score == WinningScore||(currScene=="Tutorial"&&winnerName=="Player 1"))
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
        Vector3 spawnPosition;
        if (currScene=="Tutorial"){
            spawnPosition=new Vector3(-20, 2, 0);
        }else{
            spawnPosition = defaultSpawnPoint1;
        }
        GameObject spawnPoint = GameObject.Find("SpawnPoint1");
        if (spawnPoint)
            spawnPosition = spawnPoint.transform.position;

        player1Instance = Instantiate(player1Prefab, spawnPosition, Quaternion.identity);

        player1Instance.controlType = Player.PlayerControlType.WASD;
        player1Instance.playerColor = new Color(33 / 255f, 147 / 255f, 255 / 255f, 255 / 255f); // Blue
        player1Instance.OnPlayerDied += OnPlayerDies;
    }

    void SpawnPlayer2()
    {
        Vector3 spawnPosition;
        if (currScene=="Tutorial"){
            spawnPosition=new Vector3(20, 2, 0);
        }else{
            spawnPosition = defaultSpawnPoint2;
        }
        GameObject spawnPoint = GameObject.Find("SpawnPoint2");
        if (spawnPoint)
            spawnPosition = spawnPoint.transform.position;
        player2Instance = Instantiate(player2Prefab, spawnPosition, Quaternion.identity);

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
            currentMapStats.Player2Wins = 1;
            if(currScene=="Tutorial"){
                winnerName = "Player 2";
                SwitchToVictoryScene();
                return;
            }
        }
        else if (player2Instance.remainingLives <= 0)
        {
            player1Score += 1;
            currentMapStats.Player1Wins = 1;
            if (currScene=="Tutorial"){
                winnerName = "Player 1";
                SwitchToVictoryScene();
                return;
            }
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

        if (currScene != "Tutorial"){
            //SwitchToDifferentRandomMap();
            SwitchToSequentialMap();
        }
    }
    
    private void CheckShowLifeText(){
        if (player1Instance == null || player2Instance == null) {
        return; 
        }
        if((player1Instance.remainingLives==2||player2Instance.remainingLives==2)&&currScene=="Tutorial"){
            showLifeLayerText=true;
        }   
    }

    public void SwitchToDifferentRandomMap()
    {
        List<string> availableMaps = new List<string>(allMaps); // Create a copy of all maps
        availableMaps.Remove(currScene); // Remove current scene from the list

        int randomIndex = UnityEngine.Random.Range(0, availableMaps.Count);
        string nextMap = availableMaps[randomIndex];

        SwitchScene(nextMap);
    }

    public void SwitchToSequentialMap()
    {
        int currentIndex = allMaps.IndexOf(currScene);  // Find the current scene's index
        int nextIndex = (currentIndex + 1) % allMaps.Count;  // Get the next index, and wrap around if at the end
        string nextMap = allMaps[nextIndex];  // Get the next map using the next index

        SwitchScene(nextMap);
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
