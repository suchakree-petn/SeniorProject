using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private void OnDestroy()
    {
        if (PlayerManager.Instance)
            PlayerManager.Instance.LocalPlayerController.GetMouseMovement().UnLockMouseCursor();

        if (PlayerUIManager.Instance)
            PlayerUIManager.Instance.ShowResultUI();
    }
}
