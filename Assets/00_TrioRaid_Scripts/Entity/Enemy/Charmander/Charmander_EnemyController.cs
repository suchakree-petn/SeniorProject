using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Charmander_EnemyController : EnemyController
{
    [SerializeField] private float attackPower;
    [SerializeField] private float attackPower_Multiplier;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimeInterval;
    [SerializeField] private bool isReadyToAttack;
    [SerializeField] private bool isFinishAttack;    
    [SerializeField] private Transform attackPointTransform;
    protected override void Start()
    {
        base.Start();
        OnEnemyAttack_Local += NormalAttack;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!IsServer || !IsSpawned) return;
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
            StartAttackCooldown(attackTimeInterval);
            networkAnimator.SetTrigger("Attack");
            
        }
    }
    void NormalAttack(){
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

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    void BasicAttack(){
        
    }
    void ClawAttack(){

    }
    void HornAttack(){

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
            networkAnimator.SetTrigger("Death");
            StartCoroutine(DelayDestroy(4f));
        }
    }
}
