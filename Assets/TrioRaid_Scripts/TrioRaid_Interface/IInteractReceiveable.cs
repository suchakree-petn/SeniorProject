using Gamekit3D.GameCommands;
using Unity.Netcode;

public interface IInteractReceiveable
{
    public void ReceiveInteraction(GameCommandType gameCommandType);
}