using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private PlayerCameraMode playerCameraMode;

    [Header("Reference")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private MouseMovement mouseMovement;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        playerMovement.CanMove = true;
        playerMovement.InitPlayerActions();
        playerMovement.SetCameraMode(playerCameraMode);
        playerMovement.jumpAction.performed += playerMovement.PlayerJump;
        playerMovement.runAction.performed += playerMovement.PlayerRun;
        playerMovement.movementAction.canceled += playerMovement.PlayerStopRun;

        mouseMovement.InitPlayerActions();
        mouseMovement.playerActions.PlayerCharacter.Look.performed += mouseMovement.SetLook;
        mouseMovement.playerActions.PlayerCharacter.Look.canceled += mouseMovement.SetLook;
        mouseMovement.InitCameras(playerCameraMode);
        mouseMovement.LockMouseCursor();
        SetCameraMode(playerCameraMode, true);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        playerMovement.jumpAction.performed -= playerMovement.PlayerJump;
        playerMovement.runAction.performed -= playerMovement.PlayerRun;
        playerMovement.movementAction.canceled -= playerMovement.PlayerStopRun;

        mouseMovement.playerActions.PlayerCharacter.Look.performed -= mouseMovement.SetLook;
        mouseMovement.playerActions.PlayerCharacter.Look.canceled -= mouseMovement.SetLook;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        playerMovement.MoveCharactor();
        mouseMovement.RotateCamera();

    }
    [Command]
    public void SetCameraMode(PlayerCameraMode newMode, bool isShowCrossHair = true)
    {
        switch (newMode)
        {
            case PlayerCameraMode.ThirdPerson:
                mouseMovement.SetThirdperson(isShowCrossHair);
                break;
            case PlayerCameraMode.Focus:
                mouseMovement.SetFocus(isShowCrossHair);

                break;
        }
        playerMovement.SetCameraMode(newMode);
    }

}
