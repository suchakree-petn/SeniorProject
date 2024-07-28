using System.Collections;
using Mono.CSharp;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank_PlayerWeapon : PlayerWeapon
{
    public Tank_WeaponHolderState WeaponHolderState = Tank_WeaponHolderState.OnBack;

    public SwordBase SwordWeaponData;
    [SerializeField] Transform effectpos;
    [SerializeField] Transform attackPoint;
    [SerializeField] Transform attackPointHolder;
    [SerializeField] GameObject VFX_NA_Combo_1;
    [SerializeField] GameObject VFX_NA_Combo_2;
    [SerializeField] GameObject VFX_NA_Combo_3;
    [SerializeField] AnimationClip NA_Combo_1_Clip;
    [SerializeField] AnimationClip NA_Combo_2_Clip;
    [SerializeField] AnimationClip NA_Combo_3_Clip;
    [SerializeField] Tank_PlayerController tank_PlayerController;
    public AudioSource audioSource;

    private int comboIndex = 0;
    private float comboTimeInterval = 0;
    public float ComboTimeInterval => comboTimeInterval;

    private float _attackInterval;

    public override void UseWeapon(InputAction.CallbackContext context)
    {
        if (_attackInterval > 0) return;
        if (tank_PlayerController.IsDead) return;
        if (!tank_PlayerController.IsGrounded) return;
        if (!IsReadyToUse) return;

        if (context.performed)
        {
            comboIndex++;
            if (comboIndex == 1)
            {
                comboTimeInterval = NA_Combo_1_Clip.length;
                tank_PlayerController.PlayerAnimation.SetTriggerNetworkAnimation("NA_Combo_1");
            }
            else if (comboIndex == 2 && comboTimeInterval > 0)
            {
                comboTimeInterval = NA_Combo_2_Clip.length;
                tank_PlayerController.PlayerAnimation.SetTriggerNetworkAnimation("NA_Combo_2");
            }
            else if (comboIndex == 3 && comboTimeInterval > 0)
            {
                comboTimeInterval = NA_Combo_3_Clip.length;
                tank_PlayerController.PlayerAnimation.SetTriggerNetworkAnimation("NA_Combo_3");
            }
            OnUseWeapon?.Invoke();
            _attackInterval = SwordWeaponData.AttackTimeInterval;

        }


    }

    private void Update()
    {
        if (!IsOwner) return;
        if (playerController.PlayerCameraMode == PlayerCameraMode.Focus)
        {
            RotatePointHolder();
        }
        else
        {
            SetPointHolder();
        }

        // Combo timer
        if (comboTimeInterval > 0)
        {
            comboTimeInterval -= Time.deltaTime;
        }
        else if (comboTimeInterval <= 0)
        {
            comboTimeInterval = 0;
            comboIndex = 0;
        }

        if (_attackInterval > 0)
        {
            _attackInterval -= Time.deltaTime;
        }
    }
    private void RotatePointHolder()
    {
        attackPointHolder.forward = Camera.main.transform.forward;
    }
    private void SetPointHolder()
    {
        attackPointHolder.forward = transform.forward;
    }

    public void AnimationEvent_NormalAttack(int comboIndex)
    {
        SpawnSlashVFX(comboIndex);
        NormalAttack();
    }

    public override void NormalAttack()
    {

        AttackDamage attackDamage = comboIndex switch
        {
            1 => SwordWeaponData.GetDamage(SwordWeaponData.NA_Combo_1_DamageMultiplier,
                                playerController.PlayerCharacterData, (long)OwnerClientId),
            2 => SwordWeaponData.GetDamage(SwordWeaponData.NA_Combo_2_DamageMultiplier,
                                playerController.PlayerCharacterData, (long)OwnerClientId),
            3 => SwordWeaponData.GetDamage(SwordWeaponData.NA_Combo_3_DamageMultiplier,
                                playerController.PlayerCharacterData, (long)OwnerClientId),
            _ => SwordWeaponData.GetDamage(SwordWeaponData.NA_Combo_1_DamageMultiplier,
                                playerController.PlayerCharacterData, (long)OwnerClientId),
        };

        SlashAttack_ServerRpc(attackDamage, comboIndex);

    }


    [ServerRpc(RequireOwnership = false)]
    public void SlashAttack_ServerRpc(AttackDamage attackDamage, int comboIndex)
    {
        SpawnSlashVFX_ClientRpc(comboIndex, (ulong)attackDamage.AttackerClientId);

        RaycastHit[] hits = Physics.SphereCastAll(attackPoint.position, SwordWeaponData.NA_AttackRange, transform.forward, 0, tank_PlayerController.PlayerCharacterData.TargetLayer);

        foreach (RaycastHit hit in hits)
        {

            if (hit.collider.transform.root.TryGetComponent(out IDamageable damageable)
            && hit.collider.transform.root.TryGetComponent(out EnemyController _)
            && hit.collider.CompareTag("Hitbox"))
            {
                damageable.TakeDamage_ServerRpc(attackDamage);
            }
        }
    }

    [ClientRpc]
    private void SpawnSlashVFX_ClientRpc(int comboIndex, ulong clientId)
    {
        if (NetworkManager.LocalClientId == clientId) return;
        SpawnSlashVFX(comboIndex);
    }

    private void SpawnSlashVFX(int comboIndex)
    {
        audioSource.Play();
        GameObject effect = comboIndex switch
        {
            1 => Instantiate(VFX_NA_Combo_1, effectpos.position, transform.rotation),
            2 => Instantiate(VFX_NA_Combo_2, effectpos.position, transform.rotation),
            3 => Instantiate(VFX_NA_Combo_3, effectpos.position, transform.rotation),
            _ => Instantiate(VFX_NA_Combo_1, effectpos.position, transform.rotation),
        };

        Destroy(effect, 1);

    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, SwordWeaponData.NA_AttackRange);

    }

    protected override void OnEnable()
    {
        // OnUseWeapon += (serverRpcParams) => StartWeaponCooldown(SwordWeaponData.AttackTimeInterval);
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
