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
                Transform[] playerChar = Resources.LoadAll<Transform>("Prefab/Entity/Player");
                _playerCharacterPrefab = playerChar.ToDictionary(keys => ulong.Parse(keys.name.Split("_")[0]), val => val);
            }
            return _playerCharacterPrefab;
        }
    }
    // public Dictionary<ulong, UserData> UserDatas { get; private set; } = new();
    public Dictionary<ulong, PlayerCharacterData> PlayerCharacterDatas { get; private set; } = new();

    public Dictionary<ulong, GameObject> PlayerGameObjects { get; private set; } = new();
    public Dictionary<ulong, Vector3> PlayerPos
    {
        get
        {
            return PlayerGameObjects.ToDictionary(key => key.Key, val => val.Value.transform.position);
        }
    }

    public Action<ulong> OnClientConnect;
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
            // PlayerManager_OnServerStartedHandler();
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
        OnClientConnect?.Invoke(clientId);

        Transform playerChar = Instantiate(PlayerCharacterPrefab[1002]);
        playerChar.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        Debug.Log("Client Connected");
        PlayerController[] allPlayers = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
        Debug.Log("Current player amount: " + allPlayers.Length);
        // bool isAddUserData = false;
        bool isAddPlayerCharacterData = false;
        bool isAddAll = isAddPlayerCharacterData;

        foreach (PlayerController player in allPlayers)
        {
            if (clientId == player.OwnerClientId)
            {
                // if (UserDatas.TryAdd(clientId, UserManager.Instance.UserData))
                // {
                //     isAddUserData = true;
                // }
                // else
                // {
                //     Debug.LogWarning($"Already contain this userId");
                // }
                if (PlayerCharacterDatas.TryAdd(clientId, player.PlayerCharacterData))
                {
                    isAddPlayerCharacterData = true;
                }
                else
                {
                    Debug.LogWarning($"Already contain this clientId");
                }

                if (PlayerGameObjects.TryAdd(clientId, player.gameObject))
                {
                    isAddAll = true;
                }
                else
                {
                    Debug.LogWarning($"Already contain this clientId");
                }
            }
        }
        if (!isAddAll)
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
        // UserDatas.Remove(clientId);
        PlayerCharacterDatas.Remove(clientId);
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



}