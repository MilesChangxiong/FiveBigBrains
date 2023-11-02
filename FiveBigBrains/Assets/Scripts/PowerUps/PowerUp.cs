using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public abstract void ActivatePowerUp(Player player);

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            ActivatePowerUp(col.gameObject.GetComponent<Player>());

            // Data-report related logic
            if (GameManager.instance != null && GameManager.instance.currentMapStats != null)
            {
                GameManager.instance.currentMapStats.PowerupPickupCount += 1;
            }

            Destroy(gameObject);
        }
    }
}
