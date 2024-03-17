using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Tank_PlayerController : PlayerController
{
    [Header("Tank Reference")]
    [SerializeField] protected Tank_PlayerWeapon tank_playerWeapon;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!IsOwner) return;
        base.Update();

        TankAnimation();
    }


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += tank_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled += tank_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= tank_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled -= tank_playerWeapon.UseWeapon;

    }
    public override void InitWeapon()
    {
    }

    public void SetWeaponHolderPosition(PlayerCameraMode prev, PlayerCameraMode current)
    {
        if (current == PlayerCameraMode.Focus)
        {
            // tank_playerWeapon.SwitchWeaponHolderTo(Tank_WeaponHolderState.InHand);
            tank_playerWeapon.WeaponHolderState = Tank_WeaponHolderState.InHand;
        }
        else
        {
            // tank_playerWeapon.SwitchWeaponHolderTo(Tank_WeaponHolderState.OnBack);
            tank_playerWeapon.WeaponHolderState = Tank_WeaponHolderState.OnBack;
        }

    }

    private void TankAnimation()
    {
        
        if (tank_playerWeapon.WeaponHolderState == Tank_WeaponHolderState.InHand)
        {
            WalkAnimationWhileFocus();

            if (tank_playerWeapon.IsSlash)
            {
                playerAnimation.SetBool("IsHSlash",true);
            }
            else
            {
                playerAnimation.SetBool("IsHSlash",false);
            }
        }
        else
        {
            if (tank_playerWeapon.IsSlash)
            {
                playerAnimation.SetBool("IsLSlash",true);
            }
            else
            {
                playerAnimation.SetBool("IsLSlash",false);
            }
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

    protected override void OnEnable()
    {
        base.OnEnable();
        OnPlayerCameraModeChanged += SetWeaponHolderPosition;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnPlayerCameraModeChanged -= SetWeaponHolderPosition;
    }
}
