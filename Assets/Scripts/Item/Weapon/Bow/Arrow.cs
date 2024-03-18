using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class Arrow : NetworkBehaviour
{
    public AttackDamage AttackDamage;

    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    [SerializeField] private GameObject vfx_Hit;
    [SerializeField] private GameObject vfx_HitInstance;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        Invoke(nameof(SelfDestroy), 5);
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || !IsSpawned) return;

        if (other.gameObject.layer == 3)
        {
            SetKinematic();
            return;
        }
        if (other.transform.root.TryGetComponent<PlayerController>(out _) || !other.isTrigger) return;

        Debug.Log($"Arrow {name} collide with {other.gameObject.name}");
        Transform root = other.transform.root;
        if (root.TryGetComponent(out IDamageable damageable)
            && root.TryGetComponent(out EnemyController enemyController)
            && other.CompareTag("Hitbox")
            || other.CompareTag("CriticalHitbox"))
        {
            if (other.CompareTag("CriticalHitbox"))
            {
                AttackDamage.Damage *= 1.5f;
                Debug.LogWarning("Critical");
                vfx_HitInstance = Instantiate(vfx_Hit, transform.position, Quaternion.identity);
            }
            NetworkObject networkObject = root.GetComponent<NetworkObject>();
            SetParent_ServerRpc(networkObject);
            DoDamage(networkObject);
            Invoke(nameof(DestroyVFX), 0.95f);
            Invoke(nameof(SelfDestroy), 1);
        }
    }
    private void DestroyVFX()
    {
        Destroy(vfx_HitInstance);
    }
    private void SelfDestroy()
    {
        if (!IsSpawned) return;
        gameObject.GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
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

        entityNetObj.GetComponent<IDamageable>().TakeDamage_ClientRpc(AttackDamage);
    }
}
