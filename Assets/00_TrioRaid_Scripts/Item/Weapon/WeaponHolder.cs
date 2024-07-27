using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponHolder<T> : MonoBehaviour where T : WeaponBase
{
    [SerializeField] private T _weapon;
    public T Weapon => _weapon;

    public abstract void UseWeapon();
}
