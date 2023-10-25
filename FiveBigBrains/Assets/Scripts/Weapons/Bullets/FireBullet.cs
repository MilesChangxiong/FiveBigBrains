﻿using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireBullet : MonoBehaviour
{
    public float speed = 10f;
    public float explosionRadius = 2f;
    public LayerMask explosionLayers;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        explosionLayers = LayerMask.GetMask("Default");
        SetInitialVelocity();
    }

    void SetInitialVelocity()
    {
        float initialSpeedY = Mathf.Tan(Mathf.Deg2Rad * 45) * speed;
        Vector2 initialVelocity = new Vector2(speed, initialSpeedY);

        if (transform.rotation.eulerAngles.z > 90)
        {
            initialVelocity.x *= -1;
        }

        rb.velocity = initialVelocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Explode()
    {
        // use OverlapCircleAll to detect objects in explosion area
        Collider2D[] objectsInExplosionRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius, explosionLayers);

        foreach (Collider2D collider in objectsInExplosionRadius)
        {
            Player player = collider.GetComponent<Player>();
            Ice iceBlock = collider.GetComponent<Ice>();

            if (player)
            {
                player.TakeDamage(1);
            }

            if (iceBlock)
            {
                Destroy(iceBlock.gameObject);
            }
        }

        Destroy(gameObject);
    }
}
