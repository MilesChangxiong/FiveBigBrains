using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyor : MonoBehaviour
{
    public float speed = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // 尝试获取触碰物体的Rigidbody2D组件
        Rigidbody2D rb = other.attachedRigidbody; // 使用attachedRigidbody来保证即使碰撞发生在子物体上，也能获取到Rigidbody2D

        if (rb != null && !rb.isKinematic)
        {
            // 应用力或设置速度
            Vector2 direction = transform.right * speed;
            rb.velocity = new Vector2(direction.x, rb.velocity.y); // 这将沿x轴应用速度，保持y轴速度不变
        }
    }
}

