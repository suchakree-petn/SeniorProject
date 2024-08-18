using System;
using Unity.Netcode;
using UnityEngine;

public class Map1_BattleTrigger : MonoBehaviour
{
    Action OnPlayerPlayerStayedInTriggerChanged_Local;
    [SerializeField] private int currentPlayerStayedInTrigger = 0;

    private void Start()
    {
        OnPlayerPlayerStayedInTriggerChanged_Local += CheckCondition;
    }

    private void CheckCondition()
    {
        if (currentPlayerStayedInTrigger == 3)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Map1_PuzzleManager.Instance.SpawnMobs_ServerRpc();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        currentPlayerStayedInTrigger++;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        currentPlayerStayedInTrigger--;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();
    }
}
