using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkSingleton<PlayerManager>
{
    public float currentHp;
    public Dictionary<ulong, PlayerData> PlayerDatas { get; private set; } = new();
    public Dictionary<ulong, GameObject> PlayerGameObjects { get; private set; } = new();
    public PlayerData OwnerPlayerData;

    // public Action OnFinishInitAllPlayerData;
    protected override void InitAfterAwake()
    {
    }
    public override void OnNetworkSpawn()
    {

    }
    private void Update()
    {
        // For test
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        PlayerGameObjects.TryAdd(0, playerGO);
        if (PlayerGameObjects.TryGetValue(0, out GameObject gameObject)) if (gameObject == null) PlayerGameObjects = new(); ;
    }
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

}
