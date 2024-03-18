using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Caster_PlayerWeapon : PlayerWeapon
{

    public MagicItemBase MagicItemWeaponData;
    public MagicItemConfig MagicItemConfig = new();
    private bool isHasLockTarget;
    private ulong currentLockTargetClientId;
    private Vector3 currentLockTargetPosition;

    [Header("Caster Reference")]
    [SerializeField] private Transform firePointHolderTransform;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private Caster_PlayerController caster_PlayerController;

    public override void UseWeapon(InputAction.CallbackContext context)
    {
        if (!IsReadyToUse) return;

        if (firePointTransform == null)
        {
            Debug.LogWarning($"No Fire Point transform");
            return;
        }
        if (context.performed)
        {
            NormalAttack();
            OnUseWeapon?.Invoke();
        }


    }

    private void Update()
    {
        if (!IsOwner) return;


        if (playerController.PlayerCameraMode == PlayerCameraMode.Focus)
        {
            RotateFirePointHolder();
        }


    }

    private void LateUpdate()
    {
        if (!IsOwner) return;

        if (GetLockTarget())
        {
            isHasLockTarget = true;
            PlayerUIManager.Instance.SetLockTargetState(true);
            PlayerUIManager.Instance.SetLockTargetPosition(currentLockTargetPosition);
        }
        else
        {
            isHasLockTarget = false;
            PlayerUIManager.Instance.SetLockTargetState(false);
        }
    }
    private bool GetLockTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit[] hits = Physics.SphereCastAll(ray, MagicItemConfig.SphereCastRadius, MagicItemConfig.MaxSphereCastDistance, playerController.PlayerCharacterData.TargetLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.root.TryGetComponent(out PlayerController playerController) && !playerController.IsOwner && hit.collider.isTrigger)
            {
                currentLockTargetClientId = playerController.OwnerClientId;
                currentLockTargetPosition = hit.collider.bounds.center;
                return true;
            }
        }

        return false;
    }

    public override void NormalAttack()
    {
        AttackDamage attackDamage = MagicItemWeaponData.GetDamage(MagicItemWeaponData.NormalAttack_HealMultiplier, playerController.PlayerCharacterData, (long)OwnerClientId);

        LaunchHealOrb_ServerRpc(attackDamage, currentLockTargetClientId, isHasLockTarget);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LaunchHealOrb_ServerRpc(AttackDamage attackDamage, ulong targetClientId, bool isHasLockTarget, ServerRpcParams serverRpcParams = default)
    {
        // ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
        Transform healOrbTransform = MagicItemWeaponData.GetHealOrb(position: firePointTransform.position);
        NetworkObject healOrbNetworkObject = healOrbTransform.GetComponent<NetworkObject>();
        healOrbNetworkObject.Spawn(true);
        healOrbTransform.transform.forward = Camera.main.transform.forward;
        HealOrb healOrb = healOrbTransform.GetComponent<HealOrb>();
        if (isHasLockTarget)
        {
            healOrb.target = PlayerManager.Instance.PlayerGameObjects[targetClientId].transform;
        }
        healOrb.AttackDamage = attackDamage;

    }

    private void RotateFirePointHolder()
    {
        firePointHolderTransform.forward = Camera.main.transform.forward;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currentLockTargetPosition, MagicItemConfig.SphereCastRadius);
    }

    protected override void OnEnable()
    {
        OnUseWeapon += () => StartWeaponCooldown(MagicItemWeaponData.AttackTimeInterval);
    }
}
[Serializable]
public class MagicItemConfig
{
    public float MaxSphereCastDistance;
    public float SphereCastRadius;
}