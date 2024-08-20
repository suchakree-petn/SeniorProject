using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Map2_PuzzleTrigger : MonoBehaviour
{
    Action OnPlayerPlayerStayedInTriggerChanged_Local;
    [SerializeField] private int currentPlayerStayedInTrigger = 0;
    [SerializeField] private int requirePlayerCount = 1;

    [FoldoutGroup("Reference")]
    [SerializeField] private Collider triggerCol;

    private void Awake()
    {
        if (!triggerCol)
        {
            triggerCol = GetComponent<Collider>();
        }
    }

    private void Start()
    {
        OnPlayerPlayerStayedInTriggerChanged_Local += CheckCondition;
    }

    private void CheckCondition()
    {
        if (currentPlayerStayedInTrigger == requirePlayerCount)
        {
            triggerCol.enabled = false;
            Map2_PuzzleManager.Instance.Puzzle1_LockPlayerInArea_ServerRpc();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        if (!other.isTrigger) return;
        currentPlayerStayedInTrigger++;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        if (!other.isTrigger) return;
        currentPlayerStayedInTrigger--;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();
    }
}