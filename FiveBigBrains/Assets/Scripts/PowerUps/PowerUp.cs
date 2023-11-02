using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public abstract void ActivatePowerUp(Player player);

    private bool hasPickedUp = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (hasPickedUp)
        {
            return;
        }

        if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            ActivatePowerUp(player);
            hasPickedUp = true;

            // Data-report related logic
            if (GameManager.instance != null && GameManager.instance.currentMapStats != null)
            {
                GameManager.instance.currentMapStats.PowerupPickupCount += 1;

                if (player.controlType == Player.PlayerControlType.WASD)
                {
                    GameManager.instance.currentMapStats.Player1PowerupPickupCount += 1;
                } else
                {
                    GameManager.instance.currentMapStats.Player2PowerupPickupCount += 1;
                }
            }

            Destroy(gameObject);
        }
    }
}
