using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public GameObject pistolPowerUpPrefab;
    public Vector2 spawnPosition = new Vector2(0, 20);

    private void Start()
    {
        Invoke("SpawnPistolPowerUp", 0f); // TESTING CODE: Drop a pistol after 0s.
    }

    void SpawnPistolPowerUp()
    {
        Instantiate(pistolPowerUpPrefab, spawnPosition, Quaternion.identity);
    }
}
