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
        if (context.performed)
        {
            IsDrawing = true;
        }

        if (context.canceled)
        {
            archer_PlayerController.FireArrow_ServerRpc();
            IsDrawing = false;
            DrawPower = 0;
        }

    }

    private void Update()
    {
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
    public void FireArrow(NetworkObjectReference arrowNetworkObjectReference)
    {
        if (!arrowNetworkObjectReference.TryGet(out NetworkObject arrowNetworkObject)) return;

        Rigidbody arrowRb = arrowNetworkObject.GetComponent<Rigidbody>();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 direction;
        if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, BowConfig.targetMask) && hit.distance > Vector3.Distance(hit.point, firePointTransform.position))
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
        arrowRb.AddForce(DrawPower / BowConfig.MaxDrawPower * BowConfig.ArrowSpeed * direction, ForceMode.Impulse);
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
}
[System.Serializable]
public class BowConfig
{
    public float ArrowSpeed;
    public float DrawSpeed;
    public float MaxDrawPower;
    public float MinDrawPower;
    public float MaxRaycastDistance;
    public LayerMask targetMask;
}
public enum Archer_WeaponHolderState
{
    OnBack,
    InHand
}
