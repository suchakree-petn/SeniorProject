using System;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map5_RepairBalistaManager : NetworkSingleton<Map5_RepairBalistaManager>
{
    public float HoldProgress_Local = 0;

    [FoldoutGroup("Reference")]
    [SerializeField] private CinemachineVirtualCamera bossCam;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform bossSpawn;

    protected override void InitAfterAwake()
    {
    }

    // private void Start()
    // {
    //     OnStateChanged_Local += SpawnBoss_ServerRpc;

    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void SpawnBoss_ServerRpc(Map5_GameState newState)
    // {
    //     if (newState != Map5_GameState.Phase2_RepairBalista)
    //     {
    //         return;
    //     }
    //     if (NetworkManager.IsServer)
    //     {
    //         EnemyManager.Instance.Spawn(2004);
    //         PanCameraToBoss_ClientRpc();
    //     }
    // }

    // public override void OnNetworkSpawn()
    // {
    //     if (!IsServer) return;

    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void SetState_ServerRpc(Map5_GameState newState)
    // {
    //     SetState_ClientRpc(newState);
    // }

    // [ClientRpc]
    // private void SetState_ClientRpc(Map5_GameState newState)
    // {
    //     currentState = newState;
    //     OnStateChanged_Local?.Invoke(currentState);
    // }

    // [ClientRpc]
    // private void PanCameraToBoss_ClientRpc()
    // {
    //     bossCam.Priority = 1000;
    //     Sequence sequence = DOTween.Sequence();
    //     sequence.AppendInterval(2);
    //     sequence.AppendCallback(() =>
    //     {
    //     });
    //     sequence.AppendInterval(delayPanBossCamToPlayer);
    //     sequence.OnComplete(() =>
    //     {
    //         bossCam.Priority = 0;
    //     });
    //     sequence.Play();
    // }
}


