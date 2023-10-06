using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public override void Fire()
    {
        if (CanFire())
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Start()
    {
        // JUST FOR TESTING: Shoot 3 times
        Fire();
        Invoke("Fire", fireRate);
        Invoke("Fire", fireRate * 2);
    }
}
