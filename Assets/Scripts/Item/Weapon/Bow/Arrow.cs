using Unity.Netcode;
using UnityEngine;

public abstract class Arrow : NetworkBehaviour
{
    public AttackDamage AttackDamage;

    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || !IsSpawned) return;

        Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        SetKinematic();
        if (other.transform.root.TryGetComponent(out NetworkObject otherNetObj))
        {
            SetParent_ServerRpc(otherNetObj);
            DoDamage(otherNetObj);
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
        Debug.Log(newParentNetObjRef.TryGet(out NetworkObject _));

        if (!newParentNetObjRef.TryGet(out NetworkObject newParentNetObj)) return;

        arrowRb.transform.SetParent(newParentNetObj.transform);
    }

    public virtual void DoDamage(NetworkObjectReference entityNetObjRef)
    {
        Debug.Log(entityNetObjRef.TryGet(out NetworkObject _));

        if (!entityNetObjRef.TryGet(out NetworkObject entityNetObj)) return;

        entityNetObj.GetComponent<IDamageable>().TakeDamage_ServerRpc(AttackDamage.Damage);
    }
}
