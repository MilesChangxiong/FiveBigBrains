using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public PistolBullet bulletPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        PistolBullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.owningPlayer = owningPlayer;
        if (owningPlayer.currentDirection == 0)
        {
            bullet.speed *= -1;
        }
    }

    private void Start()
    {

    }
}
