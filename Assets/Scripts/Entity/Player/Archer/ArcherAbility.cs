using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherAbility : PlayerAbility
{
    public ulong UserClientId;
    public ArcherAbilityData archerAbilityData;
    public List<Transform> ArrowManiac_List;
    public List<Vector3> ArrowManiac_Destination;
    public Transform SpawnTransform;
    public Transform ArrowManiacSpawnGroup;
    public LayerMask TargetLayer;
    public override void ActivateAbility(ulong userClientId)
    {
        Debug.Log($"{archerAbilityData.Name} activated");
        Archer_PlayerController playerController = PlayerManager.Instance.PlayerGameObjectsByRole[PlayerRole.DamageDealer].GetComponent<Archer_PlayerController>();
        UserClientId = playerController.OwnerClientId;
        TargetLayer = playerController.PlayerCharacterData.TargetLayer;
        SpawnTransform = playerController.GetArcherWeapon().GetFirePointTransform();
        if (ArrowManiacSpawnGroup == null)
        {
            ArrowManiacSpawnGroup = Instantiate(archerAbilityData.ArrowManiac_SpawnGroup, SpawnTransform);
        }
        StartCoroutine(ActiveDuration(archerAbilityData.Duration, playerController));
    }

    IEnumerator ActiveDuration(float duration, Archer_PlayerController playerController)
    {

        Debug.Log($"Start {archerAbilityData.Name} duration: {duration}");
        playerController.SetCanPlayerMove(false);
        Archer_PlayerWeapon archer_PlayerWeapon = playerController.GetArcherWeapon();
        archer_PlayerWeapon.OnUseWeapon += SpawnArrowManiac_ServerRpc;

        yield return new WaitForSeconds(duration);
        foreach (Transform arrow in ArrowManiac_List)
        {
            Destroy(arrow.gameObject);
        }
        ArrowManiac_List.Clear();
        ArrowManiac_Destination.Clear();
        playerController.SetCanPlayerMove(true);
        archer_PlayerWeapon.OnUseWeapon -= SpawnArrowManiac_ServerRpc;
        Debug.Log($"Finish {archerAbilityData.Name} ");
        Destroy(ArrowManiacSpawnGroup.gameObject);

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
        InvokeRepeating(nameof(FireArrowManiac), 0.5f, 0.2f);
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
