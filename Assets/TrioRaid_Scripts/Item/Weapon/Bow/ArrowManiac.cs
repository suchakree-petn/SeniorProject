using Unity.Netcode;
using UnityEngine;

public class ArrowManiac : NetworkBehaviour
{
    public AttackDamage AttackDamage;
    public LayerMask TargetLayer;

    [Header("Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private SphereCollider hitBox;
    public ParticleSystem glitter;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        hitBox.includeLayers = TargetLayer;

    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.layer == 3)
        {
            SetKinematic();
            return;
        }
        Transform root = other.transform.root;

        if (root.TryGetComponent<PlayerController>(out _) || !other.isTrigger) return;

        if (root.TryGetComponent(out IDamageable damageable)
            && root.TryGetComponent(out EnemyController _)
            && other.CompareTag("Hitbox"))
        {
            Debug.Log("Do damage to: " + root.name);
            DoDamage(damageable);
            hitBox.enabled = false;
            return;
        }
    }
    public virtual void SetKinematic(bool isActive = true)
    {
        arrowRb.isKinematic = isActive;
        hitBox.enabled = !isActive;
    }

    public virtual void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage_ServerRpc(AttackDamage);
    }
    private void NetworkDespawn()
    {
        Debug.Log("Network Despawn");
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}
