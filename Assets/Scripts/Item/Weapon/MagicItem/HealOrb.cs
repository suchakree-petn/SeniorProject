using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealOrb : NetworkBehaviour
{
    public AttackDamage AttackDamage;
    public float MoveSpeed = 1;

    public Transform target;

    [Header("Reference")]
    public Rigidbody healOrbRb;


    private void FixedUpdate()
    {
        if (IsSpawned && IsServer)
        {
            Vector3 dir;
            if (target != null)
            {
                dir = (target.position - transform.position).normalized;
            }
            else
            {
                dir = transform.forward;
            }
            healOrbRb.velocity = dir * MoveSpeed;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        Transform root = other.transform.root;
        if (root.TryGetComponent(out IDamageable damageable) 
            && root.TryGetComponent(out PlayerController playerController) 
            && (long)playerController.OwnerClientId != AttackDamage.AttackerClientId 
            && other.CompareTag("Hitbox"))
        {
            Debug.Log($"Orb enter {root.name}");
            damageable.TakeHeal_ClientRpc(AttackDamage);
            Debug.Log($"{other.transform.root.name} took {AttackDamage.Damage} damages");
        }
    }
}
