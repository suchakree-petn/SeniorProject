using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EntityHealth : NetworkBehaviour
{
    [SerializeField] protected NetworkVariable<float> currentHealth;
    public float CurrentHealth => currentHealth.Value;



    public virtual void TakeDamage(AttackDamage damage, float defense)
    {

        float damageTook = CalcDamageRecieve(damage, defense);
        currentHealth.Value -= damageTook;
        Debug.Log($"Entity {name} took {damageTook} damage");
    }
    public virtual void TakeHeal(AttackDamage damage)
    {
        currentHealth.Value += damage.Damage;
        Debug.Log($"Entity {name} took {damage.Damage} heal");
    }
    public virtual float CalcDamageRecieve(AttackDamage damage, float defense)
    {
        return damage.Damage - defense;
    }

    public virtual void InitHp(EntityCharacterData target)
    {
        currentHealth = new(target.GetMaxHp(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

}
