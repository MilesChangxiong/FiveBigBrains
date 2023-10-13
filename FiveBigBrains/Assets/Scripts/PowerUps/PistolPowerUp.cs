using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPowerUp : PowerUp
{
    public Pistol pistolPrefab;

    private Vector3 offset = new Vector3(0.6f, 0.18f, 0);

    public override void ActivatePowerUp(Player player)
    {
        Destroy(player.currentWeapon);

        Weapon newWeapon = Instantiate(pistolPrefab, player.transform.position + offset, Quaternion.identity, player.transform);
        newWeapon.currentAmmo = 3; // 3 bullets
        newWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.currentWeapon = newWeapon;
        newWeapon.owningPlayer = player;
        Destroy(gameObject);  // Destroy the PowerUp from the scene
    }

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
