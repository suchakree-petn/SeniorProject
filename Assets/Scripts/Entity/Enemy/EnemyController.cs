using System;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : NetworkBehaviour, IDamageable
{
    [SerializeField] protected EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;

    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected NavMeshPath path;
    [SerializeField] protected BehaviourTreeInstance behaviourTreeInstance;

    public Transform target;
    public bool CanMove = true;

    [Header("Reference")]
    public EnemyHealth enemyHealth;
    public Rigidbody enemyRb;
    public NetworkAnimator networkAnimator;
    public Animator animator;
    public Collider hitBox;
    public Collider critHitBox;

    protected virtual void Awake()
    {
        if (!IsOwner) return;
        enemyRb = GetComponent<Rigidbody>();
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

    [ClientRpc]
    public virtual void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;
        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());

    }

    [ClientRpc]
    public virtual void TakeHeal_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;
        enemyHealth.TakeHeal(damage);
    }

    [ServerRpc]
    public virtual void TakeDamage_ServerRpc(AttackDamage damage)
    {
        TakeDamage_ClientRpc(damage);
        if (enemyHealth.IsDead)
        {
            hitBox.enabled = false;
            critHitBox.enabled = false;
        }
    }
    public void StopMoving()
    {
        if (!IsOwner) return;
        Debug.Log("Stop moving");
        CanMove = false;
        agent.isStopped = true;
        animator.SetFloat("VelocityZ", 0);

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
        Debug.Log($"Start delay destroy");
        yield return new WaitForSeconds(time);
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
