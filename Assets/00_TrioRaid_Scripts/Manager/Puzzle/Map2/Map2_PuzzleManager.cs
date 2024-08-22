using Cinemachine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map2_PuzzleManager : NetworkSingleton<Map2_PuzzleManager>
{
    private NetworkVariable<bool> isLocked = new(false);
    [ShowInInspector] public bool IsLocked => isLocked.Value;


    [SerializeField] private float delayPanBossCamToPlayer = 3;



    [FoldoutGroup("Reference")]
    [SerializeField] private GateController gateController;
    [FoldoutGroup("Reference")]
    [SerializeField] private GateController bossGateController;
    [FoldoutGroup("Reference")]
    [SerializeField] private Transform bossSpawnPos;
    [FoldoutGroup("Reference")]
    [SerializeField] private JigsawBoardStandController jigsawBoardStandController;
    [FoldoutGroup("Reference")]
    [SerializeField] private CinemachineVirtualCamera bossCam;
    [FoldoutGroup("Reference")]
    [SerializeField] private Map2_JigsawScannerManager map2_JigsawScannerManager;


    protected override void InitAfterAwake()
    {
    }

    private void Start()
    {
        gateController.OpenGate();
        // Map2_JigsawScannerManager.Instance.gameObject.SetActive(false);

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
            Puzzle2_EnableJigsawScanner_ClientRpc();
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

    [ServerRpc(RequireOwnership = false)]
    public void Puzzle2_EnableJigsawScanner_ServerRpc()
    {
        Puzzle2_EnableJigsawScanner_ClientRpc();
    }

    [ClientRpc]
    private void Puzzle2_EnableJigsawScanner_ClientRpc()
    {
        map2_JigsawScannerManager.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void EncounterBoss_ServerRpc()
    {
        BossSpawn_Server();
    }

    private void BossSpawn_Server()
    {
        EnemyManager.Instance.Spawn(2003, bossSpawnPos.position);
        PanCameraToBoss_ClientRpc();
    }

    [ClientRpc]
    private void PanCameraToBoss_ClientRpc()
    {
        bossCam.Priority = 1000;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2);
        sequence.AppendCallback(() =>
        {
            bossGateController.OpenGate();
        });
        sequence.AppendInterval(delayPanBossCamToPlayer);
        sequence.OnComplete(() =>
        {
            bossCam.Priority = 0;
            gateController.OpenGate();
        });
        sequence.Play();
    }
}

