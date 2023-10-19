using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public GameObject pistolPowerUpPrefab;
    public Vector2 spawnPosition = new Vector2(0, 30);

    private List<GameObject> spawnedPowerUps = new List<GameObject>();

    public void SpawnPistolPowerUp()
    {
        GameObject spawnedPowerUp = Instantiate(pistolPowerUpPrefab, spawnPosition, Quaternion.identity);
        spawnedPowerUps.Add(spawnedPowerUp);
    }

    public void DestroyAllPowerUps()
    {
        foreach (GameObject powerUp in spawnedPowerUps)
        {
            Destroy(powerUp);
        }

        spawnedPowerUps.Clear();
    }
}
