using UnityEngine;

public abstract class WeaponBase : ScriptableObject
{
    [Header("Weapon Details")]
    public ulong WeaponId;
    public PlayerCharacterData HolderCharacterData { get; private set; }

    [Header("Attack Config")]
    public float AttackPoint;
    public WeaponType WeaponType;
    public bool IsReadyToAttack;
    public float AttackTimeInterval;

    public abstract AttackDamage GetDamage(float damageMultiplier);
    public void EquipWeaponTo(PlayerCharacterData holder)
    {
        HolderCharacterData = holder;
    }

}

public enum WeaponType
{
    Sword,
    Bow,
    MagicItem
}
