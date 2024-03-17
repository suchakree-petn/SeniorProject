using Unity.Netcode;

public interface IDamageable
{
    public void TakeDamage_ServerRpc(float damage);
    public void InitHp(EntityCharacterData attackerData);
}