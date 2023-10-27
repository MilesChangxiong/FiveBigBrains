using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallShooter : Weapon
{
    public FireBullet bulletPrefab;
    public Transform firePoint;

    protected override void Attack()
    {
        Quaternion firingAngle = owningPlayer.currentDirection == 0 ?
                                 Quaternion.Euler(0, 0, 135) :  // If facing left, adjust the angle
                                 Quaternion.Euler(0, 0, 45);   // If facing right, use the default angle

        FireBullet bullet = Instantiate(bulletPrefab, firePoint.position, firingAngle);

        GameReport.Instance.PostDataToFirebase("", new GameEvent("FireBallShot"));
    }
}