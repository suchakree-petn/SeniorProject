using UnityEngine;

[CreateAssetMenu(fileName = "New Tank Ability Data", menuName = "Ability/Tank/Ground Smash")]
public class TankAbilityData_GroundSmash : AbilityData
{
    public Transform VFX_prf;
    public Transform StunVFX_prf;
    public float VFXDuration;
    public float StunVFXDuration;
    public float StunVFXOffset;
    public float StopMoveDuration;
    public float StunDuration;
    public float Radius;
    public float PositionOffsetX;
    public float PositionOffsetY;
    public float PositionOffsetZ;
    public float DamageMultiplier;
}
