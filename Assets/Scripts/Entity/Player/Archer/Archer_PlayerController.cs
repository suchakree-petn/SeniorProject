using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Archer_PlayerController : PlayerController
{
    [Header("Archer Reference")]
    [SerializeField] protected Archer_PlayerWeapon archer_playerWeapon;


    protected override void Start()
    {
        if (!IsOwner) return;

        base.Start();
    }

    protected override void Update()
    {
        if (!IsOwner) return;
        base.Update();
        // ArcherAnimation();
    }
    protected override void LateUpdate()
    {
        if (!IsOwner) return;
        base.LateUpdate();
        ArcherAnimation();
    }

    public override void OnNetworkSpawn()
    {

        if (!IsOwner) return;
        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += archer_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled += archer_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {

        if (!IsOwner) return;
        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= archer_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled -= archer_playerWeapon.UseWeapon;

    }
    public override void InitWeapon()
    {
    }

    public void SetWeaponHolderPosition(PlayerCameraMode prev, PlayerCameraMode current)
    {
        if (current == PlayerCameraMode.Focus)
        {
            archer_playerWeapon.SwitchWeaponHolderTo(Archer_WeaponHolderState.InHand);
            archer_playerWeapon.WeaponHolderState = Archer_WeaponHolderState.InHand;
        }
        else
        {
            archer_playerWeapon.SwitchWeaponHolderTo(Archer_WeaponHolderState.OnBack);
            archer_playerWeapon.WeaponHolderState = Archer_WeaponHolderState.OnBack;
        }

    }

    private void ArcherAnimation()
    {
        if (PlayerCameraMode.Focus == PlayerCameraMode)
        {
            playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 1, Time.deltaTime * 10));
            WalkAnimationWhileFocus();

            if (archer_playerWeapon.IsDrawing)
            {
                playerAnimation.SetLayerWeight(2, 1);
                DrawingBowAnimation();
            }
            else
            {
                playerAnimation.SetLayerWeight(2, 0);
                playerAnimation.SetFloat("DrawPower", 0);
            }
        }
        else
        {
            playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 0, Time.deltaTime * 10));
        }
    }

    private void WalkAnimationWhileFocus()
    {
        Vector3 finalVelocity = playerMovement.GetMovementForce();
        finalVelocity = new(finalVelocity.x, 0f, finalVelocity.z);
        finalVelocity = Vector3.ClampMagnitude(finalVelocity, playerMovement.PlayerMovementConfig.MoveSpeed);
        finalVelocity = transform.InverseTransformDirection(finalVelocity); // Convert to local space
        finalVelocity = playerMovement.GetMoveSpeedRatioOfNormalMoveSpeed(finalVelocity);
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
    private void DrawingBowAnimation()
    {
        float drawPowerRatio = archer_playerWeapon.DrawPower / archer_playerWeapon.BowConfig.MaxDrawPower;
        playerAnimation.SetFloat("DrawPower", drawPowerRatio);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        if (!IsOwner) return;
        OnPlayerCameraModeChanged += SetWeaponHolderPosition;

    }

    protected override void OnDisable()
    {

        base.OnDisable();
        if (!IsOwner) return;
        OnPlayerCameraModeChanged -= SetWeaponHolderPosition;
    }
}
