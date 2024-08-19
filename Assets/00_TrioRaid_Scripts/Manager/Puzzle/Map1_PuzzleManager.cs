using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map1_PuzzleManager : NetworkSingleton<Map1_PuzzleManager>
{
    public Action OnActivatedPadCountChanged_Server;
    public Action OnAliveEnemyChanged_Local;

    [Min(0)]
    public int ActivatedPadCount;

    [Min(0)]
    public int EnemyWaveCount = 3;

    [SerializeField] private float delayBossSpawn = 3;

    [SerializeField] private int enemyCount = 18;
    [SerializeField] private int aliveEnemyCount = 0;


    [FoldoutGroup("Reference")]
    [SerializeField] private GateController gateController;

    [FoldoutGroup("Reference")]
    [SerializeField] private List<Transform> spawnPos;

    [FoldoutGroup("Reference")]
    [SerializeField] private Collider battleBlocker;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform bossSpawnPos;


    protected override void InitAfterAwake()
    {

    }

    private void Start()
    {
        OnAliveEnemyChanged_Local += CheckAllEnemyDead_Server;
        OnAliveEnemyChanged_Local += CheckEnemyWave_Server;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        OnActivatedPadCountChanged_Server += OnCheckCondition_Server;

    }


    private void OnCheckCondition_Server()
    {
        if (ActivatedPadCount >= 2)
        {
            gateController.OpenGate();
        }
        else
        {
            gateController.CloseGate();
        }
    }

    public void AddActivatePadCount(int count)
    {
        Debug.Log("Add");
        AddActivatePadCount_ServerRpc(count);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddActivatePadCount_ServerRpc(int count)
    {
        ActivatedPadCount += count;
        OnActivatedPadCountChanged_Server?.Invoke();
    }

    public void RemoveActivatePadCount(int count)
    {
        Debug.Log("Remove");

        RemoveActivatePadCount_ServerRpc(count);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveActivatePadCount_ServerRpc(int count)
    {
        ActivatedPadCount -= count;
        OnActivatedPadCountChanged_Server?.Invoke();
    }

    [ServerRpc]
    public void SpawnMobs_ServerRpc()
    {
        foreach (Transform position in spawnPos)
        {
            if (enemyCount > 0)
            {
                EnemyManager.Instance.Spawn(2001, position.position);
                aliveEnemyCount++;
                enemyCount--;
            }
            else
            {
                break;
            }
        }
        OnEnemyDead_RemoveAliveEnemy_ClientRpc();
    }

    [ClientRpc]
    private void OnEnemyDead_RemoveAliveEnemy_ClientRpc()
    {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();

        foreach (EnemyController enemyController in enemyControllers)
        {
            enemyController.OnEnemyDead_Local -= RemoveAliveEnemy;
            enemyController.OnEnemyDead_Local += RemoveAliveEnemy;
        }
    }

    private void RemoveAliveEnemy()
    {
        Debug.Log("Remove alive enemy");
        aliveEnemyCount--;
        OnAliveEnemyChanged_Local?.Invoke();
    }

    private void CheckAllEnemyDead_Server()
    {
        if (!NetworkManager.IsServer) return;
        
        if (aliveEnemyCount == 0 && EnemyWaveCount == 0)
        {
            DisableBattleBlocker_ClientRpc();
        }
        else
        {
            EnableBattleBlocker_ClientRpc();
        }
    }

    [ClientRpc]
    private void DisableBattleBlocker_ClientRpc()
    {
        battleBlocker.enabled = false;

    }

    [ClientRpc]
    private void EnableBattleBlocker_ClientRpc()
    {
        battleBlocker.enabled = true;

    }

    private void CheckEnemyWave_Server()
    {
        if (EnemyWaveCount > 0)
        {
            if (aliveEnemyCount < spawnPos.Count / 2)
            {
                EnemyWaveCount--;

                if (NetworkManager.IsServer)
                {
                    SpawnMobs_ServerRpc();
                }
            }
        }
    }

    public void EncounterBoss_Server()
    {
        Invoke(nameof(BossSpawn_Server), delayBossSpawn);
    }

    private void BossSpawn_Server()
    {
        EnemyManager.Instance.Spawn(2002, bossSpawnPos.position);
    }
}

