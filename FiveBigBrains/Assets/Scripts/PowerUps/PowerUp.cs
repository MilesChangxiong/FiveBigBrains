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
            Debug.Log("Player collected the power-up!");
            ActivatePowerUp(col.gameObject.GetComponent<Player>());
            Destroy(gameObject);
        }
    }
}
