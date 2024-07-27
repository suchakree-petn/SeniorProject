using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item_Weapon_Bow_TestBow", menuName = "Items/Weapons/Bow")]
public class Test_Bow : BowBase
{
    public override AttackDamage GetDamage(float damageMultiplier, PlayerCharacterData HolderCharacterData, long clientId = -1)
    {
        float playerAttack = HolderCharacterData.GetAttack();
        AttackDamage attackDamage = new(damageMultiplier, playerAttack, DamageType.Projectile, true, clientId);
        return attackDamage;
    }


}
