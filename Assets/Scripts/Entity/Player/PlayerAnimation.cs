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

    public void SetLayerWeight(int layerIndex, float weight)
    {
        animator.SetLayerWeight(layerIndex, weight);
    }

    public float GetLayerWeight(int layerIndex)
    {
        return animator.GetLayerWeight(layerIndex);
    }

    public void SetFloat(string name, float value)
    {
        animator.SetFloat(name, value);
    }
<<<<<<< HEAD
    public void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
    public void SetTrigger(string name)
    {
        animator.SetTrigger(name);
=======

    public void SetTriggerNetworkAnimation(string name)
    {
        networkAnimator.SetTrigger(name);
    }
    public void SetBool(string name,bool value)
    {
        animator.SetBool(name,value);
>>>>>>> 29e8d0917925913c44b6a93cc245a254a455df27
    }
}

