using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage = 10f;
    public float fireRate = 1f;
    protected float nextFireTime = 0;

    public abstract void Fire();

    protected bool CanFire()
    {
        return Time.time >= nextFireTime;
    }
}
