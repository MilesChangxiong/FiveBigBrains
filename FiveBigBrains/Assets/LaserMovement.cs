using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour
{
    public float speed = 5f;  // 控制移动的速度
    public float distance = 3f;  // 控制左右移动的距离

    private float originalX;  // 存储游戏对象最初的x坐标

    void Start()
    {
        originalX = transform.position.x;  // 获取游戏对象最初的x坐标
    }

    void Update()
    {
        // 使用Mathf.PingPong函数实现往复移动
        float x = originalX + Mathf.PingPong(Time.time * speed, distance);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}

