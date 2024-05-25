using System.Collections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class CasterAbility_PowerUp : PlayerAbility
{
    public ulong UserClientId;
    public CasterAbilityData_PowerUp AbilityData;

    private GameObject activeVFX;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{AbilityData.Name} activated");
        Caster_PlayerController playerController = GetComponent<Caster_PlayerController>();
        UserClientId = playerController.OwnerClientId;

        if (activeVFX != null)
        {
            Destroy(activeVFX);
        }

        Caster_PlayerWeapon caster_PlayerWeapon = playerController.GetCaster_PlayerWeapon();
        if (caster_PlayerWeapon.currentLockTargetTransform == null) return;
        ulong targetClientId = caster_PlayerWeapon.GetCurrentLockTargetClientId();
        SpawnVFX(caster_PlayerWeapon.currentLockTargetTransform);
        SpawnVFX_ServerRpc(targetClientId, UserClientId);

        AbilityUIManager.Instance.OnUseAbility_Q?.Invoke(AbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), AbilityData.Cooldown);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnVFX_ServerRpc(ulong targetClientId, ulong userClientId)
    {
        NetworkObject networkObject = PlayerManager.Instance.PlayerGameObjects[targetClientId].GetComponent<NetworkObject>();
        StartBuffEffect_ClientRpc(targetClientId, networkObject);
        SpawnVFX_ClientRpc(networkObject, userClientId);

    }
    [ClientRpc]
    private void SpawnVFX_ClientRpc(NetworkObjectReference networkObjectReference, ulong userClientId)
    {
        if (NetworkManager.LocalClientId == userClientId) return;
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            SpawnVFX(networkObject.transform);
            StartCoroutine(ActiveShield(networkObject.GetComponent<PlayerController>().PlayerCharacterData));
        }

    }
    private void SpawnVFX(Transform target)
    {
        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, target);
        vfxTransform.localPosition = new(0, AbilityData.VFXOffset, 0);
        activeVFX = vfxTransform.gameObject;
    }

    [ClientRpc]
    private void StartBuffEffect_ClientRpc(ulong targetClientId, NetworkObjectReference networkObject)
    {

        if (NetworkManager.LocalClientId != targetClientId) return;
        if (networkObject.TryGet(out NetworkObject networkObj))
        {
            networkObj.GetComponent<PlayerController>().PlayerCharacterData.AttackBonus += AbilityData.AttackBonus;
        }
    }
    private IEnumerator ActiveShield(PlayerCharacterData playerCharacterData)
    {
        yield return new WaitForSeconds(AbilityData.BuffDuration);
        playerCharacterData.AttackBonus -= AbilityData.AttackBonus;
        Destroy(activeVFX);
    }
}
