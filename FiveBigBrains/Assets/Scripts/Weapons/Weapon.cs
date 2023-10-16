using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float damage = 1f;
    public float attackRate = 1f;
    protected float nextAttackTime = 0;
    public List<string> headTags = new List<string> { "Head3", "Head2", "Head1"};
    public Player owningPlayer;
    

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
