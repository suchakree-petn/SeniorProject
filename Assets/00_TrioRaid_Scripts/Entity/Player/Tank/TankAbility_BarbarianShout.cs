using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class TankAbility_BarbarianShout : PlayerAbility
{
    public TankAbilityData_BarbarianShout AbilityData;
    public LayerMask TargetLayer;
    public AudioSource audioSource;

    protected override void Update()
    {
        base.Update();
    }

    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{AbilityData.Name} activated");
        Tank_PlayerController playerController = GetComponent<Tank_PlayerController>();
        TargetLayer = playerController.PlayerCharacterData.TargetLayer;
        audioSource.Play();
        playerController.PlayerAnimation.SetTriggerNetworkAnimation("Barbarianshout");
        StartCoroutine(ActiveDuration(playerController));

        AbilityUIManager.Instance.OnUseAbility_Q?.Invoke(AbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), AbilityData.Cooldown);
    }

    IEnumerator ActiveDuration(Tank_PlayerController playerController)
    {
        Debug.Log($"Start {AbilityData.Name} duration: {AbilityData.StopMoveDuration}");
        playerController.SetCanPlayerMove(false);
        playerController.PlayerAnimation.SetLayerWeight(1, 0);
        playerController.GetTank_PlayerWeapon().IsReadyToUse = false;

        yield return new WaitForSeconds(AbilityData.StopMoveDuration);

        playerController.PlayerAnimation.SetLayerWeight(1, 1);
        playerController.GetTank_PlayerWeapon().IsReadyToUse = true;

        playerController.PlayerCharacterData.DefenseBonus += AbilityData.DefenseBonus;

        Vector3 origin = transform.position + new Vector3(transform.position.x, transform.position.y + AbilityData.PositionOffset, transform.position.z);
        RaycastHit[] hits = Physics.SphereCastAll(origin, AbilityData.Radius, direction: transform.up, layerMask: TargetLayer, maxDistance: default);
        List<EnemyController> enemyControllers = new();
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root.TryGetComponent(out EnemyController enemyController))
            {
                Debug.Log("Here");
                if (enemyControllers.Contains(enemyController)) continue;
                enemyControllers.Add(enemyController);
                enemyController.StartTaunt_ServerRpc(playerController.NetworkObject);
            }
        }
        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, transform.position, transform.rotation);
        Destroy(vfxTransform.gameObject, AbilityData.VFXDuration);

        SpawnVFX_ServerRpc();
        playerController.SetCanPlayerMove(true);

        yield return new WaitForSeconds(AbilityData.TauntDuration);

        playerController.PlayerCharacterData.DefenseBonus -= AbilityData.DefenseBonus;

        foreach (EnemyController enemyController in enemyControllers)
        {
            if (enemyController == null) continue;

            enemyController.FinishTaunt_ServerRpc();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, transform.position.y + AbilityData.PositionOffset, 0), AbilityData.Radius);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnVFX_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SpawnVFX_ClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void SpawnVFX_ClientRpc(ulong userClientId)
    {
        if (NetworkManager.LocalClientId == userClientId) return;


        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, transform.position, transform.rotation);
        Destroy(vfxTransform.gameObject, AbilityData.VFXDuration);
    }

}
