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
        if (!IsOwner) return;

        base.Start();

    }

    protected override void Update()
    {
        playerHealth.miniHpBar.maxValue = PlayerCharacterData.GetMaxHp();
        playerHealth.miniHpBar.value = playerHealth.CurrentHealth;
        if (!IsOwner) return;
        base.Update();

        if (playerHealth.CurrentHealth < PlayerCharacterData.GetMaxHp())
        {
            playerHealth.currentHealth.Value += Time.deltaTime * 20;
        }
        if (playerHealth.CurrentHealth > PlayerCharacterData.GetMaxHp())
        {
            playerHealth.currentHealth.Value = PlayerCharacterData.GetMaxHp();
        }

    }
    protected override void LateUpdate()
    {
        if (!IsOwner) return;
        base.LateUpdate();
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
        if (!IsOwner) return;

        base.OnNetworkSpawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += caster_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        base.OnNetworkDespawn();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= caster_playerWeapon.UseWeapon;

    }
    public override void InitWeapon()
    {
    }



    private void HealOrbAnimation()
    {
        if (caster_playerWeapon.IsReadyToUse)
        {
            playerAnimation.SetTriggerNetworkAnimation("HealOrb");
            Debug.Log("Heal orb anim");
        }
        else
        {
            Debug.Log("Not ready Heal orb");

        }

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
        caster_playerWeapon.OnUseWeapon += HealOrbAnimation;

        if (!IsOwner) return;
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        caster_playerWeapon.OnUseWeapon -= HealOrbAnimation;

        if (!IsOwner) return;
        base.OnDisable();
    }
}
