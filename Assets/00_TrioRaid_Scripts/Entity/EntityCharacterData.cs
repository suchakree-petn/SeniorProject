using UnityEngine;

public abstract class EntityCharacterData : ScriptableObject
{
    [Header("Info")]
    public string Name;
    public int Level;
    public LayerMask TargetLayer;

    [Header("Movement")]
    public float MoveSpeed;

    [Header("Health Point")]
    public float HpBase;
    public float HpBonus;

    [Header("Attack Point")]
    public float AttackBase;
    public float AttackBonus;

    [Header("Defense Point")]
    public float DefenseBase;
    public float DefenseBonus;

    public abstract float GetMaxHp();
    public abstract float GetAttack();
    public abstract float GetDefense();

}