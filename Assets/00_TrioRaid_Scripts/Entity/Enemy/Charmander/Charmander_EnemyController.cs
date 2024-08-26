using System.Collections;
using Unity.Netcode;
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
    [SerializeField] private float furiousMaxPoint = 20f;
    public float FuriousPoint = 0f;
    [SerializeField] private float furiousPointRegenPerSec = 5f;
    [SerializeField] private float furiousPointDelayRegen = 3f;
    [SerializeField] private float furiousStunTime = 3f;
    [SerializeField] private bool isRegen;
    [SerializeField] private float regenPoint;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material materialDefault;
    [SerializeField] private Material materialFurious;
    [SerializeField] private AnimationClip aCScream, aCBasicAttack, aCClawAttack, aCHornAttack, aCHit;
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
        if (Input.GetKeyDown(KeyCode.X))
        {
            BallistaHit();
        }
        if (isRegen && FuriousPoint < furiousMaxPoint && regenPoint > 0)
        {
            FuriousPoint += Time.deltaTime * furiousPointRegenPerSec;
            regenPoint -= Time.deltaTime * furiousPointRegenPerSec;
        }
        else if (FuriousPoint > 20f || regenPoint < 0)
        {
            FuriousPoint = 20;
            regenPoint = 0;
        }
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
    void ChangeEnemyData(EnemyCharacterData data)
    {
        _enemyCharacterData = data;
    }
    void StunAnimation()
    {
        animator.SetBool("Stun", IsStun);
    }
    void AttackAnimation()
    {
        float randomAttack = Random.Range(0, 3);
        if (randomAttack < 1)
        {
            BasicAttack();
        }
        else if (randomAttack < 2)
        {
            ClawAttack();
        }
        else if (randomAttack <= 3)
        {
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
        StartAttackCooldown(aCHornAttack.length + 0.5f);
        attackPower_Multiplier = 1.0f;
        networkAnimator.SetTrigger("HornAttack");
    }
    void StartFuriousMode()
    {
        if (!isFurious && enemyHealth.CurrentHealth < enemyHealth.MaxHp * furiousHPRatio / 100f && currentFuriousCooldown <= 0)
        {
            FuriousModeServerRpc();
        }
        else if (!isFurious)
        {
            if (currentFuriousCooldown > 0)
            {
                currentFuriousCooldown -= Time.deltaTime;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void FuriousModeServerRpc()
    {
        ChangeEnemyData(FuriousData);
        StartScream(aCScream.length + 0.5f);
        IsStun = false;
        stunImmunity = true;
        isFurious = true;
        FuriousPoint = furiousMaxPoint;
        currentFuriousCooldown = furiousCooldownTime;
        skinnedMeshRenderer.material = materialFurious;
    }
    [ClientRpc]
    void FuriousModeClientRpc()
    {
        ChangeEnemyData(FuriousData);
        StartScream(aCScream.length + 0.5f);
        IsStun = false;
        stunImmunity = true;
        isFurious = true;
        FuriousPoint = furiousMaxPoint;
        currentFuriousCooldown = furiousCooldownTime;
        skinnedMeshRenderer.material = materialFurious;
    }
    private void StartScream(float sec)
    {
        networkAnimator.SetTrigger("Scream");
        if (_delayStunCoroutine == null)
        {
            _delayStunCoroutine = StartCoroutine(WaitForStop(sec));
        }
        else
        {
            StopCoroutine(_delayStunCoroutine);
            _delayStunCoroutine = StartCoroutine(WaitForStop(sec));
        }
    }
    private IEnumerator WaitForStop(float sec)
    {
        StopMoving();
        yield return new WaitForSeconds(sec);
        Moving();

    }
    Coroutine _delayFuriousRegenCoroutine;
    Coroutine _delayStunCoroutine;
    public void BallistaHit()
    {
        if (isFurious)
        {
            FuriousPoint -= 10f;
            regenPoint += 10f;
            FuriousPointRegen();
            OnEnemyHit_HitAnimation(aCHit.length);
            if (FuriousPoint <= 0)
            {
                FuriousPoint = 0;
                ReturnToDefaultModeServerRpc();

            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void ReturnToDefaultModeServerRpc()
    {
        ReturnToDefaultModeClientRpc();
    }
    [ClientRpc]
    void ReturnToDefaultModeClientRpc()
    {
        ChangeEnemyData(DefaultData);
        stunImmunity = false;
        isFurious = false;
        skinnedMeshRenderer.material = materialDefault;
        FuriousStun();
    }

    private void OnEnemyHit_HitAnimation(float sec)
    {
        animator.SetTrigger("Hit");
        if (_delayStunCoroutine == null)
        {
            _delayStunCoroutine = StartCoroutine(WaitForStop(sec));
        }
        else
        {
            StopCoroutine(_delayStunCoroutine);
            _delayStunCoroutine = StartCoroutine(WaitForStop(sec));
        }
    }
    void FuriousPointRegen()
    {
        if (_delayFuriousRegenCoroutine == null)
        {
            _delayFuriousRegenCoroutine = StartCoroutine(DelayFuriousPointRegen());
        }
        else
        {
            StopCoroutine(_delayFuriousRegenCoroutine);
            _delayFuriousRegenCoroutine = StartCoroutine(DelayFuriousPointRegen());
        }
    }
    void FuriousStun()
    {
        if (_delayStunCoroutine == null)
        {
            _delayStunCoroutine = StartCoroutine(StartFuriousStun());
        }
        else
        {
            StopCoroutine(_delayStunCoroutine);
            _delayStunCoroutine = StartCoroutine(StartFuriousStun());
        }
    }
    IEnumerator StartFuriousStun()
    {
        StopMoving();
        IsStun = true;
        yield return new WaitForSeconds(furiousStunTime + 0.5f);
        Moving();
        IsStun = false;
    }
    IEnumerator DelayFuriousPointRegen()
    {
        isRegen = false;
        yield return new WaitForSeconds(furiousPointDelayRegen);
        isRegen = true;
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
