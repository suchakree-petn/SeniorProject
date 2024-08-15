using System.Collections;
using System.Collections.Generic;
using Gamekit3D;
using Unity.Netcode;
using UnityEngine;

public class PressurePadTriggerController : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;

    [Header("Reference")]
    [SerializeField] private InteractOnTrigger interactOnTrigger;

    private void OnEnterPressurePadHandler()
    {
        UsePressurePad();
        ExitPressurePad();
    }

    private void UsePressurePad()
    {
        if (!IsInUse)
        {
            UsePressurePad_ServerRpc();
        }
    }

    private void ExitPressurePad()
    {
        if (IsInUse)
        {
            ExitPressurePad_ServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ExitPressurePad_ServerRpc()
    {
        if (IsInUse)
        {
            isInUse.Value = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UsePressurePad_ServerRpc()
    {
        if (!IsInUse)
        {
            isInUse.Value = true;
        }
    }

    private void OnEnable()
    {
        interactOnTrigger.OnEnter.AddListener(OnEnterPressurePadHandler);
    }

    private void OnDisable()
    {
        interactOnTrigger.OnEnter.RemoveListener(OnEnterPressurePadHandler);
    }
}
