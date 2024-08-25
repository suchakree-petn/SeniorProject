using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Charmander_EnemyController : EnemyController
{
    [SerializeField] private EnemyCharacterData DefaultData;
    [SerializeField] private EnemyCharacterData FuriousData;
    [SerializeField] private float attackPower;
    [SerializeField] private float attackPower_Multiplier;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackTimeInterval;
    [SerializeField] private bool isReadyToAttack;
    [SerializeField] private bool isFinishAttack;
    [SerializeField] private Transform attackPointTransform;
    [SerializeField] private float furiousCooldownTime = 45;
    [SerializeField] private float currentFuriousCooldown = 0;
    [SerializeField] private float furiousHPRatio = 40f;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material materialDefault;
    [SerializeField] private Material materialFurious;
    [SerializeField] private AnimationClip aCScream, aCBasicAttack, aCClawAttack, aCHornAttack;
    [SerializeField] private LineRenderer lineRenderer;
    
    bool isScream = false;
    bool isFurious = false;

    protected override void Start()
    {
        base.Start();
        SetDrawLine();
        OnEnemyAttack_Local += NormalAttack;
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(attackPointTransform.position, attackRange);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!IsServer || !IsSpawned) return;
        StartFuriousMode();
        StunAnimation();
        if (isFurious && target != null)
        {
            DrawLine();
        }
        if (Vector3.Distance(attackPointTransform.position, target.position) > attackRange && isFinishAttack && CanMove)
        {
            if (!isFurious)
            {
                if (!IsTaunted)
                {
                    target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
                }
            }
            else
            {

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
            
            AttackAnimation();
            
        }
    }
    void NormalAttack()
    {
        RaycastHit[] hits = Physics.SphereCastAll(attackPointTransform.position, attackRange, attackPointTransform.forward, attackRange, EnemyCharacterData.TargetLayer);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.transform.root.TryGetComponent(out PlayerController playerController) && hit.collider.isTrigger)
            {
                // Debug.Log("Spider hit " + hit.collider.transform.root.name);
                AttackDamage attackDamage = new(attackPower_Multiplier, EnemyCharacterData.GetAttack(), DamageType.Melee, false);
                playerController.GetComponent<IDamageable>().TakeDamage_ClientRpc(attackDamage);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    void ChangeEnemyData(EnemyCharacterData data){
        _enemyCharacterData = data;
    }
    void StunAnimation(){
        animator.SetBool("Stun", IsStun);
    }
    void AttackAnimation(){
        float randomAttack = Random.Range(0, 3);
        if(randomAttack < 1){
            BasicAttack();
        }else if(randomAttack < 2){
            ClawAttack();
        }else if(randomAttack <= 3){
            HornAttack();
        }
    }
    void BasicAttack()
    {
        StartAttackCooldown(aCBasicAttack.length + 0.5f);
        attackPower_Multiplier = 1.2f;
        networkAnimator.SetTrigger("BasicAttack");
    }
    void ClawAttack()
    {
        StartAttackCooldown(aCClawAttack.length + 0.5f);
        attackPower_Multiplier = 1.5f;
        networkAnimator.SetTrigger("ClawAttack");
    }
    void HornAttack()
    {
        StartAttackCooldown(aCHornAttack.length+0.5f);
        attackPower_Multiplier = 1.0f;
        networkAnimator.SetTrigger("HornAttack");
    }
    void StartFuriousMode()
    {
        if (!isFurious && enemyHealth.CurrentHealth < enemyHealth.MaxHp * furiousHPRatio / 100f && currentFuriousCooldown <= 0)
        {
            ChangeEnemyData(FuriousData);
            StartScream(aCScream.length+0.5f);
            IsStun = false;
            stunImmunity = true;
            isFurious = true;
            currentFuriousCooldown = furiousCooldownTime;
            skinnedMeshRenderer.material = materialFurious;
        }
        else if (!isFurious)
        {
            if (currentFuriousCooldown > 0)
            {
                currentFuriousCooldown -= Time.deltaTime;
            }
        }
    }
    private void StartScream(float sec)
    {
        networkAnimator.SetTrigger("Scream");
        StartCoroutine(WaitForScream(sec));
    }
    private IEnumerator WaitForScream(float sec)
    {
        StopMoving();
        yield return new WaitForSeconds(sec);
        Moving();

    }
    void SetDrawLine()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.useWorldSpace = true;
    }
    void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);
    }
    private void OnEnemyHit_HitAnimation()
    {
        animator.SetTrigger("Hit");
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
