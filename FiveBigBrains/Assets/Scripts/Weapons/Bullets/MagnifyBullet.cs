using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyBullet : Bullet
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

            owningPlayer.currentWeapon.ReportWeaponAction("HitIronBox");

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("IronBall"))
        {
            // enlarge it
            collision.gameObject.transform.localScale *= 1.5f;
            // destroy SpringJoint2D
            SpringJoint2D springJoint = collision.gameObject.GetComponent<SpringJoint2D>();
            if (springJoint)
                Destroy(springJoint);
            // find parent object
            Transform parentTransform = collision.gameObject.transform.parent;
            if (parentTransform)
            {
                Transform ropeTransform = parentTransform.Find("Rope");
                if (ropeTransform)
                {
                    Destroy(ropeTransform.gameObject);
                }
            }

            owningPlayer.currentWeapon.ReportWeaponAction("HitIronBall");

            Destroy(gameObject);
        }
    }
}
