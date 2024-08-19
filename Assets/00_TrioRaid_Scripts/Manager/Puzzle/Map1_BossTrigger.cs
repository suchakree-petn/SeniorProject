using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class Map1_BossTrigger : NetworkBehaviour
{
    Action OnPlayerPlayerStayedInTriggerChanged_Local;
    [SerializeField] private int currentPlayerStayedInTrigger = 0;
    [SerializeField] private int requirePlayerCount = 1;

    [FoldoutGroup("Reference")]
    [SerializeField] private Collider triggerCol;
    [SerializeField] private Transform bossAreaTransform;

    private void Start()
    {
        OnPlayerPlayerStayedInTriggerChanged_Local += CheckCondition;
    }

    private void CheckCondition()
    {
        if (currentPlayerStayedInTrigger == requirePlayerCount)
        {
            triggerCol.enabled = false;
            if (NetworkManager.Singleton.IsServer)
            {
                GroupWarp_ClientRpc();
            }
        }
    }

    private void TransitionToBossArea()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(PlayerUIManager.Instance.FadeScreenOut());
        sequence.Append(PlayerManager.Instance.LocalPlayerController.transform.DOMove(bossAreaTransform.position, 0));
        sequence.AppendInterval(1);
        sequence.Append(PlayerUIManager.Instance.FadeScreenIn());

        sequence.OnComplete(() =>
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Map1_PuzzleManager.Instance.EncounterBoss_Server();
            }
        });

        sequence.Play();
    }

    [ClientRpc]
    private void GroupWarp_ClientRpc()
    {
        TransitionToBossArea();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        if(!other.isTrigger) return;
        currentPlayerStayedInTrigger++;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;
        if(!other.isTrigger) return;
        currentPlayerStayedInTrigger--;
        OnPlayerPlayerStayedInTriggerChanged_Local?.Invoke();
    }
}
