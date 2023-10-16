using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

}
