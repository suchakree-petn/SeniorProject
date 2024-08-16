using Unity.Netcode;

[System.Serializable]
public class AttackDamage : INetworkSerializable
{
   public DamageType Type;
   public bool IsCriticalable;
   public float Damage;
   public long AttackerClientId = -1;

   public AttackDamage(float damageMultiplier, float attack, DamageType damageType = DamageType.Default, bool isCriticalable = false, long attackerClientId = -1)
   {
      Type = damageType;
      IsCriticalable = isCriticalable;
      Damage = DamageCalculation(damageMultiplier, attack);
      AttackerClientId = attackerClientId;
   }
   public AttackDamage()
   {
      Type = DamageType.Default;
      IsCriticalable = false;
      Damage = 19;
      AttackerClientId = 11;
   }

   private float DamageCalculation(float damageMultiplier, float attack)
   {
      return attack * damageMultiplier;
   }

   public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
   {
      serializer.SerializeValue(ref Type);
      serializer.SerializeValue(ref IsCriticalable);
      serializer.SerializeValue(ref Damage);
      serializer.SerializeValue(ref AttackerClientId);
   }
}
public enum DamageType
{
   Default,
   HitScan,
   Projectile,
   Melee
}
