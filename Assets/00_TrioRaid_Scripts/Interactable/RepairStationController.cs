using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        collectingPlayer = new();
        originalCollectButtonText = collectButtonText.text;
    }

    public override void OnNetworkSpawn()
    {
        collectProgress.OnValueChanged += ProgressBar;
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
