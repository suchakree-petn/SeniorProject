using Gamekit3D.GameCommands;
using UnityEngine;

public interface IInteractReceiveable
{
    // public void ReceiveInteraction(GameCommandType gameCommandType)
    // {
    //     switch (gameCommandType)
    //     {
    //         case GameCommandType.Open:
    //             break;
    //         case GameCommandType.Close:
    //             break;
    //         case GameCommandType.Start:
    //             break;
    //         case GameCommandType.Stop:
    //             break;
    //         default:
    //             Debug.LogWarning("GameCommandType mismatch");
    //             break;
    //     }
    // }
    public void ReceiveInteraction(GameCommandType gameCommandType);

}