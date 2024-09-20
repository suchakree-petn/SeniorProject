using System;
using Cinemachine;
using Gamekit3D;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class BalistaController : NetworkBehaviour
{
    public Action OnFire_Local;

    [Header("Config")]
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;
    [SerializeField] float rotateSpeed = 10;
    [SerializeField] float maxCameraPitch = 60;
    [SerializeField] float minCameraPitch = 10;
    float cameraPitch;
    [SerializeField] float arrowSpeed = 10;
    [SerializeField] float fireCooldown = 1;
    float _fireCooldown;
    [SerializeField] float firePointOffset = 1;



    public PlayerController UsingPlayer;

    [FoldoutGroup("Refererence")]
    [SerializeField] private TextMeshProUGUI text_interactButton;


    [FoldoutGroup("Refererence")]
    [SerializeField] private InteractOnButton use_interactButton;

    [FoldoutGroup("Refererence")]
    [SerializeField] private InteractOnButton exit_interactButton;

    [FoldoutGroup("Refererence")]
    [SerializeField] private Transform povTransform;

    [FoldoutGroup("Refererence")]
    [SerializeField] private CinemachineVirtualCamera povCam;

    [FoldoutGroup("Refererence")]
    [SerializeField] private NetworkAnimator networkAnimator;

    [FoldoutGroup("Refererence")]
    [SerializeField] private GameObject balista;

    [FoldoutGroup("Refererence")]
    [SerializeField] private GameObject stackBalista;

    [FoldoutGroup("Refererence")]
    [SerializeField] private Transform arrow_prf;

    private void Start()
    {
        OnFire_Local += FireBalista;
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (UsingPlayer.OwnerClientId == NetworkManager.LocalClientId && IsInUse && _fireCooldown <= 0)
            {
                // Fire animation
                networkAnimator.SetTrigger("Shoot");

                _fireCooldown = fireCooldown;
            }
        }

        if (_fireCooldown > 0)
        {
            _fireCooldown -= Time.deltaTime;
        }
        else
        {
            _fireCooldown = 0;
        }
    }

    private void LateUpdate()
    {
        if (!IsInUse) return;

        Vector2 moveVector = PlayerInputManager.Instance.MovementAction.ReadValue<Vector2>();

        moveVector *= rotateSpeed;

        // float eulerAngleX = povTransform.eulerAngles.x + moveVector.y;
        cameraPitch -= moveVector.y;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxCameraPitch, minCameraPitch);
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

    private void FireBalista()
    {

        SpawnArrow(arrow_prf);

        FireBalista_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void FireBalista_ServerRpc()
    {
        FireBalista_ClientRpc();
    }

    [ClientRpc]
    private void FireBalista_ClientRpc()
    {
        if (UsingPlayer.OwnerClientId == NetworkManager.LocalClientId) return;

        SpawnArrow(arrow_prf);
    }

    private void SpawnArrow(Transform arrow_prf)
    {
        Transform camTransform = Camera.main.transform;

        Transform arrow = Instantiate(arrow_prf, camTransform.position + (camTransform.forward * firePointOffset), Quaternion.identity);
        arrow.forward = camTransform.forward;

        LaunchArrow(arrow);
    }

    private void LaunchArrow(Transform arrow)
    {
        Rigidbody rigidbody = arrow.GetComponent<Rigidbody>();
        rigidbody.AddForce(arrow.forward * arrowSpeed, ForceMode.Impulse);
    }

    public void AnimationEventHandler_OnFire()
    {
        if (UsingPlayer.OwnerClientId != NetworkManager.LocalClientId) return;

        OnFire_Local?.Invoke();
    }

    private void ExitBalista()
    {
        if (IsInUse)
        {
            balista.SetActive(true);
            stackBalista.SetActive(false);

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
            balista.SetActive(false);
            stackBalista.SetActive(true);

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
