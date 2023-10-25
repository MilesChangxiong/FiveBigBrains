using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyBullet : MonoBehaviour
{
    public float speed = 10f;
    public float magnificationFactor = 2.5f;

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
        IronBox box = collision.gameObject.GetComponent<IronBox>();
        if (box)
        {
            box.Magnify(magnificationFactor);
            Destroy(gameObject);
        }
    }
}
