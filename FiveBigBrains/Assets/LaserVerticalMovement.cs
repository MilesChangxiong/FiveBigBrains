using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserVerticalMovement : MonoBehaviour
{
    public float speed = 5f;  
    public float distance = 3f;  

    private float originalY;  // 存储游戏对象最初的y坐标

    void Start()
    {
        originalY = transform.position.y;  // 获取游戏对象最初的y坐标
    }

    void Update()
    {
        float y = originalY + Mathf.PingPong(Time.time * speed, distance);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}

