using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    private void OnDestroy()
    {
        List<MouseMovement> mouseMovements = new List<MouseMovement>();
        if (PlayerManager.Instance.PlayerGameObjectsByRole[PlayerRole.Tank].TryGetComponent(out MouseMovement tank_mouseMovement))
        {
            mouseMovements.Add(tank_mouseMovement);
        }

        if (PlayerManager.Instance.PlayerGameObjectsByRole[PlayerRole.Archer].TryGetComponent(out MouseMovement archer_mouseMovement))
        {
            mouseMovements.Add(archer_mouseMovement);
        }
        if (PlayerManager.Instance.PlayerGameObjectsByRole[PlayerRole.Caster].TryGetComponent(out MouseMovement caster_mouseMovement))
        {
            mouseMovements.Add(caster_mouseMovement);
        }
        foreach (var movement in mouseMovements)
        {
            movement.UnLockMouseCursor();
        }
        PlayerUIManager.Instance.ShowResultUI();
    }
}
