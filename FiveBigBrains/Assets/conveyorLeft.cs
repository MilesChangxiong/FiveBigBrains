using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class conveyorLeft : MonoBehaviour
{
    public float speed = 2f;

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody; 

        if (rb != null && !rb.isKinematic)
        {
            Vector2 direction = -1.0 *transform.right * speed;
            rb.velocity = new Vector2(direction.x, rb.velocity.y); 
        }
    }
}
