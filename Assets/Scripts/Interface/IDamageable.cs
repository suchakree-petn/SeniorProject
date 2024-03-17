using Unity.Netcode;

public interface IDamageable
{
    public void TakeDamage_ClientRpc(AttackDamage damage);
    public void TakeHeal_ClientRpc(AttackDamage damage);
    public void InitHp(EntityCharacterData characterData);
}