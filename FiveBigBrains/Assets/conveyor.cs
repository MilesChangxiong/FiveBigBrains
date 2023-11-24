using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class conveyor : MonoBehaviour
{
    public float speed = 10f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // ���Ի�ȡ���������Rigidbody2D���
        Rigidbody2D rb = other.attachedRigidbody; // ʹ��attachedRigidbody����֤��ʹ��ײ�������������ϣ�Ҳ�ܻ�ȡ��Rigidbody2D

        if (rb != null && !rb.isKinematic)
        {
            // Ӧ�����������ٶ�
            Vector2 direction = transform.right * speed;
            rb.velocity = new Vector2(direction.x, rb.velocity.y); // �⽫��x��Ӧ���ٶȣ�����y���ٶȲ���
        }
    }
}

