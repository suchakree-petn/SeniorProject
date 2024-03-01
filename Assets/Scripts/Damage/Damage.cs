public class Damage : AttackerBase
{
   public float damage { get; private set; }
   public EntityCharacterData AttackerData { get; private set; }
   public Damage(float damageMultiplier, EntityCharacterData attackerData)
   {
      damage = DamageCalculation(damageMultiplier, attackerData);
      AttackerData = attackerData;
   }
   private float DamageCalculation(float damageMultiplier, EntityCharacterData attackerData)
   {
      return (attackerData.AttackBase + attackerData.AttackBonus) * damageMultiplier;
   }

}
