using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Gamekit3D.GameCommands;
using UnityEngine;

public class TestDoor : MonoBehaviour, IInteractReceiveable
{
    private float startY;
    public void ReceiveInteraction(GameCommandType gameCommandType)
    {
        switch (gameCommandType)
        {
            case GameCommandType.Open:
                transform.DOKill();
                transform.DOMoveY(startY + 10, 4);
                break;
            case GameCommandType.Close:
                transform.DOKill();
                transform.DOMoveY(startY, 4);
                break;
        }
    }


}
