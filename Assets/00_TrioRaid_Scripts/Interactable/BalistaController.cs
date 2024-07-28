using System;
using Cinemachine;
using DG.Tweening;
using Gamekit3D;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class BalistaController : NetworkBehaviour
{
    [Header("Broken")]
    public Action OnRepairSuccess;

    [SerializeField] private NetworkVariable<float> repairProgress = new(0);
    public float RepairProgress => repairProgress.Value;
    public float RepairMaxProgress = 100;
    public bool IsRepaired => RepairProgress >= RepairMaxProgress;


    [Header("Completed")]
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;
    public float RotateSpeed = 10;
    public float MaxCameraPitch = 60;
    float cameraPitch;

    public PlayerController UsingPlayer;

    [Header("Refererence")]
    [SerializeField] private TextMeshPro text_interactButton;
    [SerializeField] private InteractOnButton use_interactButton;
    [SerializeField] private InteractOnButton exit_interactButton;
    [SerializeField] private Transform povTransform;
    [SerializeField] private CinemachineVirtualCamera povCam;
    [SerializeField] private Animator animator;

    private void Start()
    {
        text_interactButton.SetText("<color=#ffa500ff> F </color> Repair");

        OnRepairSuccess += ()=> text_interactButton.SetText("<color=#ffa500ff> F </color> Use Balista");
    }

    private void Update()
    {
        animator.SetFloat("RepairProgress", RepairProgress / RepairMaxProgress);
    }

    private void LateUpdate()
    {
        if (!IsInUse) return;

        Vector2 moveVector = PlayerInputManager.Instance.MovementAction.ReadValue<Vector2>();

        moveVector *= RotateSpeed;

        float eulerAngleX = povTransform.eulerAngles.x + moveVector.y;
        cameraPitch -= moveVector.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -MaxCameraPitch, 0);
        povTransform.localRotation = Quaternion.Euler(cameraPitch, 90, 0);

        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + moveVector.x, 0);
    }

    private void OnEnable()
    {
        use_interactButton.OnButtonPress.AddListener(UseBalista);
        use_interactButton.OnEnter.AddListener(ShowInteractText);
        use_interactButton.OnExit.AddListener(HideInteractText);

        exit_interactButton.OnButtonPress.AddListener(ExitBalista);
    }

    private void OnDisable()
    {
        use_interactButton.OnButtonPress.RemoveListener(UseBalista);
        use_interactButton.OnEnter.RemoveListener(ShowInteractText);
        use_interactButton.OnExit.RemoveListener(HideInteractText);

        exit_interactButton.OnButtonPress.RemoveListener(ExitBalista);
    }

    public void AddProgress(float value)
    {
        text_interactButton.SetText("<color=#ffa500ff> F </color> Repair");

    }

    private void ExitBalista()
    {
        if (IsInUse)
        {
            povCam.Priority = -int.MaxValue;

            ShowInteractText();
            PlayerUIManager.Instance.SetPlayerCrossHairState(false);

            ExitBalista_ServerRpc();

            UsingPlayer.SetCanPlayerMove(true);
            UsingPlayer.SetIsReadyToAttack(true);
            UsingPlayer.SetCanUseAbilityE(true);
            UsingPlayer.SetCanUseAbilityQ(true);
            UsingPlayer.SetPlayerVisible(true);
        }

    }

    private void UseBalista()
    {
        if (!IsInUse)
        {
            povCam.Priority = int.MaxValue;

            HideInteractText();
            PlayerUIManager.Instance.SetPlayerCrossHairState(true);

            UseBalista_ServerRpc(NetworkManager.LocalClientId);

            UsingPlayer = PlayerManager.Instance.LocalPlayerController;
            UsingPlayer.SetCanPlayerMove(false);
            UsingPlayer.SetIsReadyToAttack(false);
            UsingPlayer.SetCanUseAbilityE(false);
            UsingPlayer.SetCanUseAbilityQ(false);
            UsingPlayer.SetPlayerVisible(false);
        }

    }


    [ServerRpc(RequireOwnership = false)]
    private void ExitBalista_ServerRpc()
    {
        if (IsInUse)
        {
            isInUse.Value = false;
            Debug.Log("Exit Balista");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UseBalista_ServerRpc(ulong userClientId)
    {
        if (!IsInUse)
        {
            isInUse.Value = true;
            // PlayerController userController = PlayerManager.Instance.PlayerControllers[userClientId];
            // userController.SetCanPlayerMove_ClienRpc(false, userClientId);
            // userController.SetIsReadyToAttack_ClientRpc(false, userClientId);
            // userController.SetCanUseAbilityE_ClientRpc(false, userClientId);
            // userController.SetCanUseAbilityQ_ClientRpc(false, userClientId);
            Debug.Log("Using Balista");
        }
    }


    private void HideInteractText()
    {
        text_interactButton.gameObject.SetActive(false);
    }

    private void ShowInteractText()
    {
        text_interactButton.gameObject.SetActive(true);
    }
}
