using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : NetworkBehaviour, IDamageable
{
    [SerializeField] private EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshPath path;
    [SerializeField] private NavMeshTriangulation Triangulation;
    [SerializeField] private BehaviourTreeInstance behaviourTreeInstance;

    public Transform target;
    private Vector3[] nextTargetPos = new Vector3[20];
    public float moveSpeed = 10;
    private Coroutine findPathToTarget;

    [Header("Reference")]
    public EnemyHealth enemyHealth;
    public Rigidbody enemyRb;
    protected virtual void Awake()
    {
        enemyRb = GetComponent<Rigidbody>();
        // Triangulation = NavMesh.CalculateTriangulation();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner || !IsServer) return;
        target = PlayerManager.Instance.PlayerGameObjects[0].transform;
        // if (findPathToTarget != null)
        // {
        //     StopCoroutine(findPathToTarget);
        // }

        // findPathToTarget = StartCoroutine(CalcPathToTartget());
    }

    public override void OnNetworkDespawn()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (!IsOwner || !IsServer || !IsSpawned) return;
        path = new();
        agent.CalculatePath(target.position, path);
        agent.SetPath(path);
        // if (Vector3.Distance(transform.position, target.position) > 3f)
        // {
        //     Vector3 direction = nextTargetPos[0] - transform.position;
        //     enemyRb.velocity = direction.normalized * moveSpeed;
        // }
        // else
        // {
        //     // Attack
        // }

    }

    protected virtual void LateUpdate()
    {
        if (!IsOwner || !IsServer || !IsSpawned) return;

        Vector3 direction = nextTargetPos[0] - transform.position;
        enemyRb.transform.rotation = Quaternion.LookRotation(direction);

    }

    private IEnumerator CalcPathToTartget()
    {
        WaitForSeconds wait = new(0.3f);
        path = new();
        Debug.Log("asdasdasd");

        while (Vector3.Distance(transform.position, target.position) > 3f)
        {
            Debug.Log("Calc");
            UpdateNextPosition();
        }

        yield return wait;
        findPathToTarget = StartCoroutine(CalcPathToTartget());
    }

    public void UpdateNextPosition()
    {
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            nextTargetPos = path.corners;
            Debug.Log(path.corners[0]);
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
            }
        }
        Debug.LogWarning("No path");
    }

    [ClientRpc]
    public virtual void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;

        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());
    }

    public virtual void InitHp(EntityCharacterData characterData)
    {
        enemyHealth.InitHp(characterData);
    }
    protected virtual void OnEnable()
    {
        InitHp(EnemyCharacterData);
    }

    [ClientRpc]
    public virtual void TakeHeal_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner) return;
        enemyHealth.TakeHeal(damage);
    }
}
