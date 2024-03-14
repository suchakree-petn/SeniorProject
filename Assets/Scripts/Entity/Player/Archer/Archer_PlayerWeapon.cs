using System.Collections;
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
    [SerializeField] private Transform firePointTransform;

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
            Transform arrow = BowWeaponData.GetArrow(position: firePointTransform.position);
            FireArrow(arrow.GetComponent<Rigidbody>());
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


    }
    private void DrawBow()
    {

        if (DrawPower < BowConfig.MaxDrawPower)
        {
            DrawPower += Time.deltaTime;
            if (DrawPower >= BowConfig.MaxDrawPower)
            {
                DrawPower = BowConfig.MaxDrawPower;
            }
        }
        else
        {
            DrawPower = 0;
        }
    }
    private void FireArrow(Rigidbody arrowRb)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, BowConfig.targetMask))
        {
            // Calculate direction towards the hit point
            Vector3 direction = (hit.point - firePointTransform.position).normalized;
            // Rotate arrow to face the hit point
            arrowRb.transform.forward = direction;
            // Apply force in the direction of the hit point
            arrowRb.AddForce(direction * BowConfig.ArrowSpeed, ForceMode.Impulse);
        }
        else
        {
            // If ray doesn't hit anything, use the default direction
            arrowRb.transform.forward = firePointTransform.forward.normalized;
            // Apply force in the default direction
            arrowRb.AddForce(arrowRb.transform.forward * BowConfig.ArrowSpeed, ForceMode.Impulse);
        }
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
}
[System.Serializable]
public class BowConfig
{
    public float ArrowSpeed;
    public float DrawSpeed;
    public float MaxDrawPower;
    public float MaxRaycastDistance;
    public LayerMask targetMask;
}
public enum Archer_WeaponHolderState
{
    OnBack,
    InHand
}
