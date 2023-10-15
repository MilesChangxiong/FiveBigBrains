using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{   

    protected float spearSpeed = 500f;
    public Transform spearTransform;
    public List<string> headTags = new List<string>{"Head3", "Head2", "Head1"};
    private int collideTimes = 0; // avoid destroying three heads simultaneously




    protected override void Attack()
    {
        collideTimes = 0;
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

    // When spear collide with head, destroy the head only
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collideTimes += 1;
        if (collideTimes == 1 && headTags.Count > 0)
        {
            string headTag = headTags[0];
            if (collision.gameObject.CompareTag(headTag))
            {
                Transform parentTrans = collision.gameObject.transform.parent;
                Player playerHit = parentTrans.gameObject.GetComponent<Player>();

                if (playerHit)
                {
                    playerHit.TakeDamage(1);
                    Destroy(collision.gameObject);
                }
            }
            headTags.RemoveAt(0);
      
        }
    }
}
