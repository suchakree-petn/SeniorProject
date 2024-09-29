using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using Sirenix.OdinInspector;
using FIMSpace.FProceduralAnimation;

public class EnemyController : NetworkBehaviour, IDamageable
{
    public Action OnEnemyDead_Local;
    public Action OnEnemyHit_Local;
    public Action OnEnemyAttack_Local;

    [FoldoutGroup("Config"),SerializeField] protected EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;

    [FoldoutGroup("Config")] public Transform Target;
    [FoldoutGroup("Config")] public bool CanMove = true;
    [FoldoutGroup("Config")] public bool IsTaunted = false;
    [FoldoutGroup("Config")] public bool IsStun = false;
    [FoldoutGroup("Config")] public bool IsDead => enemyHealth.IsDead;
    [FoldoutGroup("Config")] private float delayEnemySpawn = 1;

    [FoldoutGroup("AI")][InlineEditor,SerializeField] protected NavMeshAgent agent;
    [FoldoutGroup("AI")][SerializeField] protected NavMeshPath path;

    [FoldoutGroup("Reference")][InlineEditor,SerializeField] protected EnemyHealth enemyHealth;
    [FoldoutGroup("Reference")][InlineEditor,SerializeField] protected Rigidbody enemyRb;
    [FoldoutGroup("Reference")][InlineEditor,SerializeField] protected NetworkAnimator networkAnimator;
    [FoldoutGroup("Reference")][InlineEditor,SerializeField] protected Animator animator;
    [FoldoutGroup("Reference")][InlineEditor,SerializeField] protected LegsAnimator legsAnimator;

    [FoldoutGroup("Collider")][InlineEditor,SerializeField] protected Collider hitBox;
    [FoldoutGroup("Collider")][InlineEditor,SerializeField] protected Collider collideHitBox;
    [FoldoutGroup("Collider")][InlineEditor,SerializeField] protected Collider critHitBox;

    [FoldoutGroup("Mesh & Material")][InlineEditor,SerializeField] protected Renderer mesh;
    [FoldoutGroup("Mesh & Material")][InlineEditor,SerializeField] protected Transform mesh_parent;
    [FoldoutGroup("Mesh & Material")][InlineEditor,SerializeField] protected Material dissolveMaterial;
    protected OutlineController outlineController;

    protected virtual void Awake()
    {
        dissolveMaterial = mesh.material;
        outlineController = GetComponent<OutlineController>();
    }

    protected virtual void Start()
    {
        // OnEnemyDead_Local += () => enemyHealth.GetEnemyHealth_UI().gameObject.SetActive(false);
        OnEnemyDead_Local += () => collideHitBox.enabled = false;
        OnEnemyDead_Local += outlineController.HideOutline;
        OnEnemyHit_Local += OnEnemyHit_Shaking;

        outlineController.ShowOutline();

    }



    public override void OnNetworkSpawn()
    {
        if (!IsSpawned || !IsServer) return;
        CanMove = true;
    }

    public override void OnNetworkDespawn()
    {
    }

    protected virtual void Update()
    {

        if (!Target && IsSpawned)
        {
            Target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
        }
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
        StopMoving();
        IsStun = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishStun_ServerRpc()
    {
        Moving();
        IsStun = false;
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
