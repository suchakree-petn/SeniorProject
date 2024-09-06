using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[SelectionBase]
public class HornController : NetworkBehaviour
{
    public NetworkVariable<bool> isActive = new(false);
    public bool IsActive => isActive.Value;


    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject worldSpacecanvas;

    [FoldoutGroup("Reference")]
    [SerializeField] private GameObject overlayCanvas;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        worldSpacecanvas.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        worldSpacecanvas.SetActive(false);
        StopUsing_ServerRpc();

    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.transform.root.TryGetComponent(out PlayerController _)) return;

        if (Input.GetKeyDown(KeyCode.F) && !IsActive)
        {
            StartUsing();
        }

        if (Input.GetKeyUp(KeyCode.F) && IsActive)
        {
            StopUsing();
        }
    }

    public void StartUsing()
    {
        overlayCanvas.SetActive(true);
        StartUsing_ServerRpc();

    }

    public void StopUsing()
    {
        overlayCanvas.SetActive(false);
        StopUsing_ServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartUsing_ServerRpc()
    {
        isActive.Value = true;
        StartUsing_ClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopUsing_ServerRpc()
    {
        isActive.Value = false;
        StopUsing_ClientRpc();

    }

    [ClientRpc]
    public void StartUsing_ClientRpc()
    {
        Map5_HornManager.Instance.CurrentActiveHorns.Add(this);
    }

    [ClientRpc]
    public void StopUsing_ClientRpc()
    {
        Map5_HornManager.Instance.CurrentActiveHorns.Remove(this);
    }

}
