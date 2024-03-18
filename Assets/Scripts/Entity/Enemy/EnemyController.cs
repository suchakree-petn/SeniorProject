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
    public float moveSpeed = 5;
    protected Coroutine findPathToTarget;

    [Header("Reference")]
    public EnemyHealth enemyHealth;
    public Rigidbody enemyRb;
    public NetworkAnimator networkAnimator;
    public Animator animator;

    protected virtual void Awake()
    {
        if(!IsOwner) return;
        enemyRb = GetComponent<Rigidbody>();
        agent.speed = moveSpeed;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner || !IsServer) return;
        target = PlayerManager.Instance.GetClosestPlayerFrom(transform.position);

    }

    public override void OnNetworkDespawn()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (!IsOwner || !IsServer || !IsSpawned) return;

        if (!agent.isStopped)
        {
            path = new();
            agent.CalculatePath(target.position, path);
            agent.SetPath(path);
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

    // private IEnumerator CalcPathToTartget()
    // {
    //     WaitForSeconds wait = new(0.3f);
    //     path = new();
    //     Debug.Log("asdasdasd");

    //     while (Vector3.Distance(transform.position, target.position) > 3f)
    //     {
    //         Debug.Log("Calc");
    //         UpdateNextPosition();
    //     }

    //     yield return wait;
    //     findPathToTarget = StartCoroutine(CalcPathToTartget());
    // }

    // public void UpdateNextPosition()
    // {
    //     if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
    //     {
    //         Debug.Log(path.corners[0]);
    //         for (int i = 0; i < path.corners.Length - 1; i++)
    //         {
    //             Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
    //         }
    //     }
    //     Debug.LogWarning("No path");
    // }

    [ClientRpc]
    public virtual void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;

        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());
    }

    public virtual void InitHp(EntityCharacterData characterData)
    {
        // enemyHealth.InitHp(characterData);
    }
    protected virtual void OnEnable()
    {
        // InitHp(EnemyCharacterData);
    }

    [ClientRpc]
    public virtual void TakeHeal_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;
        enemyHealth.TakeHeal(damage);
    }
}
