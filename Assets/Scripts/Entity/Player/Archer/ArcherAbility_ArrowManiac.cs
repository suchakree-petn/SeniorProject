using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArcherAbility_ArrowManiac : PlayerAbility
{
    public ArcherAbilityData_ArrowManiac AbilityData;
    public List<Transform> ArrowManiac_List;
    public List<Vector3> ArrowManiac_Destination;
    public Transform ArrowManiacSpawnGroup;
    private LayerMask TargetLayer;

    private GameObject activeVFX;

    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{AbilityData.Name} activated");
        Archer_PlayerController playerController = GetComponent<Archer_PlayerController>();
        TargetLayer = playerController.PlayerCharacterData.TargetLayer;

        if (activeVFX != null)
        {
            Destroy(activeVFX);
        }
        SpawnVFX(transform);
        SpawnVFX_ServerRpc(userClientId);
        StartCoroutine(ActiveDuration(AbilityData.Duration, playerController));

        AbilityUIManager.Instance.OnUseAbility_E?.Invoke(AbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), AbilityData.Cooldown);
    }

    IEnumerator ActiveDuration(float duration, Archer_PlayerController playerController)
    {

        Debug.Log($"Start {AbilityData.Name} duration: {duration}");
        playerController.SetCanPlayerMove(false);
        Archer_PlayerWeapon archer_PlayerWeapon = playerController.GetArcherWeapon();
        archer_PlayerWeapon.OnUseWeapon += SpawnArrowManiac_ServerRpc;

        yield return new WaitForSeconds(duration);

        archer_PlayerWeapon.OnUseWeapon -= SpawnArrowManiac_ServerRpc;
        CancelInvoke(nameof(FireArrowManiac));

        yield return new WaitForSeconds(2);

        ClearOldArrow_ServerRpc();
        ArrowManiac_List.Clear();
        ArrowManiac_Destination.Clear();
        playerController.SetCanPlayerMove(true);
        Debug.Log($"Finish {AbilityData.Name} ");

    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnVFX_ServerRpc(ulong userClientId)
    {
        SpawnVFX_ClientRpc(userClientId);
    }
    [ClientRpc]
    private void SpawnVFX_ClientRpc(ulong userClientId)
    {
        if (NetworkManager.LocalClientId == userClientId) return;

        SpawnVFX(transform);
    }
    private void SpawnVFX(Transform parent)
    {
        Transform vfxTransform = Instantiate(AbilityData.VFX_prf, parent);
        vfxTransform.localPosition = new Vector3(0, AbilityData.VFXOffset, 0);
        activeVFX = vfxTransform.gameObject;

        Destroy(activeVFX,AbilityData.VFXDuration);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void ClearOldArrow_ServerRpc()
    {
        ArrowManiac[] allArrows = FindObjectsOfType<ArrowManiac>(true);
        foreach (ArrowManiac oldArrow in allArrows)
        {
            NetworkObject no = oldArrow.GetComponent<NetworkObject>();
            if (no.IsSpawned)
            {
                no.Despawn();
            }
            Destroy(oldArrow.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnArrowManiac_ServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"Spawn Arrow");
        foreach (Transform child in ArrowManiacSpawnGroup.transform)
        {
            Transform arrow = Instantiate(AbilityData.ArrowManiac_prf, child.position, child.rotation);
            arrow.GetComponent<NetworkObject>().Spawn(true);
            ArrowManiac_List.Add(arrow);
            CalcDirection_ClientRpc(serverRpcParams.Receive.SenderClientId, child.position);
        }
        CancelInvoke(nameof(FireArrowManiac));
        InvokeRepeating(nameof(FireArrowManiac), 0.3f, AbilityData.TimeInterval);
    }

    [ClientRpc]
    private void CalcDirection_ClientRpc(ulong clientId, Vector3 childPos)
    {
        if (NetworkManager.LocalClientId != clientId) return;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        Vector3 direction;
        if (Physics.Raycast(ray, out RaycastHit hit, AbilityData.Distance, TargetLayer, QueryTriggerInteraction.Collide))
        {
            direction = (hit.point - childPos).normalized;
        }
        else
        {
            Vector3 endPoint = Camera.main.transform.position + Camera.main.transform.forward * AbilityData.Distance;
            direction = (endPoint - childPos).normalized;
        }
        SendDirection_ServerRpc(direction);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendDirection_ServerRpc(Vector3 direction)
    {
        ArrowManiac_List[^1].forward = direction;
        ArrowManiac_Destination.Add(direction);

    }
    private void FireArrowManiac()
    {
        if (ArrowManiac_List.Count <= 0)
        {
            CancelInvoke(nameof(FireArrowManiac));
            return;
        }
        Debug.Log($"Fire Arrow");
        Transform arrow = ArrowManiac_List[0];
        ArrowManiac arrowManiac = arrow.GetComponent<ArrowManiac>();
        Archer_PlayerController archer_PlayerController = GetComponent<Archer_PlayerController>();
        arrowManiac.AttackDamage = archer_PlayerController.GetArcherWeapon().BowWeaponData.GetDamage(AbilityData.DamageMultiplier, archer_PlayerController.PlayerCharacterData, (long)NetworkManager.LocalClientId);
        ArrowManiac_List.RemoveAt(0);
        arrow.GetComponent<Rigidbody>().AddForce(ArrowManiac_Destination[0] * AbilityData.ArrowSpeed, ForceMode.Impulse);
        ArrowManiac_Destination.RemoveAt(0);
    }

    private void OnEnable()
    {
        if (!IsOwner) return;

        ArrowManiac_List.Clear();
        ArrowManiac_Destination.Clear();
    }
    private void OnDisable()
    {
        if (!IsOwner) return;

        ArrowManiac_List.Clear();
        ArrowManiac_Destination.Clear();
    }
}
