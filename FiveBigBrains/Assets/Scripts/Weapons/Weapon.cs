using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage = 1f;
    public float attackRate = 1f;
    public int currentAmmo = 0;
    public Player owningPlayer;

    protected float nextAttackTime = 0;
    

    // This method checks if the weapon can attack and if it can, it triggers the Attack method.
    // change return type to know if we can attack
    public bool TryAttack()
    {
        if (CanAttack())
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
            currentAmmo--;

            if (currentAmmo == 0)
            {
                OutOfAmmo();
            }
            return true;
        } else {
            return false;
        }
    }

    void OutOfAmmo()
    {
        Destroy(gameObject);
        owningPlayer.initializePlayerWeapon();
    }

    protected abstract void Attack();

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }
}
