using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator;
    public void SetMoveVelocityX(float velocityX)
    {
        animator.SetFloat("MoveVelocityX", velocityX);
    }
    public void SetMoveVelocityZ(float velocityZ)
    {
        animator.SetFloat("MoveVelocityZ", velocityZ);
    }

}
