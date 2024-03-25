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
        if (!IsOwner) return;

        if (Archer_WeaponHolderState.InHand != WeaponHolderState) return;

        if (!IsReadyToUse) return;

        if (firePointTransform == null)
        {
            Debug.LogWarning($"No Fire Point transform");
            return;
        }

        if (context.performed)
        {
            IsDrawing = true;
        }

        if (context.canceled)
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

        if (Archer_WeaponHolderState.InHand == WeaponHolderState)
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
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 direction;
        if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, playerController.PlayerCharacterData.TargetLayer, QueryTriggerInteraction.Ignore))
        {
            direction = (hit.point - firePointTransform.position).normalized;
        }
        else
        {
            Vector3 endPoint = Camera.main.transform.position + Camera.main.transform.forward * BowConfig.MaxRaycastDistance;
            direction = (endPoint - firePointTransform.position).normalized;
        }
        FireArrow(BowWeaponData.NormalAttack_DamageMultiplier, DrawPower, direction);
        FireArrow_ServerRpc(DrawPower, direction);
    }

    [ServerRpc(RequireOwnership = false)]
    public void FireArrow_ServerRpc(float drawPower, Vector3 arrowDirection, ServerRpcParams serverRpcParams = default)
    {
        ulong OwnerClientId = serverRpcParams.Receive.SenderClientId;
        FireArrow_ClientRpc(drawPower, OwnerClientId, arrowDirection);
    }

    [ClientRpc]
    public void FireArrow_ClientRpc(float drawPower, ulong OwnerClientId, Vector3 arrowDirection)
    {
        if (OwnerClientId == NetworkManager.LocalClientId)
        {
            Debug.Log($"Not shooter");
            return;
        }

        Transform arrowTransform = BowWeaponData.GetArrow(firePointTransform.position);

        Rigidbody arrowRb = arrowTransform.GetComponent<Rigidbody>();
        arrowTransform.forward = arrowDirection;
        arrowRb.AddForce(drawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * arrowDirection, ForceMode.Impulse);
    }
    private void FireArrow(float damageMultiplier, float drawPower, Vector3 arrowDirection)
    {
        AttackDamage attackDamage = BowWeaponData.GetDamage(damageMultiplier, playerController.PlayerCharacterData, (long)NetworkManager.LocalClientId);
        attackDamage.Damage = attackDamage.Damage / drawPower * BowConfig.MaxDrawPower;

        Transform arrowTransform = BowWeaponData.GetArrow(firePointTransform.position);
        Arrow arrow = arrowTransform.GetComponent<Arrow>();
        arrow.AttackDamage = attackDamage;

        Rigidbody arrowRb = arrowTransform.GetComponent<Rigidbody>();
        arrowTransform.forward = arrowDirection;
        arrowRb.AddForce(drawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * arrowDirection, ForceMode.Impulse);
    }
    public GameObject GetWeaponOnBack()
    {
        Transform weapon = OnBack_weaponHolderTransform.GetChild(0);
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

    public void SetWeaponHolderPosition(PlayerCameraMode prev, PlayerCameraMode current)
    {
        if (current == PlayerCameraMode.Focus)
        {
            SwitchWeaponHolderTo(Archer_WeaponHolderState.InHand);
            WeaponHolderState = Archer_WeaponHolderState.InHand;
        }
        else
        {
            SwitchWeaponHolderTo(Archer_WeaponHolderState.OnBack);
            WeaponHolderState = Archer_WeaponHolderState.OnBack;
        }

    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + Camera.main.transform.forward * BowConfig.MaxRaycastDistance, Color.red);
    }


    protected override void OnEnable()
    {
        playerController.OnPlayerCameraModeChanged += SetWeaponHolderPosition;
        if (!IsOwner) return;


        OnUseWeapon += () => StartWeaponCooldown(BowWeaponData.AttackTimeInterval);
    }
    protected override void OnDisable()
    {
        playerController.OnPlayerCameraModeChanged -= SetWeaponHolderPosition;
        if (!IsOwner) return;

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
