using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage = 1f;
    public float attackRate = 1f;
    protected float nextAttackTime = 0;

    // This method checks if the weapon can attack and if it can, it triggers the Attack method.
    public void TryAttack()
    {
        if (CanAttack())
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    protected abstract void Attack();

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }
}
