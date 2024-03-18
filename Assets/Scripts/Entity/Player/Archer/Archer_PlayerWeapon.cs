using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Archer_PlayerWeapon : PlayerWeapon
{
    public Archer_WeaponHolderState WeaponHolderState = Archer_WeaponHolderState.OnBack;

    public BowBase BowWeaponData;
    public BowConfig BowConfig = new();

    public bool IsDrawing;
    public float DrawPower;

    [Header("Archer Reference")]
    [SerializeField] private Transform OnBack_weaponHolderTransform;
    [SerializeField] private Transform firePointHolderTransform;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private Archer_PlayerController archer_PlayerController;

    public override void UseWeapon(InputAction.CallbackContext context)
    {
        if (WeaponHolderState == Archer_WeaponHolderState.OnBack) return;

        if (firePointTransform == null)
        {
            Debug.LogWarning($"No Fire Point transform");
            return;
        }
        if (context.performed && IsReadyToUse)
        {
            IsDrawing = true;
        }

        if (context.canceled && IsReadyToUse)
        {
            NormalAttack();
            IsDrawing = false;
            DrawPower = 0;
            OnUseWeapon?.Invoke();
        }

    }

    private void Update()
    {
        if (!IsOwner) return;

        if (IsDrawing)
        {
            DrawBow();
        }

        if (WeaponHolderState == Archer_WeaponHolderState.InHand)
        {
            RotateFirePointHolder();
        }
    }
    private void DrawBow()
    {

        if (DrawPower < BowConfig.MaxDrawPower)
        {
            DrawPower += Time.deltaTime * BowConfig.DrawSpeed;
            if (DrawPower >= BowConfig.MaxDrawPower)
            {
                DrawPower = BowConfig.MaxDrawPower;
            }
        }
        else
        {
            DrawPower = BowConfig.MaxDrawPower;
        }
    }
    public override void NormalAttack()
    {
        FireArrow_ServerRpc(BowWeaponData.NormalAttack_DamageMultiplier, DrawPower);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireArrow_ServerRpc(float damageMultiplier, float drawPower, ServerRpcParams serverRpcParams = default)
    {
        ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
        Transform arrowTransform = BowWeaponData.GetArrow(position: firePointTransform.position);
        NetworkObject arrowNetworkObject = arrowTransform.GetComponent<NetworkObject>();
        arrowNetworkObject.SpawnWithOwnership(OwnerClientId, true);

        FireArrow_ClientRpc(damageMultiplier, drawPower, OwnerClientId, arrowNetworkObject);
    }

    [ClientRpc]
    public void FireArrow_ClientRpc(float damageMultiplier, float drawPower, ulong OwnerClientId, NetworkObjectReference arrowObjRef)
    {
        if (!arrowObjRef.TryGet(out NetworkObject arrowNetObj) || OwnerClientId != NetworkManager.LocalClientId) return;

        AttackDamage attackDamage = BowWeaponData.GetDamage(damageMultiplier, playerController.PlayerCharacterData, (long)OwnerClientId);
        Arrow arrow = arrowNetObj.GetComponent<Arrow>();
        arrow.AttackDamage = attackDamage;

        Rigidbody arrowRb = arrowNetObj.GetComponent<Rigidbody>();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 direction;
        if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, playerController.PlayerCharacterData.TargetLayer) && hit.distance > Vector3.Distance(hit.point, firePointTransform.position))
        {
            // Calculate direction towards the hit point
            direction = (hit.point - firePointTransform.position).normalized;
            // Rotate arrow to face the hit point
            arrowRb.transform.forward = direction;
            // Apply force in the direction of the hit point
        }
        else
        {
            // If ray doesn't hit anything, use the default direction
            arrowRb.transform.forward = firePointTransform.forward.normalized;
            direction = arrowRb.transform.forward;
        }
        Debug.Log($"{drawPower} / {BowConfig.MaxDrawPower} * {BowConfig.ArrowSpeed}");
        arrowRb.AddForce(drawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * direction, ForceMode.Impulse);
    }
    public GameObject GetWeaponOnBack()
    {
        Transform weapon = OnBack_weaponHolderTransform.GetChild(0);
        // weapon.SetLocalPositionAndRotation(default, default);
        return weapon.gameObject;
    }
    public void SwitchWeaponHolderTo(Archer_WeaponHolderState newState)
    {
        switch (newState)
        {
            case Archer_WeaponHolderState.OnBack:
                GetWeaponOnBack().SetActive(true);
                GetWeaponInHand().SetActive(false);
                break;
            case Archer_WeaponHolderState.InHand:
                GetWeaponOnBack().SetActive(false);
                GetWeaponInHand().SetActive(true);
                break;
        }
        Debug.Log($"Switch Weapon to {newState}");

    }

    private void RotateFirePointHolder()
    {
        firePointHolderTransform.forward = Camera.main.transform.forward;
    }



    private void OnDrawGizmosSelected()
    {
        Debug.DrawLine(firePointHolderTransform.position, firePointTransform.position);
    }


    protected override void OnEnable()
    {
        OnUseWeapon += () => StartWeaponCooldown(BowWeaponData.AttackTimeInterval);
    }


}
[System.Serializable]
public class BowConfig
{
    public float ArrowSpeed;
    public float DrawSpeed;
    public float MaxDrawPower;
    public float MinDrawPower;
    public float MaxRaycastDistance;
}
public enum Archer_WeaponHolderState
{
    OnBack,
    InHand
}
