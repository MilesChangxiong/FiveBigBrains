using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager instance;

    public GameObject pistolPowerUpPrefab;
    public GameObject fireGunPowerUpPrefab;
    public GameObject magnifyGunPowerUpPrefab;
    public Vector2 spawnPosition = new Vector2(0, 30);

    private List<GameObject> spawnedPowerUps = new List<GameObject>();

    public void SpawnPistolPowerUp()
    {
        //GameObject spawnedPowerUp = Instantiate(pistolPowerUpPrefab, spawnPosition, Quaternion.identity);
        //GameObject spawnedPowerUp = Instantiate(fireGunPowerUpPrefab, spawnPosition, Quaternion.identity);
        GameObject spawnedPowerUp = Instantiate(magnifyGunPowerUpPrefab, spawnPosition, Quaternion.identity);
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

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
