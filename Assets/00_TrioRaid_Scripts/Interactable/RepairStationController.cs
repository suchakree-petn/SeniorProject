using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class RepairStationController : NetworkBehaviour
{
    private NetworkVariable<bool> isCollected = new(false);
    [ShowInInspector] public bool IsCollected => isCollected.Value;

    private NetworkVariable<float> collectProgress = new(0);
    [ShowInInspector] public float CollectProgress => collectProgress.Value;

    [SerializeField] private float maxCollectProgress = 100;

    [ShowInInspector] private NetworkList<ulong> collectingPlayer;

    [Min(0)]
    [SerializeField] private float collectingSpeed = 1;

    [SerializeField] private float progressReward = 30;



    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject canvas;

    [FoldoutGroup("Reference")]
    [SerializeField] private Slider progressSlider;

    [FoldoutGroup("Reference")]
    [SerializeField] private TextMeshProUGUI collectButtonText;
    private string originalCollectButtonText;

    [FoldoutGroup("Reference")]
    [SerializeField] private Collider triggerCol;

    [FoldoutGroup("Reference")]
    [SerializeField] private Outline[] outlines;

    private void Awake()
    {
        collectingPlayer = new();
        originalCollectButtonText = collectButtonText.text;
        triggerCol = GetComponent<Collider>();
        outlines = transform.GetComponentsInChildren<Outline>();
    }

    private void OnEnable()
    {
        Map5_PuzzleManager.Instance.OnStateChanged_Local += CheckEnable;
        Map5_PuzzleManager.Instance.OnStateChanged_Local += CheckShowOutline;
    }
    
    private void OnDisable()
    {
        Map5_PuzzleManager.Instance.OnStateChanged_Local -= CheckEnable;
        Map5_PuzzleManager.Instance.OnStateChanged_Local -= CheckShowOutline;
    }



    public override void OnNetworkSpawn()
    {
        collectProgress.OnValueChanged += ProgressBar;
    }

    private void CheckShowOutline(Map5_GameState state)
    {
        if (state == Map5_GameState.Phase2_RepairBalista)
        {
            ShowOutline();
        }
        else
        {
            HideOutline();
        }
    }
    private void ShowOutline()
    {
        foreach (var outline in outlines)
        {
            outline.enabled = true;
        }
    }

    private void HideOutline()
    {
        foreach (var outline in outlines)
        {
            outline.enabled = false;
        }
    }

    private void CheckEnable(Map5_GameState newState)
    {
        if (newState == Map5_GameState.Phase2_RepairBalista)
        {
            triggerCol.enabled = false;
        }
    }

    private void ProgressBar(float previousValue, float newValue)
    {
        progressSlider.value = newValue / maxCollectProgress;
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

        if (Input.GetKeyDown(KeyCode.F) && !IsCollected)
        {
            StartCollecting();
        }

        if (Input.GetKey(KeyCode.F) && !IsCollected)
        {
            Collecting();
        }

        if (Input.GetKeyUp(KeyCode.F) && !IsCollected)
        {
            StopCollecting();
        }
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
        Collecting_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void Collecting_ServerRpc()
    {
        UpdateProgress(Time.deltaTime * collectingSpeed * collectingPlayer.Count);
    }

    private void UpdateProgress(float progress)
    {
        collectProgress.Value += progress;

        if (CollectProgress >= maxCollectProgress)
        {
            collectProgress.Value = maxCollectProgress;
            isCollected.Value = true;
            // finish collected
            GiveRewardProgress_ServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GiveRewardProgress_ServerRpc()
    {
        GiveRewardProgress_ClientRpc();
    }

    [ClientRpc]
    private void GiveRewardProgress_ClientRpc()
    {
    }
}
