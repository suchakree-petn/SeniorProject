using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerWeapon : NetworkBehaviour
{
    public Action<ServerRpcParams> OnUseWeapon;
    public bool IsReadyToUse = true;
    protected Coroutine weaponCooldown;

    [Header("Base Reference")]
    [SerializeField] protected Transform InHand_weaponHolderTransform;
    [SerializeField] protected PlayerController playerController;

    public abstract void UseWeapon(InputAction.CallbackContext context);
    public virtual void StartWeaponCooldown(float sec)
    {
        if (weaponCooldown == null)
        {
            weaponCooldown = StartCoroutine(WaitForWeaponCooldown(sec));
        }
        else
        {
        }
    }

    private IEnumerator WaitForWeaponCooldown(float sec)
    {

        IsReadyToUse = false;
        yield return new WaitForSeconds(sec);
        IsReadyToUse = true;
        weaponCooldown = null;
    }
    public GameObject GetWeaponInHand()
    {
        Transform weapon = InHand_weaponHolderTransform.GetChild(0);
        // weapon.SetLocalPositionAndRotation(default, default);
        return weapon.gameObject;
    }

    public abstract void NormalAttack();

    protected abstract void OnEnable();
    protected virtual void OnDisable()
    {
        OnUseWeapon = null;
        if (weaponCooldown != null)
        {
            StopCoroutine(weaponCooldown);
            weaponCooldown = null;
        }
    }




}
