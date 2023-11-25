using UnityEngine;
using System.Collections;

public class StrongWindBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab; // 设置为你的箱子预制件
    public float initialDelay = 0f; // 开始生成前的延迟时间
    public float spawnInterval = 5f; // 生成新箱子之间的间隔时间

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
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

