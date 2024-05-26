using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Caster Ability Data", menuName = "Ability/Caster/Blessing Shield")]
public class CasterAbilityData_BlessingShield : AbilityData
{
    public Transform Shield_prf;
    public float ShieldOffset;
    public float ShieldDuration;
    public float BonusDefense = 1000000;
    public float castDelay;
}
