using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{   

    protected float spearSpeed = 16f;
    public Transform spearTransform;

    private bool hasTriggeredDamage = false;

    private void Awake()
    {
        attackCD = 0.5f;
    }


    protected override void Attack()
    {
        hasTriggeredDamage = false;
        owningPlayer.isSpearAttacking = true;
        if (spearTransform == null)
        {
            spearTransform = transform;
        }

        gameObject.layer = LayerMask.NameToLayer("SpearAttacking");

        StartCoroutine(MoveSpear());
    }

    IEnumerator MoveSpear()
    {
        float speed = spearSpeed;
        float fractionOfJourney = 0.0f;
        Vector3 localStartPosition = spearTransform.localPosition;
        Vector3 direction = owningPlayer.currentDirection == 0 ? Quaternion.Euler(0, 180, 180) * spearTransform.right : spearTransform.right;
        Vector3 localEndPosition = localStartPosition + direction * 5; // Use the direction vector to determine the end position

        // Move forward
        while (fractionOfJourney < 1.0f)
        {
            fractionOfJourney += speed * Time.deltaTime;
            spearTransform.localPosition = Vector3.Lerp(localStartPosition, localEndPosition, fractionOfJourney);
            yield return null;
        }
        spearTransform.localPosition = localEndPosition;

        fractionOfJourney = 0.0f;  // Reset for backward movement

        // Move backward
        while (fractionOfJourney < 1.0f)
        {
            fractionOfJourney += speed * Time.deltaTime;
            spearTransform.localPosition = Vector3.Lerp(localEndPosition, localStartPosition, fractionOfJourney);
            yield return null;
        }
        spearTransform.localPosition = localStartPosition;

        gameObject.layer = LayerMask.NameToLayer("SpearDefault");

        owningPlayer.isSpearAttacking = false;
    }




    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggeredDamage)
        {
            return;
        }

        if (collision.gameObject.tag == "Head")
        {
            owningPlayer.opponent.TakeDamage(1);
            hasTriggeredDamage = true;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Mirror")) // if the spear hit a mirror
        {
            Destroy(collision.gameObject); // destroy the mirror
        }
    }
}
