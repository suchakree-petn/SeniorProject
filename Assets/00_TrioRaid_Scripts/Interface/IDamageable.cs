using Unity.Netcode;

public interface IDamageable
{
    public void TakeDamage(AttackDamage damage);
    public void TakeDamage_ServerRpc(AttackDamage damage);
    public void TakeDamage_ClientRpc(AttackDamage damage);
    public void TakeHeal_ServerRpc(AttackDamage damage);
    public void TakeHeal_ClientRpc(AttackDamage damage);
}