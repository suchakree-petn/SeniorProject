using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_PlayerController : PlayerController
{
    [Header("Archer Reference")]
    [SerializeField] private Transform firePointTransform;
    [SerializeField] protected Archer_PlayerWeapon archer_playerWeapon;


    protected override void Start()
    {
        base.Start();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerInputManager playerInputManager = PlayerInputManager.Instance;
        playerInputManager.Attack.performed += archer_playerWeapon.UseWeapon;
        playerInputManager.Attack.canceled += archer_playerWeapon.UseWeapon;

    }

    public override void OnNetworkDespawn()
    {
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
