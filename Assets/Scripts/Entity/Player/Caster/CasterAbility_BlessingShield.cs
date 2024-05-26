using System.Collections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class CasterAbility_BlessingShield : PlayerAbility
{
    public ulong UserClientId;
    public CasterAbilityData_BlessingShield AbilityData;

    private GameObject activeShield;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{AbilityData.Name} activated");
        Caster_PlayerController playerController = GetComponent<Caster_PlayerController>();
        UserClientId = playerController.OwnerClientId;
        
        if (activeShield != null)
        {
            Destroy(activeShield);
        }
        Caster_PlayerWeapon caster_PlayerWeapon = playerController.GetCaster_PlayerWeapon();

        if (caster_PlayerWeapon.currentLockTargetTransform == null) return;
        playerController.playerAnimation.SetTriggerNetworkAnimation("BlessingShield");
        ulong targetClientId = caster_PlayerWeapon.GetCurrentLockTargetClientId();
        SpawnShield(caster_PlayerWeapon.currentLockTargetTransform);
        SpawnShield_ServerRpc(targetClientId, UserClientId);

        AbilityUIManager.Instance.OnUseAbility_E?.Invoke(AbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), AbilityData.Cooldown);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnShield_ServerRpc(ulong targetClientId, ulong userClientId)
    {
        NetworkObject networkObject = PlayerManager.Instance.PlayerGameObjects[targetClientId].GetComponent<NetworkObject>();
        StartShieldEffect_ClientRpc(targetClientId, networkObject);
        SpawnShield_ClientRpc(networkObject, userClientId);

    }
    [ClientRpc]
    private void SpawnShield_ClientRpc(NetworkObjectReference networkObjectReference, ulong userClientId)
    {
        if (NetworkManager.LocalClientId == userClientId) return;
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            SpawnShield(networkObject.transform);
            StartCoroutine(ActiveShield(networkObject.GetComponent<PlayerController>().PlayerCharacterData));

        }

    }
    private void SpawnShield(Transform target)
    {
        Debug.Log($"Spawn Shield");
        Transform shieldTransform = Instantiate(AbilityData.Shield_prf, target);
        shieldTransform.localPosition = new(0, AbilityData.ShieldOffset, 0);
        activeShield = shieldTransform.gameObject;
    }

    [ClientRpc]
    private void StartShieldEffect_ClientRpc(ulong targetClientId, NetworkObjectReference networkObject)
    {

        if (NetworkManager.LocalClientId != targetClientId) return;
        if (networkObject.TryGet(out NetworkObject networkObj))
        {
            networkObj.GetComponent<PlayerController>().PlayerCharacterData.DefenseBonus += AbilityData.BonusDefense;
        }
    }
    private IEnumerator ActiveShield(PlayerCharacterData playerCharacterData)
    {
        yield return new WaitForSeconds(AbilityData.ShieldDuration);
        playerCharacterData.DefenseBonus -= AbilityData.BonusDefense;
        Destroy(activeShield);
    }
}
