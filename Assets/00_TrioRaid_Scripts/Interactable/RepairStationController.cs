using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class RepairStationController : NetworkBehaviour
{
    NetworkVariable<bool> isAllCollected = new(false);
    [FoldoutGroup("Config")][ShowInInspector] public bool IsAllCollected => isAllCollected.Value;

    [FoldoutGroup("Config")][ShowInInspector] float collectProgress;
    [FoldoutGroup("Config")][ShowInInspector] public float CollectProgress => collectProgress;

    [FoldoutGroup("Config")][SerializeField] float maxCollectProgress = 100;

    [FoldoutGroup("Config")][ShowInInspector] NetworkList<ulong> collectingPlayer;

    [FoldoutGroup("Config")][Min(0)][SerializeField] float collectingSpeed = 1;
    [FoldoutGroup("Config")][SerializeField] int progressRewardAmount = 5;
    [FoldoutGroup("Config")][SerializeField] NetworkVariable<int> remainingProgressRewardAmount = new(20);
    int RemainingProgressRewardAmount => remainingProgressRewardAmount.Value;


    [FoldoutGroup("Reference")][SerializeField] GameObject canvas;
    [FoldoutGroup("Reference")][SerializeField] List<GameObject> woods;
    [FoldoutGroup("Reference")][SerializeField] Slider progressSlider;
    [FoldoutGroup("Reference")][SerializeField] TextMeshProUGUI collectButtonText;
    string originalCollectButtonText;
    [FoldoutGroup("Reference")][SerializeField] Collider triggerCol;
    OutlineController outlineController;

    private void Awake()
    {
        collectingPlayer = new();
        originalCollectButtonText = collectButtonText.text;
        triggerCol = GetComponent<Collider>();
        outlineController = GetComponent<OutlineController>();
    }

    private void Start()
    {
        outlineController.HideOutline();
        
        isAllCollected.OnValueChanged += CheckEnableRepairStation;
    }

    private void OnEnable()
    {
        Map5_PuzzleManager.Instance.OnStateChanged_Local += CheckEnableOnStateChanged;
    }

    private void OnDisable()
    {
        Map5_PuzzleManager.Instance.OnStateChanged_Local -= CheckEnableOnStateChanged;
    }



    public override void OnNetworkSpawn()
    {
    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        canvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        StopCollecting();
        canvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        if (Input.GetKeyDown(KeyCode.F) && !IsAllCollected)
        {
            StartCollecting();
        }

        if (Input.GetKey(KeyCode.F) && !IsAllCollected)
        {
            Collecting();
        }

        if (Input.GetKeyUp(KeyCode.F) && !IsAllCollected)
        {
            StopCollecting();
        }
    }




    private void CheckEnableRepairStation(bool prevValue, bool newValue)
    {
        if (newValue)
        {
            EnableRepairStation();
        }
        else
        {
            DisableRepairStation();
        }
    }

    private void CheckEnableOnStateChanged(Map5_GameState newState)
    {
        if (newState == Map5_GameState.Phase2_RepairBalista)
        {
            EnableRepairStation();
        }
        else
        {
            DisableRepairStation();
        }
    }


    public void EnableRepairStation()
    {
        triggerCol.enabled = true;

        outlineController.ShowOutline();

        foreach (GameObject wood in woods)
        {
            wood.SetActive(true);
        }
        canvas.SetActive(true);

    }

    public void DisableRepairStation()
    {
        triggerCol.enabled = false;

        outlineController.HideOutline();

        foreach (GameObject wood in woods)
        {
            wood.SetActive(false);
        }

        StopCollecting();
        canvas.SetActive(false);

    }

    public void StartCollecting()
    {
        collectButtonText.text = "กำลังรวบรวมวัสดุ";
        StartCollecting_ServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartCollecting_ServerRpc(ulong clientId)
    {
        if (!collectingPlayer.Contains(clientId))
        {
            collectingPlayer.Add(clientId);
        }
    }

    public void StopCollecting()
    {
        collectButtonText.text = originalCollectButtonText;
        StopCollecting_ServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopCollecting_ServerRpc(ulong clientId)
    {
        if (collectingPlayer.Contains(clientId))
        {
            collectingPlayer.Remove(clientId);
        }
    }

    public void Collecting()
    {
        UpdateProgress(Time.deltaTime * collectingSpeed);
        UpdateProgressBar(CollectProgress);
    }

    private void UpdateProgressBar(float progress)
    {
        progressSlider.value = progress / maxCollectProgress;
    }


    private void UpdateProgress(float progress)
    {
        collectProgress += progress;

        CheckCollectingProgress();
    }

    private void CheckCollectingProgress()
    {
        // finish collected

        if (CollectProgress >= maxCollectProgress)
        {
            collectProgress = 0;

            GiveRewardProgress_ServerRpc(NetworkManager.LocalClientId);

            if (RemainingProgressRewardAmount <= 0)
            {
                remainingProgressRewardAmount.Value = 0;
                isAllCollected.Value = true;
                DisableRepairStation();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GiveRewardProgress_ServerRpc(ulong clientId)
    {
        remainingProgressRewardAmount.Value -= progressRewardAmount;
        GiveRewardProgress_ClientRpc(clientId);
    }

    [ClientRpc]
    private void GiveRewardProgress_ClientRpc(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;

        Map5_ProgressInventoryManager.Instance.GetProgess(progressRewardAmount);
    }
}
