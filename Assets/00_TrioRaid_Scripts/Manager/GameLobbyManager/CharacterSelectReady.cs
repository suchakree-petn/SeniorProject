using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{


    public static CharacterSelectReady Instance { get; private set; }


    public event EventHandler OnReadyChanged;


    private Dictionary<ulong, bool> playerReadyDictionary;

    public bool Test_One_Player;
    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }


    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        if (!Test_One_Player)
        {
            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
                {
                    // This player is NOT ready
                    allClientsReady = false;
                    break;
                }
            }
        }
        else
        {
            // Debug with one player
            NetworkManager.SceneManager.OnLoadEventCompleted += OnNetworkManagerOnLoadEventCompleted;
            Loader.LoadNetworkString(GetStageName());
            return;
        }

        Debug.Log("Players.Count " + GameLobbyManager.Instance.GetJoinedLobby().Players.Count);

        if (allClientsReady && GameMultiplayerManager.Instance.GetPlayerDataNetworkList().Count == 3)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted += OnNetworkManagerOnLoadEventCompleted;
            Loader.LoadNetworkString(GetStageName());
        }
    }
    private string GetStageName(){
        return GameLobbyManager.Instance.GetLobby().Data["StageId"].Value;
    }

    private void OnNetworkManagerOnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            Debug.Log($"Client {clientId} connected");
            PlayerManager.Instance.OnClientConnect?.Invoke(clientId);
        }

        GameLobbyManager.Instance.DeleteLobby();
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

}