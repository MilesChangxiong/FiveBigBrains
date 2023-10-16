using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{   

    protected float spearSpeed = 500f;
    public Transform spearTransform;
    private bool destroySucc = false;

    private void Update()
    {
        if (destroySucc)
        {
            headTags.RemoveAt(0);
            destroySucc = false;
        }
    }


    protected override void Attack()
    {
        if (spearTransform == null)
        {
            spearTransform = transform;
        }

        // print current direction
        print("forward: " + owningPlayer.currentDirection);

        // move the spear in current direction by 1, then sleep 0.3s, then backward by 1
        if(owningPlayer.currentDirection == 1) {
            spearTransform.position += spearTransform.right * 5;
        } else {
            spearTransform.position -= spearTransform.right * 5;
        }

        
        StartCoroutine(Backward());

    }

    // Backward() is a coroutine that moves the spear backward by 1 after 0.3s
    IEnumerator Backward()
    {
        print("backward: " + owningPlayer.currentDirection);
        yield return new WaitForSeconds(0.05f);
        
        if(owningPlayer.currentDirection == 1) {
            spearTransform.position -= spearTransform.right * 5;
        } else {
            spearTransform.position += spearTransform.right * 5;
        }

    }


    //When spear collide with head, destroy the head only
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (headTags.Count > 0)
        {
            string headTag = headTags[0];

            foreach (ContactPoint2D contact in collision.contacts)
            {
                GameObject collidedObject = contact.collider.gameObject;
                if (collidedObject.CompareTag(headTag))
                {

                    Transform parentTrans = collidedObject.transform.parent;
                    Player playerHit = parentTrans.gameObject.GetComponent<Player>();

                    if (playerHit)
                    {
                        playerHit.TakeDamage(1);
                        Destroy(collision.gameObject);
                        destroySucc = true;
                    }
                    break;
                }
            }
        }
    }

}
