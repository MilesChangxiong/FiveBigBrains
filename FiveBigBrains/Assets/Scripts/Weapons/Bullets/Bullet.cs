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

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player playerHit = collision.GetComponent<Player>();

        if (playerHit) // if the bullet hit a player
        {
            playerHit.TakeDamage(1); // deal 1 damage
            Destroy(gameObject); // destroy the bullet
        }
    }
}
