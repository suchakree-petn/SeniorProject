using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharactorManager<T> : MonoBehaviour, IDamageable
{
    public float currentHp;

    public T charactorData { private get; set; }
    public Coroutine restoreCoroutine;

    public abstract void TakeDamage(DamageDeal damage);

    public abstract T GetCharactorData();

    float CalcDefense(CharactorData target)
    {
        return target._defenseBase + target._defenseBonus;
    }

    public float CalcDamageRecieve(CharactorData target, DamageDeal damage)
    {
        return damage._damage - CalcDefense(target);
    }
    public virtual void InitHp() { }

}
