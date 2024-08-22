using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private void OnDestroy()
    {
        PlayerManager.Instance.LocalPlayerController.GetMouseMovement().UnLockMouseCursor();

        PlayerUIManager.Instance.ShowResultUI();
    }
}
