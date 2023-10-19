using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public GameObject pistolPowerUpPrefab;
    public Vector2 spawnPosition = new Vector2(0, 30);

    public void SpawnPistolPowerUp()
    {
        Instantiate(pistolPowerUpPrefab, spawnPosition, Quaternion.identity);
    }
}
