using System;
using System.Collections;
using TheKiwiCoder;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
public class EnemyController : NetworkBehaviour, IDamageable
{
    public Action OnEnemyDead;

    [SerializeField] protected EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected NavMeshPath path;
    [SerializeField] protected BehaviourTreeInstance behaviourTreeInstance;

    public Transform target;
    public bool CanMove = true;
    public bool IsTaunted = false;
    public bool IsStun = false;

    [Header("Reference")]
    public EnemyHealth enemyHealth;
    public Rigidbody enemyRb;
    public NetworkAnimator networkAnimator;
    public Animator animator;
    public Collider hitBox;
    public Collider critHitBox;
    [SerializeField] private GameObject mesh;
    private Material dissolveMaterial;

    protected virtual void Awake()
    {
        dissolveMaterial = mesh.GetComponent<Renderer>().material;
        if (!IsOwner) return;
    }

    protected virtual void Start()
    {
    }


    public override void OnNetworkSpawn()
    {
        if (!IsOwner || !IsServer) return;
        target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
        CanMove = true;
    }

    public override void OnNetworkDespawn()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (!IsOwner || !IsServer || !IsSpawned || agent.pathStatus == NavMeshPathStatus.PathInvalid) return;

        if (!agent.isStopped)
        {
            path = new();
            agent.CalculatePath(target.position, path);
            agent.SetPath(path);
        }
        else
        {
            agent.ResetPath();
        }

    }


    protected virtual void LateUpdate()
    {
        if (!IsOwner || !IsServer || !IsSpawned) return;

        Vector3 direction = agent.velocity.normalized;
        if (agent.isStopped)
        {
            direction = (target.position - transform.position).normalized;
        }
        enemyRb.transform.rotation = Quaternion.LookRotation(direction);
    }

    protected virtual void OnEnable()
    {
    }

    public void OnEnemyDead_Dissolve()
    {
        Debug.Log("Dissolve");
        mesh.layer = 0;
        dissolveMaterial.DOFloat(1, "_Dissolve", 2).SetEase(Ease.OutSine);
    }

    [ClientRpc]
    public virtual void TakeDamage_ClientRpc(AttackDamage damage)
    {
        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());
        Debug.Log("CurrentHealth: " + enemyHealth.CurrentHealth);
        if (enemyHealth.IsDead)
        {
            StopMoving();
            hitBox.enabled = false;
            critHitBox.enabled = false;
            OnEnemyDead?.Invoke();
            Debug.Log("OnEnemyDead fired");
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
        TakeDamage_ClientRpc(damage);
    }
    public void StopMoving()
    {
        if (!IsOwner) return;
        Debug.Log("Stop moving");
        CanMove = false;
        agent.isStopped = true;
        animator.SetFloat("VelocityZ", 0);

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTaunt_ServerRpc(NetworkObjectReference tauntTarget)
    {
        Debug.Log(gameObject.name + " are taunted");
        if (tauntTarget.TryGet(out NetworkObject networkObject))
        {
            target = networkObject.transform;
            IsTaunted = true;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishTaunt_ServerRpc()
    {
        target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);
        IsTaunted = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartStun_ServerRpc()
    {
        Debug.Log(gameObject.name + " are stun");
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
        Debug.Log("Moving");
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
