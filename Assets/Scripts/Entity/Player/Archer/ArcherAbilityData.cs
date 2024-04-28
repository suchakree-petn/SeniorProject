using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Archer Ability Data", menuName = "Ability/Archer")]
public class ArcherAbilityData : AbilityData
{
    public Transform ArrowManiac_prf;
    public Transform ArrowManiac_SpawnGroup;
    public float Duration;
    public float Distance;
    public float ArrowSpeed;
}
