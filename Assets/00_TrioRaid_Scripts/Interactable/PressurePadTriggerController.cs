using Gamekit3D.GameCommands;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PressurePadTriggerController : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<bool> isInUse = new(false);
    public bool IsInUse => isInUse.Value;
    [SerializeField] private LayerMask layers;
    [SerializeField] private float coolDown;

    [FoldoutGroup("Event")]
    public UnityEvent OnEnter_Local;
    [FoldoutGroup("Event")]
    public UnityEvent OnExit_Local;

    // [Header("Reference")]

    private float lastExitTime;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (IsInUse) return;

        OnEnter_Local?.Invoke();

        if (!IsServer) return;

        if (Time.time - lastExitTime < coolDown) return;


        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            OnEnterPressurePadHandler();
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!IsInUse) return;

        OnExit_Local?.Invoke();

        if (!IsServer) return;

        if (0 != (layers.value & 1 << other.gameObject.layer))
        {
            OnExitPressurePadHandler();
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
            Map1_PuzzleManager.Instance.AddActivatePadCount(1);
        }
    }

    private void ExitPressurePad()
    {
        if (IsInUse)
        {
            ExitPressurePad_ServerRpc();
            Map1_PuzzleManager.Instance.RemoveActivatePadCount(1);
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
