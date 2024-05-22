using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayerManager : NetworkSingleton<GameMultiplayerManager>
{


    public const int MAX_PLAYER_AMOUNT = 3;
    private const string KEY_PLAYER_NAME = "PlayerNameMultiplayer";


    public static bool playMultiplayer = true;


    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;


    // [SerializeField] private KitchenObjectListSO kitchenObjectListSO;
    [SerializeField] private List<GameObject> playerClassList;
    [SerializeField] private List<Material> readyMaterial;


    public NetworkList<PlayerData> playerDataNetworkList;
    private string playerName;


    protected override void InitAfterAwake()
    {
        playerName = PlayerPrefs.GetString(KEY_PLAYER_NAME, "PlayerName" + UnityEngine.Random.Range(100, 1000));

        playerDataNetworkList = new NetworkList<PlayerData>(null, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
    public NetworkList<PlayerData> GetPlayerDataNetworkList()
    {
        return playerDataNetworkList;
    }
    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;

        PlayerPrefs.SetString(KEY_PLAYER_NAME, playerName);
    }
    public Material GetMaterial(bool ready)
    {
        Material material;
        if (ready)
        {
            material = readyMaterial[1];
        }
        else
        {
            material = readyMaterial[0];
        }
        return material;
    }
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback -= NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;

    }
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            PlayerData playerData = playerDataNetworkList[i];
            if (playerData.clientId == clientId)
            {
                // Disconnected!
                playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        playerDataNetworkList.Add(new PlayerData
        {
            clientId = clientId,
            classId = GetFirstUnusedClassId(),
        });
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        connectionApprovalResponse.Approved = true;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();
    }




    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc(GetPlayerName());
        SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerName = playerName;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.playerId = playerId;

        playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    // public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
    //     SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference) {
    //     KitchenObjectSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

    //     kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
    //     IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

    //     if (kitchenObjectParent.HasKitchenObject()) {
    //         // Parent already spawned an object
    //         return;
    //     }

    //     Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

    //     NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
    //     kitchenObjectNetworkObject.Spawn(true);

    //     KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();


    //     kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
    // }

    // public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO) {
    //     return kitchenObjectListSO.kitchenObjectSOList.IndexOf(kitchenObjectSO);
    // }

    // public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex) {
    //     return kitchenObjectListSO.kitchenObjectSOList[kitchenObjectSOIndex];
    // }



    // public void DestroyKitchenObject(KitchenObject kitchenObject) {
    //     DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
    //     kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

    //     if (kitchenObjectNetworkObject == null) {
    //         // This object is already destroyed
    //         return;
    //     }

    //     KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    //     ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

    //     kitchenObject.DestroySelf();
    // }

    // [ClientRpc]
    // private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference) {
    //     kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
    //     KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

    //     kitchenObject.ClearKitchenObjectOnParent();
    // }



    public bool IsPlayerIndexConnected(int playerIndex)
    {
        // Debug.Log(playerDataNetworkList.Count);
        return playerIndex < playerDataNetworkList.Count;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            if (playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }
        return default;
    }

    public PlayerData GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerDataNetworkList[playerIndex];
    }

    public GameObject GetPlayerClass(int classId)
    {
        return playerClassList[classId];
    }

    public void ChangePlayerClass(int classId)
    {
        ChangePlayerClassServerRpc(classId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePlayerClassServerRpc(int classId, ServerRpcParams serverRpcParams = default)
    {
        if (!IsClassAvailable(classId))
        {
            Debug.Log("Class " + classId + " is not available");
            // Color not available
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = playerDataNetworkList[playerDataIndex];

        playerData.classId = classId;
        playerDataNetworkList[playerDataIndex] = playerData;

        List<int> classSum = new List<int>();
        for (int i = 0; i < playerDataNetworkList.Count; i++)
        {
            classSum.Add(playerDataNetworkList[i].classId);
        }
        PlayerPrefs.SetString(GameLobbyManager.KEY_TANK_ID, "false");
        PlayerPrefs.SetString(GameLobbyManager.KEY_ARCHER_ID, "false");
        PlayerPrefs.SetString(GameLobbyManager.KEY_CASTER_ID, "false");
        foreach (int _classId in classSum)
        {
            Debug.Log("foreach " + _classId);
            if (_classId == 0)
            {
                PlayerPrefs.SetString(GameLobbyManager.KEY_TANK_ID, "true");
            }
            if (_classId == 1)
            {
                PlayerPrefs.SetString(GameLobbyManager.KEY_ARCHER_ID, "true");
            }
            if (_classId == 2)
            {
                PlayerPrefs.SetString(GameLobbyManager.KEY_CASTER_ID, "true");
            }

        }
    }

    private bool IsClassAvailable(int classId)
    {
        foreach (PlayerData playerData in playerDataNetworkList)
        {
            if (playerData.classId == classId)
            {
                // Already in use
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedClassId()
    {
        for (int i = 0; i < playerClassList.Count; i++)
        {
            if (IsClassAvailable(i))
            {
                return i;
            }
        }
        Debug.Log("Class error");
        return -1;
    }



    // public void KickPlayer(ulong clientId) {
    //     NetworkManager.Singleton.DisconnectClient(clientId);
    //     NetworkManager_Server_OnClientDisconnectCallback(clientId);
    // }

}
