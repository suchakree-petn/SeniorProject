using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SwordDamageDealer : MonoBehaviour
{
    public AttackDamage AttackDamage;


    // [SerializeField] private Collider hitBox;
    public bool isHit = true;
    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        // SetKinematic();
        if (other.transform.root.TryGetComponent(out NetworkObject otherNetObj) && isHit)
        {
            // SetParent_ServerRpc(otherNetObj);
            DoDamage(otherNetObj);
        }
    }

    // public virtual void SetKinematic(bool isActive = true)
    // {
    //     arrowRb.isKinematic = isActive;
    //     hitBox.enabled = !isActive;
    // }

    // [ServerRpc(RequireOwnership = false)]
    // public void SetParent_ServerRpc(NetworkObjectReference newParentNetObjRef)
    // {
    //     Debug.Log(newParentNetObjRef.TryGet(out NetworkObject _));

    //     if (!newParentNetObjRef.TryGet(out NetworkObject newParentNetObj)) return;

    //     arrowRb.transform.SetParent(newParentNetObj.transform);
    // }

    public void DoDamage(NetworkObjectReference entityNetObjRef)
    {
        Debug.Log(entityNetObjRef.TryGet(out NetworkObject _));

        if (!entityNetObjRef.TryGet(out NetworkObject entityNetObj)) return;

        entityNetObj.GetComponent<IDamageable>().TakeDamage_ClientRpc(AttackDamage);
    }
}
