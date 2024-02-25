using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(DamageDeal elementalDamage);
    public void InitHp();
}