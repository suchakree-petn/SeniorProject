using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Caster_PlayerController : PlayerController
{
    [Header("Caster Reference")]
    [SerializeField] protected Caster_PlayerWeapon caster_playerWeapon;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!IsOwner) return;
        base.Update();
        if (PlayerCameraMode == PlayerCameraMode.Focus)
        {
            WalkAnimationWhileFocus();
        }
        else if (playerAnimation.GetLayerWeight(1) > 0)
        {
            playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 0, Time.deltaTime * 10));
        }
    }


    public override void OnNetworkSpawn()
    {
        // if (!IsOwner) return;

        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += caster_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {
        // if (!IsOwner) return;

        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= caster_playerWeapon.UseWeapon;

    }
    public override void InitWeapon()
    {
    }



    private void HealOrbAnimation()
    {
        playerAnimation.SetTriggerNetworkAnimation("HealOrb");
        Debug.Log("Heal orb anim");
        // if (caster_playerWeapon.WeaponHolderState == Archer_WeaponHolderState.InHand)
        // {
        //     playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 1, Time.deltaTime * 10));
        //     WalkAnimationWhileFocus();

        //     if (caster_playerWeapon.IsDrawing)
        //     {
        //         playerAnimation.SetLayerWeight(2, 1);
        //         DrawingBowAnimation();
        //     }
        //     else
        //     {
        //         playerAnimation.SetLayerWeight(2, 0);
        //         playerAnimation.SetFloat("DrawPower", 0);
        //     }
        // }
        // else
        // {
        //     playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 0, Time.deltaTime * 10));
        // }

    }

    private void WalkAnimationWhileFocus()
    {
        playerAnimation.SetLayerWeight(1, Mathf.Lerp(playerAnimation.GetLayerWeight(1), 1, Time.deltaTime * 10));

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
        caster_playerWeapon.OnUseWeapon += HealOrbAnimation;

    }

    protected override void OnDisable()
    {

        base.OnDisable();
        caster_playerWeapon.OnUseWeapon -= HealOrbAnimation;
    }
}
