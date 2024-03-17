using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Weapon_MagicItem_DefaultMagicItem", menuName = "Items/Weapons/Magic Item")]
public class Default_MagicItem : MagicItemBase
{

    public override AttackDamage GetDamage(float damageMultiplier, PlayerCharacterData HolderCharacterData, long clientId = -1)
    {
        float playerAttack = HolderCharacterData.GetAttack();
        AttackDamage attackDamage = new(damageMultiplier, playerAttack, DamageType.Projectile, true, clientId);
        return attackDamage;
    }
}
