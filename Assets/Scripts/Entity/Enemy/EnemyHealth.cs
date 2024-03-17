
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    [Header("Enemy Reference")]
    [SerializeField] protected EnemyController enemyController;

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
        float maxHp = enemyController.EnemyCharacterData.GetMaxHp();
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
