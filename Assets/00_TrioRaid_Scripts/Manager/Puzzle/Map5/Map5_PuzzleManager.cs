using System;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map5_PuzzleManager : NetworkSingleton<Map5_PuzzleManager>
{
    public Action<Map5_GameState> OnStateChanged_Local;

    [EnumToggleButtons]
    [SerializeField] private Map5_GameState currentState = Map5_GameState.Phase1_Idle;
    [SerializeField] private float delayPanBossCamToPlayer = 3;


    [FoldoutGroup("Reference")]
    [SerializeField] private CinemachineVirtualCamera bossCam;

    [FoldoutGroup("Reference")]
    [SerializeField] private RedDragon_Fly_EnemyController bossController;

    [FoldoutGroup("Reference")]
    // [SerializeField] private BrokenBalistaController balistaController;

    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        OnStateChanged_Local += SpawnBoss;

    }

    // [ServerRpc(RequireOwnership = false)]
    private void SpawnBoss(Map5_GameState newState)
    {
        if (newState != Map5_GameState.Phase2_RepairBalista)
        {
            return;
        }

        // bossController.Target = balistaController.transform;
        // boss.SetActive(true);
        PanCameraToBoss();

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetState_ServerRpc(Map5_GameState newState)
    {
        SetState_ClientRpc(newState);
    }

    [ClientRpc]
    private void SetState_ClientRpc(Map5_GameState newState)
    {
        currentState = newState;
        OnStateChanged_Local?.Invoke(currentState);
    }

    [ClientRpc]
    private void PanCameraToBoss_ClientRpc()
    {
        PanCameraToBoss();
    }

    private void PanCameraToBoss()
    {
        bossCam.Priority = 1000;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2);
        sequence.AppendCallback(() =>
        {
        });
        sequence.AppendInterval(delayPanBossCamToPlayer);
        sequence.OnComplete(() =>
        {
            bossCam.Priority = 0;
        });
        sequence.Play();
    }
}

public enum Map5_GameState
{
    Phase1_Idle,
    Phase2_RepairBalista,
}

