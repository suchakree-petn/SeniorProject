using Unity.Netcode;
using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    public AttackDamage AttackDamage;

    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    [SerializeField] private GameObject vfx_Hit;
    private GameObject vfx_HitInstance;

    protected virtual void Start()
    {
        Invoke(nameof(SelfDestroy), 5);
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            gameObject.isStatic = true;
            SetKinematic();
            return;
        }
        if ((ulong)AttackDamage.AttackerClientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"Not owner");
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
            SetParent(other.transform);
            DoDamage(damageable);
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
        Destroy(gameObject);
    }
    public virtual void SetKinematic(bool isActive = true)
    {
        arrowRb.isKinematic = isActive;
        hitBox.enabled = !isActive;
    }

    public virtual void SetParent(Transform newParent)
    {
        arrowRb.transform.SetParent(newParent);
    }

    public virtual void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage_ServerRpc(AttackDamage);
    }
}
