using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
public class EnemyController : NetworkBehaviour
{
    [SerializeField] private EnemyCharacterData _enemyCharacterData;
    public EnemyCharacterData EnemyCharacterData => _enemyCharacterData;
    // [Header("Reference")]
    // [SerializeField] private BehaviourTreeRunner behaviourTreeRunner;
}
