using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyCharacterData_", menuName = "EntityCharacterData/EnemyCharacterData")]
public class EnemyCharacterData : EntityCharacterData
{
    public override float GetAttack()
    {
        return AttackBase + AttackBonus;
    }

    public override float GetDefense()
    {
        return DefenseBase + DefenseBonus;
    }

    public override float GetMaxHp()
    {
        return HpBase + HpBonus;
    }
}
