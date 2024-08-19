using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map1_PuzzleManager : NetworkSingleton<Map1_PuzzleManager>
{
    public Action OnActivatedPadCountChanged_Server;

    [Min(0)]
    public int ActivatedPadCount;


    [FoldoutGroup("Reference")]
    [SerializeField] private GateController gateController;

    [FoldoutGroup("Reference")]
    [SerializeField] private List<Transform> spawnPos;


    protected override void InitAfterAwake()
    {

    }
    private void Start()
    {
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
            EnemyManager.Instance.Spawn(2001, position.position);
        }
    }
}

