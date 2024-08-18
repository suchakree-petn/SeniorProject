using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

public class GateController : NetworkBehaviour
{
    private NetworkVariable<bool> isOpen = new(false);

    [ShowInInspector]
    public bool IsOpen => isOpen.Value;

    [Header("Setting")]
    [SerializeField] private float openPositon_Y;
    [SerializeField] private float openDuration = 3;
    [SerializeField] private float closePositon_Y;
    [SerializeField] private float closeDuration = 1.5f;


    [Header("Reference")]
    [SerializeField] Transform gateTransform;

    [HideIf("IsOpen")]
    [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
    public void OpenGate()
    {
        Debug.Log("Open Gate");
        OpenGate_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OpenGate_ServerRpc()
    {
        isOpen.Value = true;
        OpenGate_ClientRpc();
    }

    [ClientRpc]
    private void OpenGate_ClientRpc()
    {
        gateTransform.DOKill();
        gateTransform.DOLocalMoveY(openPositon_Y, openDuration).SetEase(Ease.Linear);
    }

    [ShowIf("IsOpen")]
    [Button(ButtonSizes.Large), GUIColor(1, 0.2f, 0)]
    public void CloseGate()
    {
        Debug.Log("Close Gate");
        CloseGate_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CloseGate_ServerRpc()
    {
        isOpen.Value = false;
        CloseGate_ClientRpc();
    }

    [ClientRpc]
    private void CloseGate_ClientRpc()
    {
        gateTransform.DOKill();
        gateTransform.DOLocalMoveY(closePositon_Y, closeDuration).SetEase(Ease.OutBounce);
    }

    // public void ReceiveInteraction(GameCommandType gameCommandType)
    // {
    //     switch (gameCommandType)
    //     {
    //         case GameCommandType.Activate:
    //             PressurePadPuzzleManager.Instance.AddActivatePadCount(1);
    //             break;
    //         case GameCommandType.Deactivate:
    //             PressurePadPuzzleManager.Instance.RemoveActivatePadCount(1);
    //             break;
    //     }
    // }
}
