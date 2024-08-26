using System;
using Unity.Netcode;
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
                CurrentHealth = 0;
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
                CurrentHealth = MaxHp;
            }
        }
    }
    public EnemyHealth_UI GetEnemyHealth_UI()
    {
        return enemyHealth_UI;
    }
    private void OnEnable()
    {
        if (enemyHealth_UI)
        {
            enemyHealth_UI.gameObject.SetActive(true);
        }

        InitHp(enemyController.EnemyCharacterData);

        currentHealth.OnValueChanged += (prev, newValue) =>
        {
            enemyHealth_UI.SetHpBar(newValue / MaxHp);
            if (newValue <= 0)
            {
                enemyHealth_UI.gameObject.SetActive(false);
            }
        };
    }

    [ClientRpc]
    private void OnEnemyTakeDamageHandler_ClientRpc(AttackDamage attackDamage)
    {
        enemyHealth_UI.SetHpBar(CurrentHealth / MaxHp);
    }

    private void OnDisable()
    {
        OnEnemyTakeDamage -= OnEnemyTakeDamageHandler_ClientRpc;

    }
}
