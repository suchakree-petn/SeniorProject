using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkSingleton<EnemyManager>
{
    private static Dictionary<ulong, Transform> _enemyCharacterPrefab;
    public static Dictionary<ulong, Transform> EnemyCharacterPrefab
    {
        get
        {
            if (_enemyCharacterPrefab == null)
            {
                _enemyCharacterPrefab = new();
                Transform[] enemyChar = Resources.LoadAll<Transform>("Prefab/Entity/Enemy");
                _enemyCharacterPrefab = enemyChar.ToDictionary(keys => ulong.Parse(keys.name.Split("_")[0]), val => val);
            }
            return _enemyCharacterPrefab;
        }
    }
    
    protected override void InitAfterAwake()
    {
    }
 

    public void Spawn(ulong id, Vector3 position = default)
    {
        Spawn_ServerRpc(id, position);
    }
    [ServerRpc(RequireOwnership = false)]
    private void Spawn_ServerRpc(ulong id, Vector3 position = default)
    {
        GameObject gameObject = Instantiate(EnemyCharacterPrefab[id], position, Quaternion.identity).gameObject;
        gameObject.GetComponent<NetworkObject>().Spawn(true);
    }

}
// public float currentHp;
// public EnemyData enemyData;
// [Header("Dead Setting")]
// public Action<GameObject> OnEnemyDead;
// public Action<GameObject, DamageDeal> OnEnemyTakeDamage;
// public AnimationClip enemyDeadClip;
// public override EnemyData GetCharactorData()
// {
//     return enemyData;
// }

// public override void TakeDamage(DamageDeal damage)
// {
//     float damageDeal = 0;
//     if (currentHp > 0)
//     {
//         EnemyData enemyData = GetCharactorData();
//         damageDeal = CalcDamageRecieve(enemyData, damage);
//         currentHp -= damageDeal;
//     }
//     OnEnemyTakeDamage?.Invoke(gameObject, damage);
// }
// public void InitHp()
// {
//     currentHp = GetCharactorData().GetMaxHp();
// }


// public override void Dead(GameObject deadCharactor)
// {
//     GameController.Instance.RemoveEnemyDead(deadCharactor);
// }
// public override void CheckDead(GameObject charactor, Elemental damage)
// {
//     //Debug.Log("CheckDead");
//     if (charactor.GetComponent<EnemyManager>().currentHp <= 0)
//     {
//         OnEnemyDead?.Invoke(charactor);
//     }
// }