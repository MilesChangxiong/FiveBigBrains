using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    public float speed = 5f;  // �����ƶ����ٶ�
    public float distance = 3f;  // ���������ƶ��ľ���

    private float originalX;  // �洢��Ϸ���������x����

    void Start()
    {
        originalX = transform.position.x;  // ��ȡ��Ϸ���������x����
    }

    void Update()
    {
        // ʹ��Mathf.PingPong����ʵ�������ƶ�
        float x = originalX + Mathf.PingPong(Time.time * speed, distance);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}

