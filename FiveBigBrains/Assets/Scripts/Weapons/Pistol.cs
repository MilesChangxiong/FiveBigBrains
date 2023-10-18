using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public Bullet bulletPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        Bullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (owningPlayer.currentDirection == 0)
        {
            bullet.speed *= -1;
        }

        GameReport.Instance.PostDataToFirebase("", new GameEvent("PistolBulletShot"));
    }

    private void Start()
    {

    }
}
