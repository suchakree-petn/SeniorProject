public interface IDamageable
{
    public void TakeDamage(Damage damage,EntityCharacterData attackerData);
    public void InitHp(EntityCharacterData attackerData);
}