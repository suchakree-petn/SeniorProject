using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityHealth : MonoBehaviour
{
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public virtual void TakeDamage(Damage damage,EntityCharacterData target)
    {
        if (currentHealth > 0)
        {
            currentHealth -= CalcDamageRecieve(damage,target);
        }
    }
    public virtual float CalcDamageRecieve(Damage damage,EntityCharacterData target)
    {
        return damage.damage - CalcDefense(target);
    }
    public virtual float CalcDefense(EntityCharacterData target)
    {
        return target.DefenseBase + target.DefenseBonus;
    }
    public virtual void InitHp(EntityCharacterData target)
    {
        currentHealth = target.GetMaxHp();
    }
}
