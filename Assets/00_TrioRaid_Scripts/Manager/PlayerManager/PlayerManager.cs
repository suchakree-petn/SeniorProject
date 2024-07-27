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
    public Dictionary<ulong, PlayerCharacterData> PlayerCharacterDatas { get; private set; } = new();

    public Dictionary<ulong, GameObject> PlayerGameObjects { get; private set; } = new();
    public Dictionary<PlayerRole, GameObject> PlayerGameObjectsByRole { get; private set; } = new();
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
            OnClientConnect += PlayerManager_OnClientConnectedHandler;
            NetworkManager.OnClientDisconnectCallback += PlayerManager_OnClientDisconnectHandler;
            // PlayerManager_OnServerStartedHandler();
        }

    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnClientConnect -= PlayerManager_OnClientConnectedHandler;
            NetworkManager.OnClientDisconnectCallback -= PlayerManager_OnClientDisconnectHandler;
        }

    }
    private void PlayerManager_OnClientConnectedHandler(ulong clientId)
    {
        Debug.Log("Id Value: " + clientId);
        ulong PLAYER_CHAR_ID;
        int playerDataIndex = GameMultiplayerManager.Instance.GetPlayerDataIndexFromClientId(clientId);
        var playerDatasList = GameMultiplayerManager.Instance.GetPlayerDataNetworkList();
        int classId = playerDatasList[playerDataIndex].classId;
        switch (classId)
        {
            case 0:
                PLAYER_CHAR_ID = 1001;
                break;
            case 1:
                PLAYER_CHAR_ID = 1002;
                break;
            case 2:
                PLAYER_CHAR_ID = 1003;
                break;
            default:
                PLAYER_CHAR_ID = 1000;
                break;
        }

        Transform playerChar = Instantiate(PlayerCharacterPrefab[PLAYER_CHAR_ID]);
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
                PlayerGameObjectsByRole.Add(player.PlayerCharacterData.PlayerRole, player.gameObject);

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
        Debug.Log("Client Disconnected " + clientId);
        // UserDatas.Remove(clientId);
        PlayerCharacterDatas.Remove(clientId);
        PlayerGameObjects.Remove(clientId);
    }

    public static Transform GetPlayerCharacterPrefab(ulong characterId)
    {
        return PlayerCharacterPrefab[characterId];
    }
    public Transform GetClosestPlayerFrom(Vector3 position)
    {
        float closestDistant = float.MaxValue;
        ulong clientId = default;
        foreach (NetworkClient item in NetworkManager.ConnectedClientsList)
        {
            PlayerController playerController = item.PlayerObject.GetComponent<PlayerController>();
            if(playerController.IsDead) continue;
            
            float distance = Vector3.Distance(position, PlayerPos[item.ClientId]);
            if (distance < closestDistant)
            {
                closestDistant = distance;
                clientId = item.ClientId;
            }
        }
        return PlayerGameObjects[clientId].transform;
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