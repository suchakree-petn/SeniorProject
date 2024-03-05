using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public partial class PlayerManager : NetworkSingleton<PlayerManager>
{
    private static Dictionary<ulong, Transform> _playerCharacterPrefab;
    public static Dictionary<ulong, Transform> PlayerCharacterPrefab
    {
        get
        {
            if (_playerCharacterPrefab == null)
            {
                _playerCharacterPrefab = new();
                Transform[] playerChar = Resources.LoadAll<Transform>("Entity/Player/Prefab");
                _playerCharacterPrefab = playerChar.ToDictionary(keys => ulong.Parse(keys.name.Split("_")[0]), val => val);
            }
            return _playerCharacterPrefab;
        }
    }
    public Dictionary<ulong, UserData> PlayerDatas { get; private set; } = new();
    public Dictionary<ulong, GameObject> PlayerGameObjects { get; private set; } = new();
    public Dictionary<ulong, Vector3> PlayerPos
    {
        get
        {
            return PlayerGameObjects.ToDictionary(key => key.Key, val => val.Value.transform.position);
        }
    }
    // public UserData OwnerPlayerData;

    public Action<ulong> OnAfterClientConnect;

    protected override void InitAfterAwake()
    {
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += PlayerManager_OnClientConnectedHandler;
            NetworkManager.OnClientDisconnectCallback += PlayerManager_OnClientDisconnectHandler;
            PlayerManager_OnServerStartedHandler();
        }

    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback -= PlayerManager_OnClientConnectedHandler;
            NetworkManager.OnClientDisconnectCallback -= PlayerManager_OnClientDisconnectHandler;
        }

    }
    private void PlayerManager_OnClientConnectedHandler(ulong clientId)
    {
        Transform playerChar = Instantiate(PlayerCharacterPrefab[1001]);
        playerChar.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);

        Debug.Log("Client Connected");
        PlayerController[] allPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
        Debug.Log("Current player amount: " + allPlayers.Length);
        bool isAdd = false;
        foreach (PlayerController player in allPlayers)
        {
            Debug.Log(player.OwnerClientId);
            if (clientId == player.OwnerClientId)
            {
                if (PlayerGameObjects.TryAdd(clientId, player.gameObject))
                {
                    isAdd = true;
                    break;
                }
            }
        }
        if (!isAdd)
        {
            Debug.LogWarning("Already has this clientId");
        }
        OnAfterClientConnect_ClientRpc(clientId);
    }
    private void PlayerManager_OnServerStartedHandler()
    {
        PlayerManager_OnClientConnectedHandler(0);
    }
    [ClientRpc]
    private void OnAfterClientConnect_ClientRpc(ulong clientId)
    {
        if (clientId != NetworkManager.LocalClientId) return;
        OnAfterClientConnect?.Invoke(clientId);
    }
    private void PlayerManager_OnClientDisconnectHandler(ulong clientId)
    {
        Debug.Log("Client Disconnected");

        PlayerGameObjects.Remove(clientId);
    }
    public static Transform GetPlayerCharacterPrefab(ulong characterId)
    {
        return PlayerCharacterPrefab[characterId];
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchPlayerCharacter_ServerRpc(ulong charId, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        GameObject prevPlayerGO = PlayerGameObjects[clientId];
        GameObject newPlayerGO = Instantiate(
                    GetPlayerCharacterPrefab(charId).gameObject,
                    PlayerPos[clientId],
                    prevPlayerGO.transform.rotation);
        prevPlayerGO.GetComponent<NetworkObject>().Despawn();
        Destroy(prevPlayerGO);

        PlayerGameObjects[clientId] = newPlayerGO;

        newPlayerGO.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
    }
    private void Update()
    {
        // For test
        // GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        // PlayerGameObjects.TryAdd(0, playerGO);
        // if (PlayerGameObjects.TryGetValue(0, out GameObject gameObject)) if (gameObject == null) PlayerGameObjects = new(); ;
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
public enum CharacterType
{
    Supporter,
    FrontLine,
    DamageDealer
}