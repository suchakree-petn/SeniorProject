using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class TankAbility_GroundSmash : PlayerAbility
{
    public TankAbilityData_GroundSmash AbilityData;
    public LayerMask TargetLayer;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{AbilityData.Name} activated");
        Tank_PlayerController playerController = GetComponent<Tank_PlayerController>();
        TargetLayer = playerController.PlayerCharacterData.TargetLayer;


        StartCoroutine(ActiveDuration(playerController));

        AbilityUIManager.Instance.OnUseAbility_E?.Invoke(AbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), AbilityData.Cooldown);
    }

    IEnumerator ActiveDuration(Tank_PlayerController playerController)
    {

        Debug.Log($"Start {AbilityData.Name} duration: {AbilityData.StopMoveDuration}");
        playerController.SetCanPlayerMove(false);

        yield return new WaitForSeconds(AbilityData.StopMoveDuration);

        Vector3 origin = transform.position + (transform.forward * AbilityData.PositionOffsetX);
        RaycastHit[] hits = Physics.BoxCastAll(origin, new(AbilityData.Radius, 2, AbilityData.Radius), transform.forward);
        List<EnemyController> enemyControllers = new();
        Debug.Log("Hits count: " + hits.Length);
        foreach (RaycastHit hit in hits)
        {
            // if (hit.collider == null) continue;

            if (hit.collider.transform.root.TryGetComponent(out EnemyController enemyController))
            {
                Debug.Log("Here");
                if (enemyControllers.Contains(enemyController)) continue;
                enemyControllers.Add(enemyController);
                enemyController.StartStun_ServerRpc();
                if (enemyController.TryGetComponent(out IDamageable damageable))
                {
                    AttackDamage attackDamage = playerController.GetTank_PlayerWeapon().SwordWeaponData.GetDamage(AbilityData.DamageMultiplier, playerController.PlayerCharacterData, (long)NetworkManager.LocalClientId);
                    damageable.TakeDamage_ServerRpc(attackDamage);
                    SpawnStunVFX(enemyController.NetworkObject);
                    SpawnStunVFX_ServerRpc(enemyController.NetworkObject);
                }
            }
        }
        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, origin, transform.rotation);
        Destroy(vfxTransform.gameObject, AbilityData.VFXDuration);
        SpawnVFX_ServerRpc(origin);
        playerController.SetCanPlayerMove(true);

        yield return new WaitForSeconds(AbilityData.StunDuration);

        foreach (EnemyController enemyController in enemyControllers)
        {
            if (enemyController == null) continue;

            enemyController.FinishStun_ServerRpc();
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 origin = transform.position + (transform.forward * AbilityData.PositionOffsetX);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(origin, new Vector3(AbilityData.Radius, 2, AbilityData.Radius));
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnVFX_ServerRpc(Vector3 origin, ServerRpcParams serverRpcParams = default)
    {
        SpawnVFX_ClientRpc(serverRpcParams.Receive.SenderClientId, origin);
    }

    [ClientRpc]
    private void SpawnVFX_ClientRpc(ulong userClientId, Vector3 origin)
    {
        if (NetworkManager.LocalClientId == userClientId) return;

        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, origin, transform.rotation);
        Destroy(vfxTransform.gameObject, AbilityData.VFXDuration);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnStunVFX_ServerRpc(NetworkObjectReference networkObjectReference, ServerRpcParams serverRpcParams = default)
    {
        SpawnStunVFX_ClientRpc(serverRpcParams.Receive.SenderClientId, networkObjectReference);
    }

    [ClientRpc]
    private void SpawnStunVFX_ClientRpc(ulong userClientId, NetworkObjectReference networkObjectReference)
    {
        if (NetworkManager.LocalClientId == userClientId) return;

        SpawnStunVFX(networkObjectReference);
    }

    private void SpawnStunVFX(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            Transform vfxTransform = Instantiate(AbilityData.StunVFX_prf, networkObject.transform);
            try
            {
                Destroy(vfxTransform.gameObject, AbilityData.StunVFXDuration);
            }
            catch
            {
                Debug.Log("Catch error stun VFX");
            }
        }

    }
}
