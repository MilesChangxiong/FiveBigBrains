using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyGun : Weapon
{
    public MagnifyBullet bulletPrefab;
    public Transform firePoint;

    private LineRenderer lineRenderer;
    public float lineLength = 5f; // length of line

    protected override void Attack()
    {
        MagnifyBullet bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (owningPlayer.currentDirection == 0)
        {
            bullet.speed *= -1;
        }
    }

    private void Awake()
    {
        isShootingAngleAdjustable = true;
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetShootingAngle(float angle)
    {
        float finalAngle = (owningPlayer.currentDirection == 1) ? angle : -angle;
        firePoint.eulerAngles = new Vector3(0, 0, finalAngle);

        // Use player direction to determine endPosition of the line
        Vector3 direction = (owningPlayer.currentDirection == 1) ? firePoint.right : -firePoint.right;
        Vector3 endPosition = firePoint.position + direction * lineLength;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPosition);
    }
}
