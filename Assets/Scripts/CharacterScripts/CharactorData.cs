using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class CharactorData : ScriptableObject
{
   [Header("Info")]
   public string _name;
   public int _level;
   public LayerMask targetLayer;

   [Header("Movement")]
   public float _moveSpeed;

   [Header("Health Point")]
   public float _hpBase;
   public float _hpBonus;

   [Header("Attack Point")]
   public float _attackBase;
   public float _attackBonus;

   [Header("Defense Point")]
   public float _defenseBase;
   public float _defenseBonus;

   public float GetMaxHp(){
      return _hpBase + _hpBonus;
   }
   public float GetAttack(){
      return _attackBase + _attackBonus;
   }

}