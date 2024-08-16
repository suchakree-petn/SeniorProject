using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Archer Ability Data", menuName = "Ability/Archer/Arrow Maniac")]
public class ArcherAbilityData_ArrowManiac : AbilityData
{
    public Transform ArrowManiac_prf;
    public Transform ArrowManiac_SpawnGroup;
    public Transform VFX_Ground_prf;
    public Transform VFX_Arrow_prf;
    public float Duration;
    public float TimeInterval;
    public float Distance;
    public float ArrowSpeed;
    public float DamageMultiplier;
    public float VFXOffset;
    public float VFXDuration;

}
