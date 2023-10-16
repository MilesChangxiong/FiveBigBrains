using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player player1Prefab;
    public Player player2Prefab;
    
    //change to public to be used in Taunt()
    public static Player player1Instance;
    public static Player player2Instance;

    private void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        player1Instance = Instantiate(player1Prefab, new Vector3(-10, -2, 0), Quaternion.identity);
        player2Instance = Instantiate(player2Prefab, new Vector3(10, -2, 0), Quaternion.Euler(0, 180, 0));

        player1Instance.controlType = Player.PlayerControlType.WASD;
        player1Instance.playerColor = new Color(33 / 255f, 147 / 255f, 255 / 255f, 255 / 255f); // Blue

        player2Instance.controlType = Player.PlayerControlType.ARROW_KEYS;
        player2Instance.playerColor = new Color(255 / 255f, 67 / 255f, 40 / 255f, 255 / 255f); // Red
    }
}
