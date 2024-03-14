using System.Collections;
using Cinemachine;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private PlayerCameraMode playerCameraMode;
    [SerializeField] private PlayerCharacterData _playerCharacterData;
    public PlayerCharacterData PlayerCharacterData => _playerCharacterData;
    public Vector3 OuterForce;


    [Header("Reference")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseMovement mouseMovement;
    [SerializeField] private PlayerAnimation playerAnimation;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerWeapon playerWeapon;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        Debug.Log("spawn");
        InitPlayerCharacter();

        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.InitPlayerActions();
        playerInputManager.JumpAction.performed += playerMovement.PlayerJump;
        playerInputManager.RunAction.performed += playerMovement.PlayerRun;
        playerInputManager.MovementAction.canceled += playerMovement.PlayerStopRun;

        playerInputManager.Look.performed += mouseMovement.SetLook;
        playerInputManager.Look.canceled += mouseMovement.SetLook;
        playerInputManager.SwitchViewMode.performed += SwitchViewMode;
        playerInputManager.SwitchViewMode.canceled += SwitchViewMode;

        playerInputManager.Attack.performed += playerWeapon.UseWeapon;

    }

    private void InitPlayerCharacter()
    {
        playerMovement.CanMove = true;
        playerMovement.SetCameraMode(playerCameraMode);

        mouseMovement.InitCameras(playerCameraMode);
        mouseMovement.LockMouseCursor();
        SetCameraMode(playerCameraMode, true);

        playerHealth.InitHp(PlayerCharacterData);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        Debug.Log("Despawn");
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;

        playerInputManager.JumpAction.performed -= playerMovement.PlayerJump;
        playerInputManager.RunAction.performed -= playerMovement.PlayerRun;
        playerInputManager.MovementAction.canceled -= playerMovement.PlayerStopRun;

        playerInputManager.Look.performed -= mouseMovement.SetLook;
        playerInputManager.Look.canceled -= mouseMovement.SetLook;
        playerInputManager.SwitchViewMode.performed -= SwitchViewMode;
        playerInputManager.SwitchViewMode.canceled -= SwitchViewMode;

        playerInputManager.Attack.performed -= playerWeapon.UseWeapon;

    }
    private void Update()
    {

        // Test spawn enemy entity
        if (Input.GetKeyDown(KeyCode.F) && IsOwner)
        {
            EnemyManager.Instance.Spawn(2001, transform.position);
        }

        if (IsOwner)
        {
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        playerMovement.MoveCharactor();
        mouseMovement.RotatePlayer();


    }


    private void MovementAnimation()
    {
        Vector3 finalVelocity = playerMovement.GetMovementForce();
        finalVelocity = new(finalVelocity.x, 0f, finalVelocity.z);
        finalVelocity = Vector3.ClampMagnitude(finalVelocity, playerMovement.PlayerMovementConfig.MoveSpeed);
        finalVelocity = transform.InverseTransformDirection(finalVelocity); // Convert to local space
        if (PlayerInputManager.Instance.MovementAction.IsPressed())
        {
            playerAnimation.SetMoveVelocityX(finalVelocity.x);
            playerAnimation.SetMoveVelocityZ(finalVelocity.z);
        }
        else
        {
            finalVelocity -= playerMovement.PlayerMovementConfig.groundDrag * Time.fixedDeltaTime * finalVelocity;
            playerAnimation.SetMoveVelocityX(finalVelocity.x);
            playerAnimation.SetMoveVelocityZ(finalVelocity.z);
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner) return;
        MovementAnimation();
        mouseMovement.RotateCamera();

    }
    [Command]
    public void SetCameraMode(PlayerCameraMode newMode, bool isShowCrossHair = true)
    {
        playerCameraMode = newMode;
        switch (newMode)
        {
            case PlayerCameraMode.ThirdPerson:
                mouseMovement.SetThirdperson(isShowCrossHair);
                StartCoroutine(WaitSetCameraMode());

                break;
            case PlayerCameraMode.Focus:
                mouseMovement.SetFocus(isShowCrossHair);
                playerMovement.SetCameraMode(playerCameraMode);
                mouseMovement.SetCameraMode(playerCameraMode);
                break;
        }
    }
    IEnumerator WaitSetCameraMode()
    {
        yield return new WaitForSeconds(Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time);
        playerMovement.SetCameraMode(playerCameraMode);
        mouseMovement.SetCameraMode(playerCameraMode);
    }
    private void SwitchViewMode(InputAction.CallbackContext context)
    {
        switch (playerCameraMode)
        {
            case PlayerCameraMode.ThirdPerson:
                SetCameraMode(PlayerCameraMode.Focus, true);
                break;
            case PlayerCameraMode.Focus:
                SetCameraMode(PlayerCameraMode.ThirdPerson, false);
                break;
        }
    }

    public float GetCurrentHp()
    {
        return playerHealth.CurrentHealth;
    }
}
