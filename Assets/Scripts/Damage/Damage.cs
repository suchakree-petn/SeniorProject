public class AttackDamage
{
   public DamageType Type;
   public bool IsCriticalable;
   public float Damage { get; private set; }
   public EntityCharacterData AttackerData { get; private set; }

   public AttackDamage(float damageMultiplier, EntityCharacterData attackerData, DamageType damageType = DamageType.Default,bool isCriticalable = false)
   {
      Type = damageType;
      IsCriticalable = isCriticalable;
      Damage = DamageCalculation(damageMultiplier, attackerData);
      AttackerData = attackerData;
   }

   private float DamageCalculation(float damageMultiplier, EntityCharacterData attackerData)
   {
      return (attackerData.AttackBase + attackerData.AttackBonus) * damageMultiplier;
   }

}
public enum DamageType
   {
      Default,
      HitScan,
      Projectile,
      Melee
   }
