using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Archer Ability Data", menuName = "Ability/Archer/Vine Trap")]
public class ArcherAbilityData_VineTrap: AbilityData
{
    public Transform VineTrap_prf;
    public float StopMoveDuration;
    public float PopOffset;
    public float PopDuration;
    public float ThrowForce;
}
