
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : EntityHealth
{
    [Header("Player Reference")]
    [SerializeField] protected PlayerController playerController;
    public Slider miniHpBar;

  
    public override void TakeDamage(AttackDamage damage, float defense)
    {
        if (CurrentHealth > 0)
        {
            base.TakeDamage(damage, defense);

            if (CurrentHealth < 0)
            {
                currentHealth.Value = 0;
            }
        }
        UIHPBar.Instance.SetHP_ServerRpc(NetworkManager.LocalClientId);
    }

    public override void TakeHeal(AttackDamage damage)
    {
        float maxHp = playerController.PlayerCharacterData.GetMaxHp();
        if (CurrentHealth < maxHp)
        {
            base.TakeHeal(damage);

            if (CurrentHealth > maxHp)
            {
                currentHealth.Value = maxHp;
            }
        }
        UIHPBar.Instance.SetHP_ServerRpc(NetworkManager.LocalClientId);

    }
    private void OnEnable()
    {
        InitHp(playerController.PlayerCharacterData);
    }
}
