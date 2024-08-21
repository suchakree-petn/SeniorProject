using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map2_PuzzleManager : NetworkSingleton<Map2_PuzzleManager>
{
    private NetworkVariable<bool> isLocked = new(false);
    [ShowInInspector] public bool IsLocked => isLocked.Value;


    [SerializeField] private float delayBossSpawn = 3;



    [FoldoutGroup("Reference")]
    [SerializeField] private GateController gateController;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform bossSpawnPos;
    [FoldoutGroup("Reference")]
    [SerializeField] private JigsawBoardStandController jigsawBoardStandController;


    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        gateController.OpenGate();

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

    }

    [ServerRpc(RequireOwnership = false)]
    public void Puzzle1_LockPlayerInArea_ServerRpc()
    {
        if (!IsLocked)
        {
            isLocked.Value = true;
            EnablePuzzleBlocker_ClientRpc();
        }
    }


    [ClientRpc]
    private void DisablePuzzleBlocker_ClientRpc()
    {
        gateController.OpenGate();
    }

    [ClientRpc]
    private void EnablePuzzleBlocker_ClientRpc()
    {
        gateController.CloseGate();

    }

    public void Puzzle2_CollectJigsaw(uint jigsawId)
    {
        foreach (var jigsaw in jigsawBoardStandController.CollectedJigsawDict)
        {
            if (jigsaw.Key.JigsawId == jigsawId)
            {
                Debug.Log("Add jigsaw: " + jigsawId);
                jigsawBoardStandController.CollectedJigsawDict[jigsaw.Key] = true;
                return;
            }
        }

        Debug.Log("Cannot add jigsaw: " + jigsawId);
    }


    public void EncounterBoss_Server()
    {
        Invoke(nameof(BossSpawn_Server), delayBossSpawn);
    }

    private void BossSpawn_Server()
    {
        EnemyManager.Instance.Spawn(2003, bossSpawnPos.position);
    }
}

