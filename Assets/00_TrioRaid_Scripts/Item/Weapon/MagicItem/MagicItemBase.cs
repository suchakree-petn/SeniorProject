using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicItemBase : WeaponBase
{
    public float NormalAttack_HealMultiplier;

    [Header("Bow Weapon References")]
    [SerializeField] private Transform healOrb_prf;

    public Transform GetHealOrb(Vector3 position = default, Transform parent = default, Quaternion quaternion = default)
    {
        return Instantiate(healOrb_prf, position, quaternion, parent);
    }
}
