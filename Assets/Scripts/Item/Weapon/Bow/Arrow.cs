using Unity.Netcode;
using UnityEngine;

public abstract class Arrow : NetworkBehaviour
{
    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || !IsSpawned) return;

        Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        SetKinematic();
        if (other.TryGetComponent<NetworkObject>(out NetworkObject otherNetObj))
        {
            SetParent_ServerRpc(otherNetObj);
        }
    }

    public virtual void SetKinematic(bool isActive = true)
    {
        arrowRb.isKinematic = isActive;
        hitBox.enabled = !isActive;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void SetParent_ServerRpc(NetworkObjectReference newParentNetObjRef)
    {
        if (!newParentNetObjRef.TryGet(out NetworkObject newParentNetObj)) return;

        arrowRb.transform.SetParent(newParentNetObj.transform);
    }
}
