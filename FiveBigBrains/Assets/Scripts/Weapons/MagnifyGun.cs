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
        float finalAngle = (owningPlayer.currentDirection == 1) ? angle : 180 - angle; // Flip angle if direction is left
        firePoint.eulerAngles = new Vector3(0, 0, finalAngle);

        // update position of Line Renderer
        Vector3 endPosition = firePoint.position + firePoint.right * lineLength;
        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPosition);
    }
}
