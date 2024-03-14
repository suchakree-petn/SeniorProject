using UnityEngine;

public abstract class Arrow : MonoBehaviour
{
    [Header("Base Reference")]
    [SerializeField] private Rigidbody arrowRb;
    [SerializeField] private Collider hitBox;
    public abstract void OnCollisionEnter(Collision other);

    public virtual void SetKinematic(bool isActive = true)
    {
        arrowRb.isKinematic = isActive;
        hitBox.enabled = !isActive;
    }
}
