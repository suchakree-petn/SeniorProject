using UnityEngine.InputSystem;

public class PlayerInputManager : Singleton<PlayerInputManager>
{
    public PlayerActions PlayerActions;
    public InputAction MovementAction;
    public InputAction JumpAction;
    public InputAction RunAction;
    public InputAction Look;
    public InputAction SwitchViewMode;

    protected override void InitAfterAwake()
    {
    }
    public void InitPlayerActions()
    {
        PlayerActions = new();
        PlayerActions.PlayerCharacter.Enable();

        MovementAction = PlayerActions.PlayerCharacter.Movement;
        JumpAction = PlayerActions.PlayerCharacter.Jump;
        RunAction = PlayerActions.PlayerCharacter.Run;
        Look = PlayerActions.PlayerCharacter.Look;
        SwitchViewMode = PlayerActions.PlayerCharacter.SwitchViewMode;

    }

}
