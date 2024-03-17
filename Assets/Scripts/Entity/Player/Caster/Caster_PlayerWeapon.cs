using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Caster_PlayerWeapon : PlayerWeapon
{

    public MagicItemBase MagicItemWeaponData;
    public MagicItemConfig MagicItemConfig = new();


    [Header("Caster Reference")]
    [SerializeField] private Transform firePointHolderTransform;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private Caster_PlayerController caster_PlayerController;

    public override void UseWeapon(InputAction.CallbackContext context)
    {

        if (firePointTransform == null)
        {
            Debug.LogWarning($"No Fire Point transform");
            return;
        }
        if (context.performed)
        {
            NormalAttack();
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

    public override void NormalAttack()
    {
        LaunchHealOrb_ServerRpc(MagicItemWeaponData.NormalAttack_HealMultiplier);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LaunchHealOrb_ServerRpc(float healMultiplier,ulong targetClientId, ServerRpcParams serverRpcParams = default)
    {
        ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
        Transform healOrbTransform = MagicItemWeaponData.GetHealOrb(position: firePointTransform.position);
        NetworkObject healOrbNetworkObject = healOrbTransform.GetComponent<NetworkObject>();
        healOrbNetworkObject.Spawn(true);
        healOrbTransform.GetComponent<HealOrb>().target = PlayerManager.Instance.PlayerGameObjects[targetClientId].transform;

        // FireArrow_ClientRpc(damageMultiplier, drawPower, OwnerClientId, arrowNetworkObject);
    }

    // [ClientRpc]
    // // public void FireArrow_ClientRpc(float damageMultiplier, float drawPower, ulong OwnerClientId, NetworkObjectReference arrowObjRef)
    // // {
    // //     if (!arrowObjRef.TryGet(out NetworkObject arrowNetObj) || OwnerClientId != NetworkManager.LocalClientId) return;

    // //     AttackDamage attackDamage = BowWeaponData.GetDamage(damageMultiplier, playerController.PlayerCharacterData,(long)OwnerClientId);
    // //     Arrow arrow = arrowNetObj.GetComponent<Arrow>();
    // //     arrow.AttackDamage = attackDamage;

    // //     Rigidbody arrowRb = arrowNetObj.GetComponent<Rigidbody>();
    // //     Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    // //     Vector3 direction;
    // //     if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, BowConfig.targetMask) && hit.distance > Vector3.Distance(hit.point, firePointTransform.position))
    // //     {
    // //         // Calculate direction towards the hit point
    // //         direction = (hit.point - firePointTransform.position).normalized;
    // //         // Rotate arrow to face the hit point
    // //         arrowRb.transform.forward = direction;
    // //         // Apply force in the direction of the hit point
    // //     }
    // //     else
    // //     {
    // //         // If ray doesn't hit anything, use the default direction
    // //         arrowRb.transform.forward = firePointTransform.forward.normalized;
    // //         direction = arrowRb.transform.forward;
    // //     }
    // //     Debug.Log($"{drawPower} / {BowConfig.MaxDrawPower} * {BowConfig.ArrowSpeed}");
    // //     arrowRb.AddForce(drawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * direction, ForceMode.Impulse);
    // // }

    private void RotateFirePointHolder()
    {
        firePointHolderTransform.forward = Camera.main.transform.forward;
    }


}
[System.Serializable]
public class MagicItemConfig
{
    public float MaxRaycastDistance;
    public LayerMask targetMask;
}