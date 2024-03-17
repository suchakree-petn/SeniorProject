using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealOrb : NetworkBehaviour
{
    public Transform target;
    public Rigidbody healOrbRb;
    public float MoveSpeed = 1;
    private void FixedUpdate()
    {
        if (IsSpawned && IsServer)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            healOrbRb.velocity = dir * MoveSpeed;

        }
    }
}
