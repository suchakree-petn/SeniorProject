using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class EnemyManager : NetworkSingleton<EnemyManager>
{
    private Dictionary<ulong, GameObject> _enemies;
    public Dictionary<ulong, GameObject> Enemies
    {
        get
        {
            if (_enemies == null)
            {
                Init_Enemies_Dictionary();
                return _enemies;
            }
            return _enemies;
        }
    }
    public List<GameObject> gameObjects;
    private void Start()
    {

    }

    private void Update()
    {

    }


    public void Spawn(ulong id, Vector3 position = default)
    {
        Spawn_ServerRpc(id, position);
    }
    [ServerRpc(RequireOwnership = false)]
    public void Spawn_ServerRpc(ulong id, Vector3 position = default)
    {
        GameObject gameObject = Instantiate(Instance.Enemies[id], position, Quaternion.identity);
        gameObject.GetComponent<NetworkObject>().Spawn(true);

    }
    protected override void InitAfterAwake()
    {
    }
    private void Init_Enemies_Dictionary()
    {
        _enemies = new();
        GameObject[] enemies_prf = Resources.LoadAll<GameObject>("Entity/Enemy/Prefab");
        foreach (var e in enemies_prf)
        {
            Debug.Log(ulong.Parse(e.name.Split("_")[0]));
        }
        _enemies = enemies_prf.ToDictionary((key) => ulong.Parse(key.name.Split("_")[0]), value => value);
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