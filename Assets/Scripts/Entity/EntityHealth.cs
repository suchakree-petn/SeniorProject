using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EntityHealth : NetworkBehaviour
{
    public NetworkVariable<float> currentHealth = new(1337, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public float CurrentHealth => currentHealth.Value;
    private float maxHp;
    public float MaxHp => maxHp;
    public bool IsDead => CurrentHealth <= 0;


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
        if (damage.Damage - defense < 0) return 0;
        return damage.Damage - defense;
    }

    public virtual void InitHp(EntityCharacterData target)
    {
        maxHp = target.GetMaxHp();
        currentHealth = new(MaxHp, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

}
