using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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
            "StrongWind",
            "FiregunAndIce",
            "Laser",
            "WindRopeBallBox",
            "Bridge",
            
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
    public VideoPlayer videoPlayer;
    public Canvas instructionCanvas;
    public RawImage rawImage; 
    public bool isShowLaser=false; 
    public bool isShowWind=false;
    public bool isShowFire=false; 
    // Time Scale
    private int timeScale = 1;
    private bool isSlowMotion = false;

    // Count Down
    public TextMeshProUGUI countDownText;
    public bool isCountingDown = false;
    private bool isGamePaused = false;
    private const float COUNT_DOWN_TIME = 3.0f;
    private float currentCountDownTime = COUNT_DOWN_TIME;
    

    private void Start()
    {
        // TODO:
        SetCameraAspectRatios();
        instructionBg.enabled = false;
        sceneInstruction.enabled = false;
        countDownText.text = "";
        instructionCanvas.enabled=false; 
    }

    private void StartGame()
    {
        SpawnPlayers();
    }

    private void Update()
    {
        CheckShowLifeText();
    }

    static void SetCameraAspectRatios()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            cam.aspect = 16f / 9f; 
        }
    }

    void reportMapStatisticsAndReset()
    {
        MapEvent mapEvent = new MapEvent(currScene, currentMapStats);
        GameReport.Instance.PostDataToFirebase("mapData", mapEvent);
        currentMapStats = new MapStatistics();
    }

    public void SwitchScene(string sceneName)
    {
        resetTimeScale();
        reportMapStatisticsAndReset();
        currScene = sceneName;
        SceneManager.LoadScene(sceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;


        if (currScene == "StrongWind")
        {
            ShowInstruction("","Be careful! The wind is stong");
        }
        else if (currScene == "WindRopeBallBox"&&isShowWind==false)

        {
             isShowWind=true;
             ShowInstruction( "Videos/WindRopeBallBoxClip.mp4","Fire at pumpkins and boxes to increase their size and control yourself not to fall");
        }
        else if (currScene == "Laser"&&isShowLaser==false)
        {

             isShowLaser=true;
             ShowInstruction( "Videos/LaserClip.mov","Mirror can be destroyed to change lazer");

        }
        else if (currScene == "FiregunAndIce"&&isShowFire==false)
        {
            isShowFire=true;
            ShowInstruction( "Videos/FiregunAndIceClip.mp4","Only fire can melt ice");
        }
        else if(currScene=="Bridge"){
            ShowInstruction("","Bridge can be destroyed");
        }
        
        else
        {
            instructionCanvas.enabled=false; 
            if (instructionBg != null) instructionBg.enabled = false;
            if (sceneInstruction != null) sceneInstruction.enabled = false;
        }
    }
    private void ShowInstruction(string videoPath,string text)
    {   
        Debug.Log("1"+currScene);
       
        if ((instructionCanvas!=null)&&(instructionBg != null) && (sceneInstruction != null))
        {
            Debug.Log("2"+currScene);
            
            instructionCanvas.enabled=true;
            videoPlayer.enabled=false; 
            rawImage.enabled=false;
            if(videoPath!=""){
                videoPlayer.enabled=true;
                rawImage.enabled=true;
                string filePath = System.IO.Path.Combine(Application.dataPath, videoPath);
                videoPlayer.url = filePath;
            }

            // videoPlayer.Prepare();
            // videoPlayer.Play();
            instructionBg.enabled = true;
            sceneInstruction.enabled = true;
            sceneInstruction.text = text;
            PauseGame();
            // yield return new WaitForSeconds(delay);
            // sceneInstruction.enabled = false;
            // instructionBg.enabled = false;

        }

    }

    public void returnToGame(){
            // sceneInstruction.enabled = false;
            // instructionBg.enabled = false;
            instructionCanvas.enabled=false; 
            ResumeGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartGame();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void SwitchToVictoryScene()
    {
        resetTimeScale();
        // TODO:
        instructionBg.enabled = false;
        sceneInstruction.enabled = false;

        reportMapStatisticsAndReset();
        SceneManager.LoadScene("Victory");
        SceneManager.sceneLoaded += OnVictorySceneLoaded;
    }

    void OnVictorySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (player1Score == WinningScore || (currScene == "Tutorial" && winnerName == "Player 1"))
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
        if (currScene == "Tutorial")
        {
            spawnPosition = new Vector3(-20, 2, 0);
        }
        else
        {
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
        if (currScene == "Tutorial")
        {
            spawnPosition = new Vector3(20, 2, 0);
        }
        else
        {
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
        StartCoroutine(PlayerDeathSequence(player));
    }

    IEnumerator PlayerDeathSequence(Player player)
    {
        isSlowMotion = true;
        updateTimeScale(0);

        yield return new WaitForSecondsRealtime(2);

        isSlowMotion = false;
        updateTimeScale(0);
        ChangeSceneOnPlayerDies();

    }

    void ChangeSceneOnPlayerDies()
    {
        if (player1Instance.remainingLives <= 0)
        {
            player2Score += 1;
            currentMapStats.Player2Wins = 1;
            if (currScene == "Tutorial")
            {
                winnerName = "Player 2";
                SwitchToVictoryScene();
                return;
            }
        }
        else if (player2Instance.remainingLives <= 0)
        {
            player1Score += 1;
            currentMapStats.Player1Wins = 1;
            if (currScene == "Tutorial")
            {
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

        if (currScene != "Tutorial")
        {
            //SwitchToDifferentRandomMap();
            SwitchToSequentialMap();
        }
    }

    private void CheckShowLifeText()
    {
        if (player1Instance == null || player2Instance == null)
        {
            return;
        }
        if ((player1Instance.remainingLives == 2 || player2Instance.remainingLives == 2) && currScene == "Tutorial")
        {
            showLifeLayerText = true;
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
        updateTimeScale(-1);
        currentCountDownTime = COUNT_DOWN_TIME;
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        while (currentCountDownTime > 0)
        {
            if (isGamePaused)
            {
                yield return new WaitForSecondsRealtime(0.1f); ;
            }
            else
            {
                yield return new WaitForSecondsRealtime(0.1f);
                countDownText.text = Mathf.CeilToInt(currentCountDownTime).ToString();
                currentCountDownTime -= 0.1f;
            }

        }

        isCountingDown = false;
        countDownText.text = "";
        updateTimeScale(1);
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
            sceneInstruction = instance.sceneInstruction;
            instructionBg = instance.instructionBg;
            countDownText = instance.countDownText;

            // Update ScoreText.text
            SetPlayerScoreTexts();

            // Destroy the old GameManager instance and set this as the new singleton instance
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        updateTimeScale(-1);
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        updateTimeScale(1);
    }

    /// <summary>
    /// Updates the game's time scale (Time.timeScale).
    /// This method uses a counter to track changes in the game's pause state.
    /// </summary>
    /// <param name="delta">The change in time scale. Pass 1 to resume time flow, or -1 to pause time.</param>
    public void updateTimeScale(int delta)
    {
        timeScale += delta;

        // If the counter is 1, it means there are no active pause states, so resume normal time flow
        if (timeScale == 1)
        {
            if (isSlowMotion)
            {
                Time.timeScale = 0.05f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        // If the counter is not 1, e.g. 0/-1/-2, it means the game is in some form of paused state, so pause time flow
        else
        {
            Time.timeScale = 0f;
        }
    }

    void resetTimeScale()
    {
        timeScale = 1;
        updateTimeScale(0);
    }
}
