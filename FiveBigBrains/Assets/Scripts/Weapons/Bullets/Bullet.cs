using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;

    void Update()
    {
        Move();

        if (IsOutsideScreenBounds())
        {
            Destroy(gameObject);
        }
    }

    void Move()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private bool IsOutsideScreenBounds()
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.x < 0 || screenPosition.x > Screen.width ||
            screenPosition.y < 0 || screenPosition.y > Screen.height)
        {
            return true;
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Player playerHit = collision.GetComponent<Player>();

        if (playerHit) // if the bullet hit a player
        {
            playerHit.TakeDamage(1); // deal 1 damage
            Destroy(gameObject); // destroy the bullet

            GameReport.Instance.PostDataToFirebase("", new GameEvent("PistolBulletHit"));
        }
    }
}
