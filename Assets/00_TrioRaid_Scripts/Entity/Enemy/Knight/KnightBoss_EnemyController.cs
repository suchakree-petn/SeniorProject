using System.Collections;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
public class KnightBoss_EnemyController : EnemyController
{

    [FoldoutGroup("Knight Config")]
    [SerializeField] private float attackPower_Multiplier;
    [FoldoutGroup("Knight Config")]
    [SerializeField] private float attackRange;
    [FoldoutGroup("Knight Config")]
    [SerializeField] private float attackTimeInterval;
    [FoldoutGroup("Knight Config")]
    [SerializeField] private bool isReadyToAttack;
    [FoldoutGroup("Knight Config")]
    [SerializeField] private bool isFinishAttack;

    [FoldoutGroup("Knight Reference")]
    [SerializeField] private Transform attackPointTransform;

    protected override void Start()
    {
        base.Start();
        OnEnemyAttack_Local += NormalAttack;
        // OnEnemyHit_Local += OnEnemyHit_HitAnimation;

    }



    protected override void Update()
    {
        base.Update();
        if (!IsServer || !IsSpawned) return;
        if (Vector3.Distance(transform.position, Target.position) > attackRange + 2 && isFinishAttack && CanMove)
        {
            if (!IsTaunted)
            {
                Target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
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
            StartAttackCooldown(attackTimeInterval);
            animator.SetTrigger("Attack");
            
        }
    }

    public void NormalAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(attackPointTransform.position, attackRange, attackPointTransform.forward, attackRange, EnemyCharacterData.TargetLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root.TryGetComponent(out PlayerController playerController) && hit.collider.isTrigger)
            {
                AttackDamage attackDamage = new(attackPower_Multiplier, EnemyCharacterData.AttackBase, DamageType.Melee, false);
                playerController.GetComponent<IDamageable>().TakeDamage_ClientRpc(attackDamage);
            }
        }

    }

    private void OnEnemyHit_HitAnimation()
    {
        animator.SetTrigger("Hit");
    }

    // private void OnDrawGizmos()
    // {
    //     Gizmos.DrawWireSphere(attackPointTransform.position, attackRange);
    // }
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
            animator.SetTrigger("Death");
            StartCoroutine(DelayDestroy(4f));
        }
    }

}
