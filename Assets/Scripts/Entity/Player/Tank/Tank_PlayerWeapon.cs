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
    [SerializeField] Transform effectpos;
    [SerializeField] Transform attackRange;
    [SerializeField] Transform attackRangeHolder;
    [SerializeField] GameObject slashEffectPrefab;
    [SerializeField] GameObject slashEffectPrefab2;
    public bool IsSlash;
    public float DrawPower;



    public override void UseWeapon(InputAction.CallbackContext context)
    {
        if (!IsReadyToUse) return;

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
        if (playerController.PlayerCameraMode == PlayerCameraMode.Focus)
        {
            RotatePointHolder();
        }
        else
        {
            SetPointHolder();
        }
    }
    private void RotatePointHolder()
    {
        attackRangeHolder.forward = Camera.main.transform.forward;
    }
    private void SetPointHolder()
    {
        attackRangeHolder.forward = transform.forward;
    }
    public override void NormalAttack()
    {
        AttackDamage attackDamage = SwordWeaponData.GetDamage(SwordWeaponData.LightAttack_DamageMultiplier, playerController.PlayerCharacterData, (long)OwnerClientId);
        Debug.Log("NAttack");
        SlashAttack_ServerRpc(attackDamage);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SlashAttack_ServerRpc(AttackDamage attackDamage, ServerRpcParams serverRpcParams = default)
    {
        GameObject effect;
        if(WeaponHolderState == Tank_WeaponHolderState.InHand){
            effect = Instantiate(slashEffectPrefab, effectpos.position, attackRange.rotation);
        }else{
            effect = Instantiate(slashEffectPrefab2, effectpos.position, attackRange.rotation);
        }
        effect.GetComponent<SlashEffect>()._position = effectpos;
        effect.GetComponent<NetworkObject>().Spawn(true);

        RaycastHit[] hits = Physics.BoxCastAll(attackRange.position, new Vector3(4, 1, 2), transform.forward, transform.rotation, 3f);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("outif" + hit.transform.gameObject.name);

            if (hit.collider.transform.root.TryGetComponent(out IDamageable damageable)
            && hit.collider.transform.root.TryGetComponent(out EnemyController _)
            && hit.collider.CompareTag("Hitbox"))
            {
                Debug.Log("inif" + hit.transform.root.gameObject.name);
                damageable.TakeDamage_ClientRpc(attackDamage);
            }
        }
    }

    void OnDrawGizmos()
    {
        float maxDistance = 3f;

        RaycastHit[] hits = Physics.BoxCastAll(attackRange.position, new Vector3(4, 1, 2), transform.forward, transform.rotation, maxDistance);
        foreach (RaycastHit hit in hits)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(attackRange.position, new Vector3(4, 1, 2));
        }

    }

    protected override void OnEnable()
    {

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
