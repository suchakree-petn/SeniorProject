using Unity.Netcode;
using UnityEngine;

public class ArrowManiac : NetworkBehaviour
{
    public AttackDamage AttackDamage;
    public LayerMask TargetLayer;

    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private SphereCollider hitBox;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        hitBox.includeLayers = TargetLayer;

        Destroy(gameObject, 5);
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.gameObject.layer == 3)
        {
            SetKinematic();
            return;
        }
        if ((ulong)AttackDamage.AttackerClientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        if (other.transform.root.TryGetComponent<PlayerController>(out _) || !other.isTrigger) return;

        Transform root = other.transform.root;
        if (root.TryGetComponent(out IDamageable damageable)
            && other.TryGetComponent(out EnemyController _)
            && other.CompareTag("Hitbox")
            || other.CompareTag("CriticalHitbox"))
        {
            if (other.CompareTag("CriticalHitbox"))
            {
                AttackDamage.Damage *= 1.5f;
                Debug.LogWarning("Critical");
                DoDamage(damageable);
                hitBox.enabled = false;

                Destroy(gameObject, 1);
                return;

            }
            DoDamage(damageable);
            hitBox.enabled = false;
            Destroy(gameObject, 1);
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
}
