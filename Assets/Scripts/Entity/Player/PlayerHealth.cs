
using UnityEngine;

public class PlayerHealth : EntityHealth
{
    [Header("Player Reference")]
    [SerializeField] protected PlayerController playerController;

    public override void TakeDamage(AttackDamage damage, float defense)
    {
        if (CurrentHealth > 0)
        {
            base.TakeDamage(damage, defense);

            if (CurrentHealth < 0)
            {
                currentHealth.Value = 0;
            }
        }
    }

    public override void TakeHeal(AttackDamage damage)
    {
        float maxHp = playerController.PlayerCharacterData.GetMaxHp();
        if (CurrentHealth < maxHp)
        {
            base.TakeHeal(damage);

            if (CurrentHealth > maxHp)
            {
                currentHealth.Value = maxHp;
            }
        }
    }
}
