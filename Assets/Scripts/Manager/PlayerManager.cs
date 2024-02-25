using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    public float currentHp;
    // public NetworkList<PlayerData> AllPlayerDatas;
    public PlayerData OwnerPlayerData;

    // public Action OnFinishInitAllPlayerData;
    // protected override void InitAfterAwake()
    // {
    // }

    // private void Start()
    // {
    //     playerData_1 = ScriptableObject.CreateInstance<PlayerData>();
    //     playerData_2 = ScriptableObject.CreateInstance<PlayerData>();
    //     if (playerData_1 != null || playerData_2 != null)
    //     {
    //         InitOwnerPlayerData();
    //         OnFinishInitAllPlayerData?.Invoke();
    //     }
    // }

    // [ClientRpc]
    // public void InitPlayerData_ClientRpc(ulong playerId)
    // {
    //     PlayerData data = PlayerData.Cache[playerId];
    //     if (playerData_1 == null)
    //     {
    //         playerData_1 = data;
    //     }
    //     else
    //     {
    //         playerData_2 = data;
    //     }
    // }
    // private void InitOwnerPlayerData()
    // {
    //     OwnerPlayerData = IsServer ? playerData_1 : playerData_2;
    // }
    // public static ulong GetOwnerPlayerId()
    // {
    //     return Instance.OwnerPlayerData.PlayerId;
    // }
    // public static ulong GetPlayerId(int playerOrderIndex)
    // {
    //     return playerOrderIndex == 1 ? Instance.playerData_1.PlayerId : Instance.playerData_2.PlayerId;
    // }
    public void TakeDamage(DamageDeal damage)
    {
        float damageDeal = 0;
        if (currentHp > 0)
        {
            damageDeal = CalcDamageRecieve(OwnerPlayerData, damage);
            currentHp -= damageDeal;
        }
    }
    public float CalcDamageRecieve(PlayerData target, DamageDeal damage)
    {
        return damage._damage - CalcDefense(target);
    }
    float CalcDefense(PlayerData target)
    {
        return target._defenseBase + target._defenseBonus;
    }
    // public override void InitHp()
    // {
    //     currentHp = GetCharactorData().GetMaxHp();
    // }
    protected override void InitAfterAwake()
    {
        // throw new NotImplementedException();
    }
}
