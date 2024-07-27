using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Gamekit3D;
using Gamekit3D.GameCommands;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BalistaController : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;
    public float RotateSpeed = 10;
    public float MaxCameraPitch = 60;
    float cameraPitch;

    [Header("Refererence")]
    [SerializeField] private TextMeshPro text_interactButton;
    [SerializeField] private InteractOnButton use_interactButton;
    [SerializeField] private InteractOnButton exit_interactButton;
    [SerializeField] private Transform povTransform;
    [SerializeField] private CinemachineVirtualCamera povCam;

    private void Start()
    {
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

    private void ExitBalista()
    {
        if (IsInUse)
        {
            povCam.Priority = -int.MaxValue;
            ShowInteractText();
            ExitBalista_ServerRpc();
        }

    }

    private void UseBalista()
    {
        if (!IsInUse)
        {
            povCam.Priority = int.MaxValue;
            HideInteractText();
            UseBalista_ServerRpc();
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
    private void UseBalista_ServerRpc()
    {
        if (!IsInUse)
        {
            isInUse.Value = true;
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
