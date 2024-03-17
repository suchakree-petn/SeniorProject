using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterData_", menuName = "EntityCharacterData/PlayerCharacterData")]
public class PlayerCharacterData : EntityCharacterData
{
    public PlayerRole PlayerRole;
    public override float GetMaxHp()
    {
        return HpBase + HpBonus;
    }
    public override float GetAttack()
    {
        return AttackBase + AttackBonus;
    }

    public override float GetDefense()
    {
        return DefenseBase + DefenseBonus;
    }
}
public enum PlayerRole
{
    FrontLine,
    DamageDealer,
    Supporter
}
