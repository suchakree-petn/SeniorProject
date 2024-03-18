using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerWeapon : NetworkBehaviour
{
    public Action OnUseWeapon;

    public abstract void UseWeapon(InputAction.CallbackContext context);
    public GameObject GetWeaponInHand()
    {
        Transform weapon = InHand_weaponHolderTransform.GetChild(0);
        // weapon.SetLocalPositionAndRotation(default, default);
        return weapon.gameObject;
    }

    public abstract void NormalAttack();

    [Header("Base Reference")]
    [SerializeField] protected Transform InHand_weaponHolderTransform;
    [SerializeField] protected PlayerController playerController;
}
