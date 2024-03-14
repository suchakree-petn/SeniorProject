using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    public BowWeaponHolder BowWeapon;


    public void UseWeapon(InputAction.CallbackContext context)
    {
        switch (playerController.PlayerCharacterData.PlayerRole)
        {
            case PlayerRole.FrontLine:
                break;
            case PlayerRole.DamageDealer:
                BowWeapon.UseWeapon();
                break;
            case PlayerRole.Supporter:
                break;
        }
    }

    [Header("Reference")]
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private PlayerController playerController;
}
