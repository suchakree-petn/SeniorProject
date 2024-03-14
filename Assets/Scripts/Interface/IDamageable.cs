public interface IDamageable
{
    public void TakeDamage(AttackDamage damage,EntityCharacterData attackerData);
    public void InitHp(EntityCharacterData attackerData);
}