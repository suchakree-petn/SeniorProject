using Unity.Netcode;
using UnityEngine;

public class ArcherAbility_VineTrap : PlayerAbility
{
    public ulong UserClientId;
    public ArcherAbilityData_VineTrap archerAbilityData;

    private GameObject activeVineTrap;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{archerAbilityData.Name} activated");
        Archer_PlayerController playerController = GetComponent<Archer_PlayerController>();
        UserClientId = playerController.OwnerClientId;

        SpawnVineTrap_ServerRpc();

        AbilityUIManager.Instance.OnUseAbility_Q?.Invoke(archerAbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), archerAbilityData.Cooldown);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnVineTrap_ServerRpc()
    {
        if (activeVineTrap != null)
        {
            activeVineTrap.GetComponent<NetworkObject>().Despawn();
            Destroy(activeVineTrap);
        }

        Debug.Log($"Spawn VineTrap");
        Transform vineTrapGO = Instantiate(archerAbilityData.VineTrap_prf, transform.position, Quaternion.identity);
        VineTrap vineTrap = vineTrapGO.GetComponent<VineTrap>();
        vineTrap.StopMoveDuration = archerAbilityData.StopMoveDuration;
        vineTrap.PopOffset = archerAbilityData.PopOffset;

        vineTrapGO.GetComponent<NetworkObject>().SpawnWithOwnership(UserClientId);
        vineTrapGO.GetComponent<Rigidbody>().AddForce(transform.forward * archerAbilityData.ThrowForce, ForceMode.Impulse);

        activeVineTrap = vineTrapGO.gameObject;
    }
}
