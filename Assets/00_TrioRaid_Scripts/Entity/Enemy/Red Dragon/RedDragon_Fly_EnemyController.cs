using System.Collections;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
public class RedDragon_Fly_EnemyController : EnemyController
{

    [FoldoutGroup("RedDragon_Fly Config")]
    [SerializeField] private float attackPower_Multiplier;
    [FoldoutGroup("RedDragon_Fly Config")]
    [SerializeField] private float attackRange;
    [FoldoutGroup("RedDragon_Fly Config")]
    [SerializeField] private float attackTimeInterval;
    [FoldoutGroup("RedDragon_Fly Config")]
    [SerializeField] private bool isReadyToAttack;
    [FoldoutGroup("RedDragon_Fly Config")]
    [SerializeField] private bool isFinishAttack;

    [FoldoutGroup("RedDragon_Fly Reference")]
    [SerializeField] private Transform attackPointTransform;

    protected override void Start()
    {
        base.Start();
        OnEnemyAttack_Local += NormalAttack;
        // OnEnemyHit_Local += OnEnemyHit_HitAnimation;

        Target = null;
        gameObject.SetActive(false);

    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner || !IsServer) return;
    }

    protected override void Update()
    {
        base.Update();
        if (!IsServer || !IsSpawned) return;

        if (Target && Vector3.Distance(transform.position, Target.position) > attackRange && isFinishAttack)
        {
            agent.isStopped = false;
            animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 1, Time.deltaTime * 5));
        }
        else
        {
            agent.isStopped = true;

            // Attack
            animator.SetFloat("VelocityZ", Mathf.Lerp(animator.GetFloat("VelocityZ"), 0, Time.deltaTime * 10));
            if (!isReadyToAttack || IsStun) return;
            StartAttackCooldown(attackTimeInterval);
            networkAnimator.SetTrigger("Attack");

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

    private void OnDrawGizmos()
    {
        if (Target)
        {
            Vector3 dir = (Target.position - attackPointTransform.position).normalized;
            Debug.DrawLine(attackPointTransform.position, dir * attackRange);

        }
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

    }

    [ServerRpc(RequireOwnership = false)]
    public override void TakeDamage_ServerRpc(AttackDamage damage)
    {

    }

}
