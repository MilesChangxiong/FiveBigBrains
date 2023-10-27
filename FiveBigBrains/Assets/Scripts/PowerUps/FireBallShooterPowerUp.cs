using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooterPowerUp : PowerUp
{
    public FireBallShooter fireBallShooterPrefab;

    private Vector3 offset = new Vector3(0.6f, 0.18f, 0);

    public override void ActivatePowerUp(Player player)
    {
        if (!(player.currentWeapon is Spear))
        {
            return;
        }

        Destroy(player.currentWeapon.gameObject);

        Vector3 adjustedOffset = player.currentDirection == 0 ?
                              new Vector3(-offset.x, offset.y, offset.z) : offset;

        Weapon newWeapon = Instantiate(fireBallShooterPrefab, player.transform.position + adjustedOffset, Quaternion.identity, player.transform);
        newWeapon.currentAmmo = 5;
        newWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.currentWeapon = newWeapon;
        newWeapon.owningPlayer = player;
        Destroy(gameObject);  // Destroy the PowerUp from the scene
    }
}