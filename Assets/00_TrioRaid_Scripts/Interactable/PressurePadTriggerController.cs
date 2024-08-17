using Gamekit3D.GameCommands;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PressurePadTriggerController : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;
    [SerializeField] private LayerMask layers;
    [SerializeField] private float coolDown;

    public UnityEvent OnEnter_Local;
    public UnityEvent OnExit_Local;

    // [Header("Reference")]

    private float lastExitTime;

    void OnTriggerEnter(Collider other)
    {
        if(!IsServer) return;
        if (Time.time - lastExitTime < coolDown) return;

        if (IsInUse) return;

        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            if (IsServer)
            {
                OnEnterPressurePadHandler();
            }
            OnEnter_Local?.Invoke();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(!IsServer) return;
        if (!IsInUse) return;

        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            if (IsServer)
            {
                OnExitPressurePadHandler();
            }
            OnExit_Local?.Invoke();
            lastExitTime = Time.time;
        }
    }

    private void OnEnterPressurePadHandler()
    {
        UsePressurePad();
    }
    private void OnExitPressurePadHandler()
    {
        ExitPressurePad();
    }

    private void UsePressurePad()
    {
        if (!IsInUse)
        {
            UsePressurePad_ServerRpc();
            PressurePadPuzzleManager.Instance.AddActivatePadCount(1);
        }
    }

    private void ExitPressurePad()
    {
        if (IsInUse)
        {
            ExitPressurePad_ServerRpc();
            PressurePadPuzzleManager.Instance.RemoveActivatePadCount(1);
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

    [ServerRpc]
    private void UsePressurePad_ServerRpc()
    {
        if (!IsInUse)
        {
            isInUse.Value = true;
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
    }
}
