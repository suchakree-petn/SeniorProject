using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkSingleton<EnemyManager>
{
    // public float currentHp;
    // public EnemyData enemyData;
    // [Header("Dead Setting")]
    // public Action<GameObject> OnEnemyDead;
    // public Action<GameObject, DamageDeal> OnEnemyTakeDamage;
    // public AnimationClip enemyDeadClip;
    // public override EnemyData GetCharactorData()
    // {
    //     return enemyData;
    // }

    // public override void TakeDamage(DamageDeal damage)
    // {
    //     float damageDeal = 0;
    //     if (currentHp > 0)
    //     {
    //         EnemyData enemyData = GetCharactorData();
    //         damageDeal = CalcDamageRecieve(enemyData, damage);
    //         currentHp -= damageDeal;
    //     }
    //     OnEnemyTakeDamage?.Invoke(gameObject, damage);
    // }
    // public void InitHp()
    // {
    //     currentHp = GetCharactorData().GetMaxHp();
    // }


    // public override void Dead(GameObject deadCharactor)
    // {
    //     GameController.Instance.RemoveEnemyDead(deadCharactor);
    // }
    // public override void CheckDead(GameObject charactor, Elemental damage)
    // {
    //     //Debug.Log("CheckDead");
    //     if (charactor.GetComponent<EnemyManager>().currentHp <= 0)
    //     {
    //         OnEnemyDead?.Invoke(charactor);
    //     }
    // }
    protected override void InitAfterAwake()
    {
    }
}
