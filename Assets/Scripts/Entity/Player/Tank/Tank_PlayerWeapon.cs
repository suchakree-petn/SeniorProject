using System.Collections;
using Mono.CSharp;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank_PlayerWeapon : PlayerWeapon
{
    public Tank_WeaponHolderState WeaponHolderState = Tank_WeaponHolderState.OnBack;

    public SwordBase SwordWeaponData;
    public SwordDamageDealer swordPrefab;
    // public BowConfig BowConfig = new();

    public bool IsSlash;
    public float DrawPower;

    // [Header("Tank Reference")]
    // [SerializeField] private Transform OnBack_weaponHolderTransform;
    // [SerializeField] private Transform firePointHolderTransform;
    // [SerializeField] private Transform firePointTransform;
    // [SerializeField] private Tank_PlayerController archer_PlayerController;

    public override void UseWeapon(InputAction.CallbackContext context)
    {
        if(!IsReadyToUse) return;

        if (context.performed)
        {
            IsSlash = true;
            NormalAttack();
        }

        if (context.canceled)
        {
            IsSlash = false;
            OnUseWeapon?.Invoke();
        }

    }

    private void Update()
    {
        if (!IsOwner) return;

    }

    public override void NormalAttack()
    {
        AttackDamage attackDamage = SwordWeaponData.GetDamage(SwordWeaponData.LightAttack_DamageMultiplier, playerController.PlayerCharacterData, (long)OwnerClientId);
        Debug.Log("NAttack");
        SlashAttack_ServerRpc(attackDamage);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SlashAttack_ServerRpc(AttackDamage attackDamage, ServerRpcParams serverRpcParams = default){
        SwordDamageDealer swordDamage = swordPrefab.GetComponent<SwordDamageDealer>();
        swordDamage.AttackDamage = attackDamage;
        // ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
        // Transform healOrbTransform = MagicItemWeaponData.GetHealOrb(position: firePointTransform.position);
        // NetworkObject healOrbNetworkObject = healOrbTransform.GetComponent<NetworkObject>();
        // healOrbNetworkObject.Spawn(true);
        // healOrbTransform.transform.forward = Camera.main.transform.forward;
        // HealOrb healOrb = healOrbTransform.GetComponent<HealOrb>();
        // if (isHasLockTarget)
        // {
        //     healOrb.target = PlayerManager.Instance.PlayerGameObjects[targetClientId].transform;
        // }
        // healOrb.AttackDamage = attackDamage;
    }

    protected override void OnEnable()
    {
        // if(!IsOwner) return;

        // OnUseWeapon += () => StartWeaponCooldown(SwordWeaponData.AttackTimeInterval);
    }


    //     [ServerRpc(RequireOwnership = false)]
    //     public void SlashAttack_ServerRpc(float damageMultiplier, float drawPower, ServerRpcParams serverRpcParams = default)
    //     {
    //         ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
    //         Transform arrowTransform = SwordWeaponData.GetArrow(position: firePointTransform.position);
    //         NetworkObject arrowNetworkObject = arrowTransform.GetComponent<NetworkObject>();
    //         arrowNetworkObject.SpawnWithOwnership(OwnerClientId, true);

    //         SlashAttack_ClientRpc(damageMultiplier, drawPower, OwnerClientId, arrowNetworkObject);
    //     }

    //     [ClientRpc]
    //     public void SlashAttack_ClientRpc(float damageMultiplier, float drawPower, ulong OwnerClientId, NetworkObjectReference swordObjRef)
    //     {
    //         if (!swordObjRef.TryGet(out NetworkObject arrowNetObj) || OwnerClientId != NetworkManager.LocalClientId) return;

    //         AttackDamage attackDamage = SwordWeaponData.GetDamage(damageMultiplier, playerController.PlayerCharacterData,(long)OwnerClientId);
    //         SwordDamageDealer sword = arrowNetObj.GetComponent<SwordDamageDealer>();
    //         sword.AttackDamage = attackDamage;

    //         // Rigidbody arrowRb = swordObjRef.GetComponent<Rigidbody>();
    //         Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    //         Vector3 direction;
    //         if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, BowConfig.targetMask) && hit.distance > Vector3.Distance(hit.point, firePointTransform.position))
    //         {
    //             // Calculate direction towards the hit point
    //             direction = (hit.point - firePointTransform.position).normalized;
    //             // Rotate arrow to face the hit point
    //             arrowRb.transform.forward = direction;
    //             // Apply force in the direction of the hit point
    //         }
    //         else
    //         {
    //             // If ray doesn't hit anything, use the default direction
    //             arrowRb.transform.forward = firePointTransform.forward.normalized;
    //             direction = arrowRb.transform.forward;
    //         }
    //         Debug.Log($"{drawPower} / {BowConfig.MaxDrawPower} * {BowConfig.ArrowSpeed}");
    //         arrowRb.AddForce(drawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * direction, ForceMode.Impulse);
    //     }
    
    void OnDrawGizmos()
    {
        float maxDistance = 3f;

        RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.lossyScale / 2, transform.forward);
        foreach(RaycastHit hit in hits){
            if (hit.transform.root.TryGetComponent(out IDamageable damageable) && hit.transform.root.TryGetComponent(out PlayerController playerController))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                Gizmos.DrawWireCube(transform.position + transform.forward * hit.distance, transform.lossyScale);
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
            }
        }
        
    }
}
// [System.Serializable]
// public class BowConfig
// {
//     public float ArrowSpeed;
//     public float DrawSpeed;
//     public float MaxDrawPower;
//     public float MinDrawPower;
//     public float MaxRaycastDistance;
//     public LayerMask targetMask;
// }
public enum Tank_WeaponHolderState
{
    OnBack,
    InHand
}
