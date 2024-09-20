using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Map5_HornManager : NetworkSingleton<Map5_HornManager>
{
    public Action<ulong> OnHornActive_Server; // send hornId
    public Action<ulong> OnHornInActive_Server; // send hornId

    [SerializeField] bool isFirstTimeActivated = false;

    public Dictionary<ulong, HornController> ActiveHorns = new();


    private bool IsHasSomeHornActive
    {
        get
        {
            foreach (KeyValuePair<ulong, HornController> horn in ActiveHorns)
            {
                if (horn.Value.IsActive)
                {
                    return true;
                }
            }
            return false;
        }
    }

    protected override void InitAfterAwake()
    {
    }


    private void Start()
    {
        OnHornActive_Server += CheckFirstTimeActivated;

        OnHornInActive_Server += OnHornInActiveHandler;
    }

    private void OnHornActiveHandler(ulong hornId)
    {
        ProvokeDragon(hornId);
    }

    private void OnHornInActiveHandler(ulong hornId)
    {
        StopProvokeDragon();
    }

    private void ProvokeDragon(ulong hornId)
    {
        RedDragon_Fly_EnemyController redDragon_Fly_EnemyController = Map5_PuzzleManager.Instance.BossController;
        Map5_PuzzleManager map5_PuzzleManager = Map5_PuzzleManager.Instance;


        Transform closestHornTransform = ActiveHorns[hornId].transform;

        foreach (KeyValuePair<ulong, HornController> horn in ActiveHorns)
        {
            HornController hornController = horn.Value;
            if (!hornController.IsActive) continue;

            float closestDistance = float.MaxValue;

            float distance = Vector3.Distance(hornController.transform.position, redDragon_Fly_EnemyController.transform.position);

            if (distance < closestDistance)
            {

                closestDistance = distance;
                closestHornTransform = hornController.transform;
            }
        }
        map5_PuzzleManager.BossController.MovingTo(closestHornTransform);

    }

    private void StopProvokeDragon()
    {
        if (!IsHasSomeHornActive)
        {
            Map5_PuzzleManager map5_PuzzleManager = Map5_PuzzleManager.Instance;

            map5_PuzzleManager.BossController.MovingTo(map5_PuzzleManager.Broken_BalistaController.transform);
        }
    }

    private void CheckFirstTimeActivated(ulong hornId)
    {
        Debug.Log("Check first time trigger");
        if (!isFirstTimeActivated)
        {
            OnHornActive_Server -= CheckFirstTimeActivated;

            isFirstTimeActivated = true;

            if (IsServer)
            {
                Map5_PuzzleManager.Instance.SetState_ServerRpc(Map5_GameState.Phase2_RepairBalista);
                OnHornActive_Server += OnHornActiveHandler;
                Map5_PuzzleManager.Instance.BossController.OnActiveBoss += () => ProvokeDragon(hornId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UseHorn_ServerRpc(ulong hornId)
    {
        HornController hornController = ActiveHorns[hornId];


        if (!hornController.IsActive)
        {
            ActiveHorns[hornId].IsActive = true;
            OnHornActive_Server?.Invoke(hornId);
        }
        else
        {
            Debug.Log("Already active");
        }


    }

    [ServerRpc(RequireOwnership = false)]
    public void StopUseHorn_ServerRpc(ulong hornId)
    {
        HornController hornController = ActiveHorns[hornId];


        if (hornController.IsActive)
        {
            ActiveHorns[hornId].IsActive = false;
            OnHornInActive_Server?.Invoke(hornId);
        }
        else
        {
            Debug.Log("Already inactive");
        }


    }
}

