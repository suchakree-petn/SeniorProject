using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Item_Weapon_Bow_TestBow", menuName = "Items/Weapons/Bow")]
public class Test_Bow : BowBase
{
    public override AttackDamage GetDamage(float damageMultiplier)
    {
        AttackDamage attackDamage = new(damageMultiplier, HolderCharacterData,DamageType.Projectile,true);
        return attackDamage;
    }


}
