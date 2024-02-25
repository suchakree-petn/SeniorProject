using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDeal
{
   public float _damage;
   public float knockbackGaugeDeal;
   public CharactorData _attackerData;
   public GameObject attacker;
   public LayerMask targetLayer;
   public DamageDeal(float damage, GameObject attacker, CharactorData attackerData, LayerMask targetLayer)
   {
      this._damage = damage;
      this.attacker = attacker;
      this._attackerData = attackerData;
      this.targetLayer = targetLayer;
   }
   public static DamageDeal DamageCalculation(GameObject attacker, float _baseSkillDamageMultiplier, LayerMask targetLayer, float knockbackGaugeDeal)
   {
      CharactorData _attackerData;
      if (attacker.tag == "Player")
      {
         _attackerData = attacker.GetComponent<CharactorManager<PlayerData>>().GetCharactorData();
      }
      else
      {
         _attackerData = attacker.GetComponent<CharactorManager<EnemyData>>().GetCharactorData();
      }

      return new DamageDeal(CalcAttack(_attackerData), attacker, _attackerData, targetLayer);
   }

   static private float CalcAttack(CharactorData attacker)
   {
      return attacker._attackBase + attacker._attackBonus;
   }
}
