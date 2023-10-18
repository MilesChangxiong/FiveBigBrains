using UnityEngine;
using Proyecto26;

public static class GameEnvironment
{
    public const string UnityEditor = "UnityEditor";
    public const string WebGL = "WebGL";
}

[System.Serializable]
public class GameEvent
{
    public string EventType;
    public string Environment;

    public GameEvent(string EventType)
    {
        this.EventType = EventType;

        #if UNITY_EDITOR
                Environment = GameEnvironment.UnityEditor;
        #elif UNITY_WEBGL
                Environment = GameEnvironment.WebGL;
        #endif
    }
}

public class GameReport : MonoBehaviour
{
    public static GameReport Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string databaseURL = "https://fivebigbrains-usccs526fall2023-default-rtdb.firebaseio.com/";

    public void PostDataToFirebase(string path, GameEvent gameEvent)
    {
        RestClient.Post(databaseURL + path + ".json", gameEvent);
    }
}
