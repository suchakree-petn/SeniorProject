using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public partial class Caster_PlayerController : PlayerController
{
    [Header("Caster Reference")]
    [SerializeField] protected Caster_PlayerWeapon caster_playerWeapon;
    [SerializeField] protected CasterAbility_BlessingShield casterAbility_BlessingShield;
    [SerializeField] protected CasterAbility_PowerUp casterAbility_PowerUp;


    protected override void Start()
    {

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
            playerHealth.CurrentHealth += Time.deltaTime * 20;
        }
        if (playerHealth.CurrentHealth > PlayerCharacterData.GetMaxHp())
        {
            playerHealth.CurrentHealth = PlayerCharacterData.GetMaxHp();
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
        else if (PlayerAnimation.GetLayerWeight(1) > 0)
        {
            PlayerAnimation.SetLayerWeight(1, Mathf.Lerp(PlayerAnimation.GetLayerWeight(1), 0, Time.deltaTime * 10));
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        base.OnNetworkSpawn();
        AbilityUIManager.Instance.OnSetAbilityIcon_E?.Invoke(casterAbility_BlessingShield.AbilityData.Icon);
        AbilityUIManager.Instance.OnSetAbilityIcon_Q?.Invoke(casterAbility_PowerUp.AbilityData.Icon);

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        base.OnNetworkDespawn();


    }




    private void HealOrbAnimation()
    {
        if (caster_playerWeapon.IsReadyToUse)
        {
            PlayerAnimation.SetTriggerNetworkAnimation("HealOrb");
            Debug.Log("Heal orb anim");
        }
        else
        {
            Debug.Log("Not ready Heal orb");

        }

    }

    private void WalkAnimationWhileFocus()
    {
        PlayerAnimation.SetLayerWeight(1, Mathf.Lerp(PlayerAnimation.GetLayerWeight(1), 1, Time.deltaTime * 10));

        Vector3 finalVelocity = playerMovement.GetMovementForce();
        finalVelocity = new(finalVelocity.x, 0f, finalVelocity.z);
        finalVelocity = Vector3.ClampMagnitude(finalVelocity, playerMovement.PlayerMovementConfig.MoveSpeed);
        finalVelocity = transform.InverseTransformDirection(finalVelocity); // Convert to local space
        finalVelocity = playerMovement.GetMoveSpeedRatioOfNormalMoveSpeed(finalVelocity);
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

    public Caster_PlayerWeapon GetCaster_PlayerWeapon()
    {
        return caster_playerWeapon;
    }

    protected override void OnEnable()
    {
        caster_playerWeapon.OnUseWeapon += HealOrbAnimation;

        base.OnEnable();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += caster_playerWeapon.UseWeapon;
    }

    protected override void OnDisable()
    {
        caster_playerWeapon.OnUseWeapon -= HealOrbAnimation;

        base.OnDisable();
        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed -= caster_playerWeapon.UseWeapon;
    }
}
