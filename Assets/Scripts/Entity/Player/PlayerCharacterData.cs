using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCharacterData_", menuName = "EntityCharacterData/PlayerCharacterData")]
public class PlayerCharacterData : EntityCharacterData
{
    public ulong OwnerUserId;
    public PlayerRole PlayerRole;
    public override float GetMaxHp()
    {
        return HpBase + HpBonus;
    }
    public override float GetAttack()
    {
        return AttackBase + AttackBonus;
    }
}
public enum PlayerRole
{
    FrontLine,
    DamageDealer,
    Supporter
}
