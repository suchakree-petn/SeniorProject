using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using QFSW.QC;

public class PlayerManager : CharactorManager<PlayerData>
{
    [SerializeField] private PlayerAction _playerAction;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip knockbackClip;
    [SerializeField] private float playerKnockbackDistance;
    [SerializeField] private Rigidbody2D playerRB;
    [SerializeField] private GameObject skillSlot;
    [SerializeField] private Collider2D hitbox;
    public Action OnPlayerKnockback;
    void Start()
    {
        skillSlot.SetActive(false);
        skillSlot.SetActive(true);
    }
    public override PlayerData GetCharactorData()
    {
        return playerData;
    }
    public override void TakeDamage(DamageDeal damage)
    {
        float damageDeal = 0;
        if (currentHp > 0)
        {
            damageDeal = CalcDamageRecieve(GetCharactorData(), damage);
            currentHp -= damageDeal;
        }
    }
    public override void InitHp()
    {
        currentHp = GetCharactorData().GetMaxHp();
    }
    // public override void CheckDead(GameObject charactor, Elemental damage)
    // {
    //     if (charactor.GetComponent<PlayerManager>().currentHp <= 0)
    //     {
    //         GameController.OnPlayerDead?.Invoke(charactor);
    //     }
    // }

    // public override void Dead(GameObject deadCharactor)
    // {
    //     PlayerInputSystem.Instance.playerAction.Player.Disable();
    //     hitbox.enabled = false;
    //     animator.SetTrigger("isDead");
    //     GameController.Instance.ToResultScene();
    // }
    
}


