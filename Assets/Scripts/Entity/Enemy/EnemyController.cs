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
    [SerializeField] private BehaviourTreeInstance behaviourTreeInstance;
    public float Radius_X;
    public float Radius_Y;
    public float Radius_Z;
    public float toleranceValue = 1;
    public float acceleration = 60;
    public float maxSpeed = 20;
    public bool IsNavAgent;

    public List<Transform> targetGroup = new();

    [Header("Reference")]
    public EnemyHealth enemyHealth;
    public Rigidbody enemyRb;
    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody>();

    }

    public override void OnNetworkSpawn()
    {
    }

    public override void OnNetworkDespawn()
    {
    }

    private void FixedUpdate()
    {
        if (targetGroup.Count == 0)
        {
            foreach (Transform child in GameObject.Find("TargetGroup").transform)
            {
                targetGroup.Add(child);
            }
        }
        if (Vector3.Distance(transform.position, targetGroup[0].position) > 1f)
        {
            Vector3 direction = targetGroup[0].position - transform.position;
            enemyRb.AddForce(direction.normalized * maxSpeed, ForceMode.Force);
        }
        else
        {
            List<Transform> newTargetGroup = new();
            newTargetGroup = targetGroup;

            Transform firstTransform = newTargetGroup[0];
            newTargetGroup.Add(firstTransform);
            newTargetGroup.RemoveAt(0);
        }

    }

    private void LateUpdate()
    {
        Vector3 direction = targetGroup[0].position - transform.position;
        enemyRb.transform.rotation = Quaternion.LookRotation(direction);
        // rb.transform.rotation = Quaternion.LookRotation(rb.transform.rotation.eulerAngles.x, rb.transform.rotation.eulerAngles.y + 1, rb.transform.rotation.eulerAngles.z);

    }
    [ClientRpc]
    public void TakeDamage_ClientRpc(AttackDamage damage)
    {
        if(!IsOwner) return;

        enemyHealth.TakeDamage(damage, EnemyCharacterData.GetDefense());
    }

    public void InitHp(EntityCharacterData characterData)
    {
        enemyHealth.InitHp(characterData);
    }
    protected virtual void OnEnable()
    {
        InitHp(EnemyCharacterData);
    }

    [ClientRpc]
    public void TakeHeal_ClientRpc(AttackDamage damage)
    {
        if(!IsOwner) return;
        enemyHealth.TakeHeal(damage);
    }
}
