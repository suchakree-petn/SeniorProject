using System;
using UnityEngine;

public class EnemyHealth : EntityHealth
{
    [Header("Enemy Reference")]
    [SerializeField] protected EnemyController enemyController;
    [SerializeField] protected EnemyHealth_UI enemyHealth_UI;

    public Action<AttackDamage> OnEnemyTakeDamage;
    public override void TakeDamage(AttackDamage damage, float defense)
    {
        if (CurrentHealth > 0)
        {
            base.TakeDamage(damage, defense);

            if (CurrentHealth < 0)
            {
                currentHealth.Value = 0;
            }
            OnEnemyTakeDamage?.Invoke(damage);
        }
    }

    public override void TakeHeal(AttackDamage damage)
    {
        if (CurrentHealth < MaxHp)
        {
            base.TakeHeal(damage);

            if (CurrentHealth > MaxHp)
            {
                currentHealth.Value = MaxHp;
            }
        }
    }
    private void OnEnable()
    {
        InitHp(enemyController.EnemyCharacterData);
        OnEnemyTakeDamage += (AttackDamage attackDamage) => enemyHealth_UI.SetHpBar(CurrentHealth/MaxHp);
    }
    private void OnDisable()
    {

    }
}
