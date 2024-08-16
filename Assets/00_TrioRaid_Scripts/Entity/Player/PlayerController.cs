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
    public Action OnPlayerDie;
    public Action OnPlayerTakeDamage;
    public bool IsDead;
    public bool IsGrounded => playerMovement.IsGrouded;

    [Header("Reference")]
    [SerializeField] protected PlayerMovement playerMovement;
    [SerializeField] protected MouseMovement mouseMovement;
    [SerializeField] protected PlayerWeapon playerWeapon;
    [SerializeField] protected PlayerAbility playerAbilityE;
    [SerializeField] protected PlayerAbility playerAbilityQ;
    public PlayerAnimation PlayerAnimation;
    [SerializeField] protected PlayerHealth playerHealth;
    [SerializeField] private Renderer meshRenderer_character;
    [SerializeField] private Renderer meshRenderer_weapon;

    protected virtual void Start()
    {
        if (IsLocalPlayer)
        {
            meshRenderer_character.gameObject.layer = 0;

            if (meshRenderer_weapon != null)
            {
                meshRenderer_weapon.gameObject.layer = 0;
            }

            Camera.main.GetComponent<AudioListener>().enabled = false;
            gameObject.AddComponent(typeof(AudioListener));
        }

        OnPlayerTakeDamage += PlayerUIManager.Instance.FullScreen_Player_Hit;
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
        if (Input.GetKeyDown(KeyCode.T) && IsOwner)
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
    public MouseMovement GetMouseMovement()
    {
        return mouseMovement;
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
            PlayerAnimation.SetMoveVelocityX(finalVelocity.x);
            PlayerAnimation.SetMoveVelocityZ(finalVelocity.z);
        }
        else
        {
            finalVelocity -= playerMovement.PlayerMovementConfig.groundDrag * Time.fixedDeltaTime * finalVelocity;
            PlayerAnimation.SetMoveVelocityX(finalVelocity.x);
            PlayerAnimation.SetMoveVelocityZ(finalVelocity.z);
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
    public virtual void SwitchViewMode(InputAction.CallbackContext context)
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

    public void SwitchViewMode(PlayerCameraMode playerCameraMode)
    {
        if (this.playerCameraMode == playerCameraMode) return;

        switch (playerCameraMode)
        {
            case PlayerCameraMode.ThirdPerson:
                SetCameraMode(PlayerCameraMode.ThirdPerson, false);
                OnPlayerCameraModeChanged?.Invoke(PlayerCameraMode.Focus, PlayerCameraMode.ThirdPerson);
                break;
            case PlayerCameraMode.Focus:
                SetCameraMode(PlayerCameraMode.Focus, true);
                OnPlayerCameraModeChanged?.Invoke(PlayerCameraMode.ThirdPerson, PlayerCameraMode.Focus);
                break;
        }
        Debug.Log($"Switch to {playerCameraMode}");
    }

    public virtual float GetCurrentHp()
    {
        return playerHealth.CurrentHealth;
    }
    
    public void TakeDamage(AttackDamage damage)
    {
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
            OnPlayerTakeDamage?.Invoke();
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

    [ClientRpc]
    public void SetCanPlayerMove_ClientRpc(bool canMove, ulong clientId)
    {
        if (!IsLocalPlayer) return;
        SetCanPlayerMove(canMove);
    }

    public void SetIsReadyToAttack(bool isReady)
    {
        playerWeapon.IsReadyToUse = isReady;
    }

    [ClientRpc]
    public void SetIsReadyToAttack_ClientRpc(bool isReady, ulong clientId)
    {
        if (NetworkManager.LocalClientId != clientId) return;
        SetIsReadyToAttack(isReady);
    }

    public void SetCanUseAbilityE(bool canUseAbilityE)
    {
        playerAbilityE.CanUse = canUseAbilityE;
    }

    [ClientRpc]
    public void SetCanUseAbilityE_ClientRpc(bool canUseAbilityE, ulong clientId)
    {
        if (NetworkManager.LocalClientId != clientId) return;

        SetCanUseAbilityE(canUseAbilityE);
    }

    public void SetCanUseAbilityQ(bool canUseAbilityQ)
    {
        playerAbilityQ.CanUse = canUseAbilityQ;
    }

    [ClientRpc]
    public void SetCanUseAbilityQ_ClientRpc(bool canUseAbilityQ, ulong clientId)
    {
        if (NetworkManager.LocalClientId != clientId) return;

        SetCanUseAbilityE(canUseAbilityQ);
    }

    public void SetPlayerVisible(bool visible)
    {
        meshRenderer_character.enabled = visible;
    }

    public void PlayerController_OnPlayerDie()
    {
        IsDead = true;
        // Animation
        PlayerAnimation.SetBool("IsDying", IsDead);
        PlayerUIManager.Instance.ShowRespawnCountdown();
        Invoke(nameof(WaitForRespawn), 10);
    }
    private void WaitForRespawn()
    {
        IsDead = false;
        PlayerAnimation.SetBool("IsDying", IsDead);
        PlayerUIManager.Instance.HideRespawnCountdown();
        AttackDamage healAmount = new()
        {
            Damage = 100000
        };
        TakeHeal_ClientRpc(healAmount);
    }
    protected virtual void OnEnable()
    {
        playerHealth.InitHp(PlayerCharacterData);

        OnPlayerDie += PlayerController_OnPlayerDie;
    }
    protected virtual void OnDisable()
    {
        OnPlayerDie -= PlayerController_OnPlayerDie;
    }


}
