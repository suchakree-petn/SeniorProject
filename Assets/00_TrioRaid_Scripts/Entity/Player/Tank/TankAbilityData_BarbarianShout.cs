using UnityEngine;

[CreateAssetMenu(fileName = "New Tank Ability Data", menuName = "Ability/Tank/Barbarian Shout")]
public class TankAbilityData_BarbarianShout : AbilityData
{
    public Transform VFX_prf;
    public float VFXDuration;
    public float StopMoveDuration;
    public float TauntDuration;
    public float Radius;
    public float DefenseBonus;
    public float PositionOffset;
}
