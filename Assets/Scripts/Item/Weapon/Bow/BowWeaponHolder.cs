using UnityEngine;

public class BowWeaponHolder : WeaponHolder<BowBase>
{
    public BowConfig BowConfig = new();

    [Header("Reference")]
    [SerializeField] private Transform firePointTransform;
    public override void UseWeapon()
    {
        if (firePointTransform == null)
        {
            Debug.LogWarning($"No Fire Point transform");
            return;
        }
        Transform arrow = Weapon.GetArrow(position: firePointTransform.position);

        FireArrow(arrow.GetComponent<Rigidbody>());

    }

    private void FireArrow(Rigidbody arrowRb)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, BowConfig.MaxRaycastDistance, BowConfig.targetMask))
        {
            // Calculate direction towards the hit point
            Vector3 direction = (hit.point - firePointTransform.position).normalized;
            // Rotate arrow to face the hit point
            arrowRb.transform.forward = direction;
            // Apply force in the direction of the hit point
            arrowRb.AddForce(direction * BowConfig.ArrowSpeed, ForceMode.Impulse);
        }
        else
        {
            // If ray doesn't hit anything, use the default direction
            arrowRb.transform.forward = firePointTransform.forward.normalized;
            // Apply force in the default direction
            arrowRb.AddForce(arrowRb.transform.forward * BowConfig.ArrowSpeed, ForceMode.Impulse);
        }
    }

}

// [System.Serializable]
// public class BowConfig
// {
//     public float ArrowSpeed;
//     public float MaxRaycastDistance;
//     public LayerMask targetMask;
// }
