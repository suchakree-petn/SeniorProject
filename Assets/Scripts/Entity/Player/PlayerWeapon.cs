using System;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerWeapon : MonoBehaviour
{

    public abstract void UseWeapon(InputAction.CallbackContext context);
    public GameObject GetWeaponInHand()
    {
        Transform weapon = InHand_weaponHolderTransform.GetChild(0);
        // weapon.SetLocalPositionAndRotation(default, default);
        return weapon.gameObject;
    }

    [Header("Base Reference")]
    [SerializeField] protected Transform InHand_weaponHolderTransform;
    [SerializeField] protected PlayerController playerController;
}
