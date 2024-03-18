using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Animator animator; 
    [SerializeField] private NetworkAnimator networkAnimator;
    
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

    public void SetTriggerNetworkAnimation(string name)
    {
        networkAnimator.SetTrigger(name);
    }
    public void SetBool(string name,bool value)
    {
        animator.SetBool(name,value);
    }
}

