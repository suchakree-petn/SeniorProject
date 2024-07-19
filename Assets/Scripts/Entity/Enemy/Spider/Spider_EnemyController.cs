using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class Spider_EnemyController : EnemyController
{

    [SerializeField] private float attackPower;
    [SerializeField] private float attackPower_Multiplier;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimeInterval;
    [SerializeField] private bool isReadyToAttack;
    [SerializeField] private bool isFinishAttack;


    [Header("Spider Reference")]
    [SerializeField] private Transform attackPointTransform;

    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        if (!IsServer || !IsSpawned ) return;
        if (Vector3.Distance(transform.position, target.position) > attackRange + 2 && isFinishAttack && CanMove)
        {
            if (!IsTaunted)
            {
                target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
            }
            agent.isStopped = false;
            animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 1, Time.deltaTime * 5));
        }
        else
        {
            agent.isStopped = true;

            if (!CanMove) return;
            // Attack
            animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 0, Time.deltaTime * 10));
            if (!isReadyToAttack || IsStun) return;
            NormalAttack();
            StartAttackCooldown(attackTimeInterval);
            networkAnimator.SetTrigger("Attack");
        }
    }

    private void NormalAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(attackPointTransform.position, attackRange, attackPointTransform.forward, attackRange, EnemyCharacterData.TargetLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root.TryGetComponent(out PlayerController playerController) && hit.collider.isTrigger)
            {
                // Debug.Log("Spider hit " + hit.collider.transform.root.name);
                AttackDamage attackDamage = new(attackPower_Multiplier, attackPower, DamageType.Melee, false);
                playerController.GetComponent<IDamageable>().TakeDamage_ClientRpc(attackDamage);
            }
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPointTransform.position, attackRange);
    }
    private void StartAttackCooldown(float sec)
    {
        StartCoroutine(WaitForWeaponCooldown(sec));
    }
    private IEnumerator WaitForWeaponCooldown(float sec)
    {
        isReadyToAttack = false;
        isFinishAttack = false;
        yield return new WaitForSeconds(sec);
        isReadyToAttack = true;
        isFinishAttack = true;
    }



    protected override void OnEnable()
    {
        base.OnEnable();
    }
    [ClientRpc]
    public override void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner)
        {
            Debug.Log($"Not owner on take damage");
            return;
        }
        base.TakeDamage_ClientRpc(damage);

    }
    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamage_ServerRpc(AttackDamage damage)
    {
        base.TakeDamage_ServerRpc(damage);

        if (enemyHealth.CurrentHealth <= 0)
        {
            networkAnimator.SetTrigger("Death");
            StartCoroutine(DelayDestroy(4f));
        }
    }

}
