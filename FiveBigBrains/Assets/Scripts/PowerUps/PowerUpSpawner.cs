using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public float delayBeforeDrop = 0f;

    private void Start()
    {
        if (powerUpPrefab)
            StartCoroutine(DropPowerUp());
    }

    private IEnumerator DropPowerUp()
    {
        yield return new WaitForSeconds(delayBeforeDrop);
        Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
    }
}