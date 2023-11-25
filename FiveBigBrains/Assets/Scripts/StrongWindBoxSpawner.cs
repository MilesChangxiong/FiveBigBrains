using UnityEngine;
using System.Collections;

public class StrongWindBoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab; // ����Ϊ�������Ԥ�Ƽ�
    public float initialDelay = 0f; // ��ʼ����ǰ���ӳ�ʱ��
    public float spawnInterval = 5f; // ����������֮��ļ��ʱ��

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

