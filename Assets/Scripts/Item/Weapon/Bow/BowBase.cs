using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BowBase : WeaponBase
{
    public float chargeDuration;

    [Header("Bow Weapon References")]
    [SerializeField] private Transform arrow_prf;

    public Transform GetArrow(Vector3 position = default, Transform parent = default, Quaternion quaternion = default)
    {
        return Instantiate(arrow_prf, position, quaternion, parent);
    }
}
