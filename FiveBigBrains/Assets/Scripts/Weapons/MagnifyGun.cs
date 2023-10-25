using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyGun : Weapon
{
    public MagnifyBullet bulletPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
