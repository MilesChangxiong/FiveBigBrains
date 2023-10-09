using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }

    private void Start()
    {

    }
}
