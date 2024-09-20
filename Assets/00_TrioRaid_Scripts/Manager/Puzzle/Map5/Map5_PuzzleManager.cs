using System;
using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map5_PuzzleManager : NetworkSingleton<Map5_PuzzleManager>
{
    public Action<Map5_GameState> OnStateChanged_Local;
    public Action OnFinishPanBossCamera_Local;

    [EnumToggleButtons]
    [SerializeField] private Map5_GameState currentState = Map5_GameState.Phase1_Idle;
    public Map5_GameState CurrentState => currentState;

    [SerializeField] private float delayPanBossCamToPlayer = 3;
    public RedDragon_Fly_EnemyController BossController => bossController;
    public Broken_BalistaController Broken_BalistaController => broken_BalistaController;


    [FoldoutGroup("Reference")]
    [SerializeField] private CinemachineVirtualCamera bossCam;

    [FoldoutGroup("Reference")]
    [SerializeField] private RedDragon_Fly_EnemyController bossController;

    [FoldoutGroup("Reference")]
    [SerializeField] private Broken_BalistaController broken_BalistaController;

    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        OnStateChanged_Local += SpawnBoss;

    }

    private void SpawnBoss(Map5_GameState newState)
    {
        if (newState != Map5_GameState.Phase2_RepairBalista)
        {
            return;
        }

        bossController.ActiveBoss();
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
            OnFinishPanBossCamera_Local?.Invoke();
        });
        sequence.Play();
    }
}

public enum Map5_GameState
{
    Phase1_Idle,
    Phase2_RepairBalista,
}

