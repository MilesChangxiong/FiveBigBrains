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
        // TODO: Potential hint?
        //if (owningPlayer.controlType == Player.PlayerControlType.ARROW_KEYS)
        //    firePoint.Rotate(Vector3.up, 180);
    }

    private void Start()
    {

    }
}
