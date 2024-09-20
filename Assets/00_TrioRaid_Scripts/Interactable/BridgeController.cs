using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class BridgeController : NetworkBehaviour
{
    private NetworkVariable<bool> isRepaired = new(false);
    [ShowInInspector] public bool IsRepaired => isRepaired.Value;

    private NetworkVariable<float> repairProgress = new(0);
    [ShowInInspector] public float RepairProgress => repairProgress.Value;

    [SerializeField] private float maxRepairProgress = 100;

    [ShowInInspector] private NetworkList<ulong> repairingPlayer;

    [Min(0)]
    [SerializeField] private float repairSpeed = 1;
    [SerializeField] private int requirePlayerCount = 2;

    [FoldoutGroup("Reference")]
    [SerializeField] private Collider bridgeBlocker;

    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject bridgeGameObeject;

    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject canvas;

    [FoldoutGroup("Reference")]
    [SerializeField] private Renderer bridgeRenderer;

    [FoldoutGroup("Reference")]
    [SerializeField] private Slider progressSlider;

    [FoldoutGroup("Reference")]
    [SerializeField] private TextMeshProUGUI repairButtonText;
    private string originalRepairButtonText;

    private void Awake()
    {
        repairingPlayer = new();
        originalRepairButtonText = repairButtonText.text;
    }

    public override void OnNetworkSpawn()
    {
        repairProgress.OnValueChanged += BridgeDissolve;
        repairProgress.OnValueChanged += ProgressBar;
    }

    private void ProgressBar(float previousValue, float newValue)
    {
        progressSlider.value = newValue / maxRepairProgress;
    }

    private void BridgeDissolve(float previousValue, float newValue)
    {
        float ratio = 1 - (newValue / maxRepairProgress);
        bridgeRenderer.material.DOFloat(ratio, "_Dissolve", 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger) return;
        if (!other.transform.root.TryGetComponent(out PlayerController playerController)) return;
        if (!playerController.IsLocalPlayer) return;

        canvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.isTrigger) return;
        if (!other.transform.root.TryGetComponent(out PlayerController playerController)) return;
        if (!playerController.IsLocalPlayer) return;

        StopRepairBridge();
        canvas.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.isTrigger) return;
        if (!other.transform.root.TryGetComponent(out PlayerController playerController)) return;
        if (!playerController.IsLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.F) && !IsRepaired)
        {
            StartRepairBridge();
        }

        if (Input.GetKey(KeyCode.F) && !IsRepaired)
        {
            RepairBridge();
        }

        if (Input.GetKeyUp(KeyCode.F) && !IsRepaired)
        {
            StopRepairBridge();
        }
    }

    public void StartRepairBridge()
    {
        repairButtonText.text = "กำลังซ่อมสะพาน";
        StartRepairBridge_ServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartRepairBridge_ServerRpc(ulong clientId)
    {
        if (!repairingPlayer.Contains(clientId))
        {
            repairingPlayer.Add(clientId);
        }
    }

    public void StopRepairBridge()
    {
        repairButtonText.text = originalRepairButtonText;
        StopRepairBridge_ServerRpc(NetworkManager.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopRepairBridge_ServerRpc(ulong clientId)
    {
        if (repairingPlayer.Contains(clientId))
        {
            repairingPlayer.Remove(clientId);
        }
    }

    public void RepairBridge()
    {
        RepairBridge_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RepairBridge_ServerRpc()
    {
        if (repairingPlayer.Count == requirePlayerCount)
        {
            UpdateProgress(Time.deltaTime * repairSpeed);
        }
    }

    private void UpdateProgress(float progress)
    {
        repairProgress.Value += progress;

        if (RepairProgress >= maxRepairProgress)
        {
            repairProgress.Value = maxRepairProgress;
            isRepaired.Value = true;
            ShowBridge_ClientRpc();
        }
    }

    [ClientRpc]
    private void ShowBridge_ClientRpc()
    {
        bridgeBlocker.enabled = false;
        GetComponent<Collider>().enabled = false;
        bridgeGameObeject.SetActive(true);
        canvas.SetActive(false);
    }
}
