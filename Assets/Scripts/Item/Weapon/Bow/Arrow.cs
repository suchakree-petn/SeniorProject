using Unity.Netcode;
using UnityEngine;

public abstract class Arrow : NetworkBehaviour
{
    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    public virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        SetKinematic();
        SetParent(other.transform);
    }

    public virtual void SetKinematic(bool isActive = true)
    {
        arrowRb.isKinematic = isActive;
        hitBox.enabled = !isActive;
    }

    public virtual void SetParent(Transform newParent)
    {
        if (!newParent.TryGetComponent<NetworkObject>(out _)) return;
        
        arrowRb.transform.SetParent(newParent);
    }
}
