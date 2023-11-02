using UnityEngine;
using Proyecto26;

public static class GameEnvironment
{
    public const string UnityEditor = "UnityEditor";
    public const string WebGL = "WebGL";
}

[System.Serializable]
public class GameEventBase
{
    public string Environment;

    public GameEventBase()
    {
#if UNITY_EDITOR
        Environment = GameEnvironment.UnityEditor;
#elif UNITY_WEBGL
            Environment = GameEnvironment.WebGL;
#endif
    }
}

[System.Serializable]
public class MapEvent : GameEventBase
{
    public string mapName;
    public MapStatistics statistics;

    public MapEvent(string mapName, MapStatistics statistics)
    {
        this.mapName = mapName;
        this.statistics = statistics;
    }
}

[System.Serializable]
public class WeaponEvent : GameEventBase
{
    public string weaponName;
    public bool isFreezed;
    public bool isOpponentTaunted;
    public string eventType; // "bulletShot" or "bulletHit"

    public WeaponEvent(string weaponName, bool isFreezed, bool isOpponentTaunted, string eventType)
    {
        this.weaponName = weaponName;
        this.isFreezed = isFreezed;
        this.isOpponentTaunted = isOpponentTaunted;
        this.eventType = eventType;
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

    public void PostDataToFirebase(string path, object data)
    {
        RestClient.Post(databaseURL + path + ".json", data);
    }
}
