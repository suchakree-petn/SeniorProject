using System.Collections.Generic;
using UnityEngine;

public class Map5_HornManager : NetworkSingleton<Map5_HornManager>
{
    private static bool isFirstTimeActivated = false;
    public List<HornController> CurrentActiveHorns = new();

    [SerializeField] private List<HornController> horns = new();


    protected override void InitAfterAwake()
    {
    }


    private void Start()
    {
        foreach (var horn in horns)
        {
            horn.isActive.OnValueChanged += CheckFirstTimeActivated;
            horn.isActive.OnValueChanged += OnUsingHornValueChanged;
        }
    }

    private void OnUsingHornValueChanged(bool notActive, bool isActive)
    {
        RedDragon_Fly_EnemyController redDragon_Fly_EnemyController = Map5_PuzzleManager.Instance.BossController;
        Map5_PuzzleManager map5_PuzzleManager = Map5_PuzzleManager.Instance;

        if (isActive)
        {
            float closestDistance = float.MaxValue;
            int hornIndex = 0;

            foreach (var horn in CurrentActiveHorns)
            {
                float distance = Vector3.Distance(horn.transform.position, redDragon_Fly_EnemyController.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    hornIndex = CurrentActiveHorns.IndexOf(horn);
                }
            }
            map5_PuzzleManager.BossController.Target = CurrentActiveHorns[hornIndex].transform;
        }
        else
        {
            map5_PuzzleManager.BossController.Target = map5_PuzzleManager.Broken_BalistaController.transform;
        }
    }

    private void CheckFirstTimeActivated(bool prevValue, bool newValue)
    {
        if (isFirstTimeActivated)
        {
            foreach (var horn in horns)
            {
                horn.isActive.OnValueChanged -= CheckFirstTimeActivated;
            }

            isFirstTimeActivated = false;

            if (IsServer)
            {
                Map5_PuzzleManager.Instance.SetState_ServerRpc(Map5_GameState.Phase2_RepairBalista);
            }
        }
    }
}

