using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Weapon
{   

    protected float spearSpeed = 500f;
    public Transform spearTransform;

    

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
    
        // when spear collide with ballon, destroy the ballon only

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
}
