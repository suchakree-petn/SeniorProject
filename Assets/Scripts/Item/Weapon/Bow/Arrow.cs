using Unity.Netcode;
using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    public AttackDamage AttackDamage;
    public LayerMask TargetLayer;

    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private SphereCollider hitBox;
    [SerializeField] private GameObject vfx_Hit;
    private GameObject vfx_HitInstance;

    private void Awake()
    {
        hitBox.includeLayers = TargetLayer;
    }
    protected virtual void Start()
    {
        Destroy(gameObject, 1);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
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
            && other.TryGetComponent(out EnemyController enemyController)
            && other.CompareTag("Hitbox")
            || other.CompareTag("CriticalHitbox"))
        {
            if (other.CompareTag("CriticalHitbox"))
            {
                AttackDamage.Damage *= 1.5f;
                Debug.LogWarning("Critical");
                DoDamage(damageable);
                hitBox.enabled = false;
                vfx_HitInstance = Instantiate(vfx_Hit, transform.position, Quaternion.identity);

                Destroy(vfx_HitInstance, 0.95f);
                Destroy(gameObject, 1);
                return;

            }
            DoDamage(damageable);
            hitBox.enabled = false;
            vfx_HitInstance = Instantiate(vfx_Hit, transform.position, Quaternion.identity);
            Destroy(vfx_HitInstance, 0.95f);
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
