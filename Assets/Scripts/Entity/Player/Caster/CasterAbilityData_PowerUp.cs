using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Caster Ability Data", menuName = "Ability/Caster/Power Up")]
public class CasterAbilityData_PowerUp : AbilityData
{
    public Transform VFX_prf;
    public float VFXOffset;
    public float BuffDuration;
    public float AttackBonus;
}
