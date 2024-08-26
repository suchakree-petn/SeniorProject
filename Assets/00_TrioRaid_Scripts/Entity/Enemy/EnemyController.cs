using System;
using System.Collections;
using TheKiwiCoder;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class EnemyController : NetworkBehaviour, IDamageable
{
    public Action OnEnemyDead_Local;
    public Action OnEnemyHit_Local;
    public Action OnEnemyAttack_Local;

    [SerializeField] protected EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected NavMeshPath path;
    [SerializeField] protected BehaviourTreeInstance behaviourTreeInstance;

    public Transform Target;
    public bool CanMove = true;
    public bool IsTaunted = false;
    public bool IsStun = false;
    public bool stunImmunity = false;
    public bool IsDead => enemyHealth.IsDead;
    private float delayEnemySpawn = 1;

    [FoldoutGroup("Reference")]
    public EnemyHealth enemyHealth;
    [FoldoutGroup("Reference")]
    public Rigidbody enemyRb;
    [FoldoutGroup("Reference")]
    public NetworkAnimator networkAnimator;
    [FoldoutGroup("Reference")]
    public Animator animator;
    [FoldoutGroup("Reference")]
    public Collider hitBox;
    [FoldoutGroup("Reference")]
    public Collider collideHitBox;
    [FoldoutGroup("Reference")]
    public Collider critHitBox;
    [FoldoutGroup("Reference")]
    [SerializeField] private Renderer mesh;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform mesh_parent;
    [FoldoutGroup("Reference")]
    private Material dissolveMaterial;

    protected virtual void Awake()
    {
        dissolveMaterial = mesh.material;
        if (!IsOwner) return;
    }

    protected virtual void Start()
    {
        // OnEnemyDead_Local += () => enemyHealth.GetEnemyHealth_UI().gameObject.SetActive(false);
        OnEnemyDead_Local += () => collideHitBox.enabled = false;
        OnEnemyHit_Local += OnEnemyHit_Shaking;
    }



    public override void OnNetworkSpawn()
    {
        if (!IsOwner || !IsServer) return;
        Target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
        CanMove = true;
    }

    public override void OnNetworkDespawn()
    {
    }

    protected virtual void Update()
    {
        delayEnemySpawn -= Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        if (delayEnemySpawn > 0) return;
        if (!IsOwner || !IsServer || !IsSpawned || agent.pathStatus == NavMeshPathStatus.PathInvalid) return;

        if (!agent.isStopped)
        {
            path = new();
            agent.CalculatePath(Target.position, path);
            agent.SetPath(path);
        }
        else
        {
            agent.ResetPath();
        }

    }


    protected virtual void LateUpdate()
    {
        if (delayEnemySpawn > 0) return;
        if (!IsOwner || !IsServer || !IsSpawned) return;

        Vector3 direction = agent.velocity.normalized;
        if (agent.isStopped)
        {
            direction = (Target.position - transform.position).normalized;
        }
        else if (direction != Vector3.zero && CanMove)
        {
            Vector3 forward = enemyRb.transform.forward;
            enemyRb.transform.forward = new(forward.x, direction.y, forward.z);
        }
    }

    protected virtual void OnEnable()
    {
    }

    public void AnimationEvent_OnEnemyAttackHandler()
    {
        OnEnemyAttack_Local?.Invoke();
    }

    public void OnEnemyDead_Dissolve()
    {
        mesh.gameObject.layer = 0;
        dissolveMaterial.DOFloat(1, "_Dissolve", 2).SetEase(Ease.OutSine);
    }

    public void OnEnemyHit_Shaking()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(mesh_parent.DOShakePosition(0.2f, strength: 0.5f, vibrato: 50));
        sequence.Play();
    }

    [ClientRpc]
    public virtual void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if (damage.AttackerClientId == (long)NetworkManager.LocalClientId) return;

        if (enemyHealth.IsDead)
        {
            StopMoving();
            hitBox.enabled = false;
            critHitBox.enabled = false;
            OnEnemyDead_Local?.Invoke();
        }
        else
        {
            OnEnemyHit_Local?.Invoke();
        }
    }

    public void TakeDamage(AttackDamage damage)
    {
        TakeDamage_ServerRpc(damage);

        if (enemyHealth.IsDead)
        {
            StopMoving();
            hitBox.enabled = false;
            critHitBox.enabled = false;
            OnEnemyDead_Local?.Invoke();
            OnEnemyDead_Local = null;
        }
        else
        {
            OnEnemyHit_Local?.Invoke();
        }
    }

    [ClientRpc]
    public virtual void TakeHeal_ClientRpc(AttackDamage damage)
    {
        // if (!IsOwner) return;
        enemyHealth.TakeHeal(damage);
    }

    [ServerRpc]
    public virtual void TakeDamage_ServerRpc(AttackDamage damage)
    {
        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());

        TakeDamage_ClientRpc(damage);
    }
    public void StopMoving()
    {
        if (!IsOwner) return;
        CanMove = false;
        agent.isStopped = true;
        animator.SetFloat("VelocityZ", 0);

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTaunt_ServerRpc(NetworkObjectReference tauntTarget)
    {
        if (tauntTarget.TryGet(out NetworkObject networkObject))
        {
            Target = networkObject.transform;
            IsTaunted = true;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishTaunt_ServerRpc()
    {
        Target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
        IsTaunted = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartStun_ServerRpc()
    {
        if (!stunImmunity)
        {
            StopMoving();
            IsStun = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishStun_ServerRpc()
    {
        if (!stunImmunity)
        {
            Moving();
            IsStun = false;
        }
    }
    public void Moving()
    {
        if (!IsOwner) return;
        CanMove = true;
        agent.isStopped = false;
    }
    [ServerRpc]
    public virtual void TakeHeal_ServerRpc(AttackDamage damage)
    {
        TakeHeal_ClientRpc(damage);
    }
    public IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
