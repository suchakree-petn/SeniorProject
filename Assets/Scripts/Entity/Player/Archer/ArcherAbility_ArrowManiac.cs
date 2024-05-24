using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArcherAbility_ArrowManiac : PlayerAbility
{
    public ulong UserClientId;
    public ArcherAbilityData_ArrowManiac archerAbilityData;
    public List<Transform> ArrowManiac_List;
    public List<Vector3> ArrowManiac_Destination;
    public Transform ArrowManiacSpawnGroup;
    private LayerMask TargetLayer;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{archerAbilityData.Name} activated");
        Archer_PlayerController playerController = GetComponent<Archer_PlayerController>();
        UserClientId = playerController.OwnerClientId;
        TargetLayer = playerController.PlayerCharacterData.TargetLayer;

        StartCoroutine(ActiveDuration(archerAbilityData.Duration, playerController));

        AbilityUIManager.Instance.OnUseAbility_E?.Invoke(archerAbilityData.Cooldown);

        SetOnCD();
        Invoke(nameof(SetFinishCD), archerAbilityData.Cooldown);
    }

    IEnumerator ActiveDuration(float duration, Archer_PlayerController playerController)
    {

        Debug.Log($"Start {archerAbilityData.Name} duration: {duration}");
        playerController.SetCanPlayerMove(false);
        Archer_PlayerWeapon archer_PlayerWeapon = playerController.GetArcherWeapon();
        archer_PlayerWeapon.OnUseWeapon += SpawnArrowManiac_ServerRpc;

        yield return new WaitForSeconds(duration);
        archer_PlayerWeapon.OnUseWeapon -= SpawnArrowManiac_ServerRpc;
        CancelInvoke(nameof(FireArrowManiac));

        yield return new WaitForSeconds(2);

        foreach (Transform arrow in ArrowManiac_List)
        {
            arrow.GetComponent<NetworkObject>().Despawn();
            Destroy(arrow.gameObject);
        }
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
        ArrowManiac_List.Clear();
        ArrowManiac_Destination.Clear();
        playerController.SetCanPlayerMove(true);
        Debug.Log($"Finish {archerAbilityData.Name} ");

    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnArrowManiac_ServerRpc()
    {
        Debug.Log($"Spawn Arrow");
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        foreach (Transform child in ArrowManiacSpawnGroup.transform)
        {
            Transform arrow = Instantiate(archerAbilityData.ArrowManiac_prf, child.position, child.rotation);
            arrow.GetComponent<NetworkObject>().SpawnWithOwnership(UserClientId);
            ArrowManiac_List.Add(arrow);
            Vector3 direction;
            if (Physics.Raycast(ray, out RaycastHit hit, archerAbilityData.Distance, TargetLayer, QueryTriggerInteraction.Collide))
            {
                direction = (hit.point - child.position).normalized;
            }
            else
            {
                Vector3 endPoint = Camera.main.transform.position + Camera.main.transform.forward * archerAbilityData.Distance;
                direction = (endPoint - child.position).normalized;
            }
            arrow.forward = direction;
            ArrowManiac_Destination.Add(direction);
        }
        CancelInvoke(nameof(FireArrowManiac));
        InvokeRepeating(nameof(FireArrowManiac), 0.3f, archerAbilityData.TimeInterval);
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
        arrowManiac.AttackDamage = archer_PlayerController.GetArcherWeapon().BowWeaponData.GetDamage(archerAbilityData.DamageMultiplier, archer_PlayerController.PlayerCharacterData, (long)NetworkManager.LocalClientId);
        ArrowManiac_List.RemoveAt(0);
        arrow.GetComponent<Rigidbody>().AddForce(ArrowManiac_Destination[0] * archerAbilityData.ArrowSpeed, ForceMode.Impulse);
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
