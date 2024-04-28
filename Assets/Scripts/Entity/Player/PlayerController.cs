using System;
using System.Collections;
using Cinemachine;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour, IDamageable
{
    [SerializeField] private PlayerCameraMode playerCameraMode;
    public PlayerCameraMode PlayerCameraMode => playerCameraMode;
    [SerializeField] private PlayerCharacterData _playerCharacterData;
    public PlayerCharacterData PlayerCharacterData => _playerCharacterData;
    public Vector3 OuterForce;

    public Action<PlayerCameraMode, PlayerCameraMode> OnPlayerCameraModeChanged;


    [Header("Reference")]
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected MouseMovement mouseMovement;
    [SerializeField] protected PlayerAnimation playerAnimation;
    [SerializeField] protected PlayerHealth playerHealth;
    public Collider HitboxCollider;


    protected virtual void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("spawn");
        InitPlayerCharacter();

        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.InitPlayerActions();
        playerInputManager.JumpAction.performed += playerMovement.PlayerJump;
        playerInputManager.RunAction.performed += playerMovement.PlayerRun;
        playerInputManager.MovementAction.canceled += playerMovement.PlayerStopRun;

        playerInputManager.Look.performed += mouseMovement.SetLook;
        playerInputManager.Look.canceled += mouseMovement.SetLook;
        // playerInputManager.SwitchViewMode.performed += SwitchViewMode;
        playerInputManager.SwitchViewMode.canceled += SwitchViewMode;


    }

    protected virtual void InitPlayerCharacter()
    {
        playerMovement.CanMove = true;
        playerMovement.SetCameraMode(playerCameraMode);

        mouseMovement.InitCameras(playerCameraMode);
        mouseMovement.LockMouseCursor();
        SetCameraMode(playerCameraMode, false);


    }

    public override void OnNetworkDespawn()
    {
        Debug.Log("Despawn");
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;

        playerInputManager.JumpAction.performed -= playerMovement.PlayerJump;
        playerInputManager.RunAction.performed -= playerMovement.PlayerRun;
        playerInputManager.MovementAction.canceled -= playerMovement.PlayerStopRun;

        playerInputManager.Look.performed -= mouseMovement.SetLook;
        playerInputManager.Look.canceled -= mouseMovement.SetLook;
        // playerInputManager.SwitchViewMode.performed -= SwitchViewMode;
        playerInputManager.SwitchViewMode.canceled -= SwitchViewMode;


    }
    protected virtual void Update()
    {

        // Test spawn enemy entity
        if (Input.GetKeyDown(KeyCode.F) && IsOwner)
        {
            EnemyManager.Instance.Spawn(2001, transform.position);
        }

    }

    protected virtual void FixedUpdate()
    {
        if (!IsOwner) return;

        playerMovement.ApplyGravity();
        playerMovement.MoveCharactor();
        mouseMovement.RotatePlayer();


    }

    public bool SetPlayerCharacterData(PlayerCharacterData playerCharacterData)
    {
        _playerCharacterData = playerCharacterData;
        return true;
    }

    protected virtual void MovementAnimation()
    {
        Vector3 finalVelocity = playerMovement.GetMovementForce();
        finalVelocity = new(finalVelocity.x, 0f, finalVelocity.z);
        finalVelocity = Vector3.ClampMagnitude(finalVelocity, playerMovement.PlayerMovementConfig.MoveSpeed);
        finalVelocity = transform.InverseTransformDirection(finalVelocity); // Convert to local space
        finalVelocity = playerMovement.GetMoveSpeedRatioOfMaxMoveSpeed(finalVelocity);
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

    protected virtual void LateUpdate()
    {
        MovementAnimation();
        mouseMovement.RotateCamera();

    }
    [Command]
    protected virtual void SetCameraMode(PlayerCameraMode newMode, bool isShowCrossHair = true)
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
    protected virtual void SwitchViewMode(InputAction.CallbackContext context)
    {
        switch (playerCameraMode)
        {
            case PlayerCameraMode.ThirdPerson:
                SetCameraMode(PlayerCameraMode.Focus, true);
                OnPlayerCameraModeChanged?.Invoke(PlayerCameraMode.ThirdPerson, PlayerCameraMode.Focus);
                break;
            case PlayerCameraMode.Focus:
                SetCameraMode(PlayerCameraMode.ThirdPerson, false);
                OnPlayerCameraModeChanged?.Invoke(PlayerCameraMode.Focus, PlayerCameraMode.ThirdPerson);
                break;
        }
        Debug.Log($"Switch to {playerCameraMode}");

    }

    public virtual float GetCurrentHp()
    {
        return playerHealth.CurrentHealth;
    }

    [ClientRpc]
    public void TakeDamage_ClientRpc(AttackDamage damage)
    {

        if (!IsOwner)
        {
            playerHealth.miniHpBar.maxValue = PlayerCharacterData.GetMaxHp();
            playerHealth.miniHpBar.value = playerHealth.CurrentHealth;
        }
        else
        {
            playerHealth.TakeDamage(damage, PlayerCharacterData.GetDefense());
        }
    }

    [ClientRpc]
    public void TakeHeal_ClientRpc(AttackDamage damage)
    {
        if (!IsOwner)
        {
            playerHealth.miniHpBar.maxValue = PlayerCharacterData.GetMaxHp();
            playerHealth.miniHpBar.value = playerHealth.CurrentHealth;
        }
        else
        {
            playerHealth.TakeHeal(damage);
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamage_ServerRpc(AttackDamage damage)
    {
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeHeal_ServerRpc(AttackDamage damage)
    {
    }

    public void SetCanPlayerMove(bool canMove)
    {
        playerMovement.CanMove = canMove;
    }

    protected virtual void OnEnable()
    {
        playerHealth.InitHp(PlayerCharacterData);
    }
    protected virtual void OnDisable()
    {
    }

}
