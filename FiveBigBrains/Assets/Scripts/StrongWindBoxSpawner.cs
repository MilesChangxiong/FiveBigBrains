using UnityEngine;
using System.Collections;

public class StrongWindBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public float initialDelay = 0f; 
    public float spawnInterval = 5f; 

    private void Start()
    {
        if (boxPrefab)
            StartCoroutine(SpawnBoxes());
    }

    private IEnumerator SpawnBoxes()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            Instantiate(boxPrefab, transform.position, Quaternion.identity);
            spawnInterval /= 2;
            if (spawnInterval < 1f)
                spawnInterval = 1f;

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}


