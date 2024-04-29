using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameLobbyManager : NetworkSingleton<GameLobbyManager>
{
    [SerializeField] public const int MAX_PLAYER = 3;
    [SerializeField] public const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    [SerializeField] public const string KEY_PLAYER_NAME = "PlayerName";
    [SerializeField] public const string KEY_STAGE_ID = "StageId";
    [SerializeField] public const string KEY_TANK_ID = "TANK";
    [SerializeField] public const string KEY_ARCHER_ID = "ARCHER";
    [SerializeField] public const string KEY_CASTER_ID = "CASTER";
    private float heartbeatTimer;
    private float listLobbiesTimer;
    // +_______________________________________________

    
    public event EventHandler OnCreateLobbyStarted;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnJoinStarted;
    public event EventHandler OnQuickJoinFailed;
    public event EventHandler OnJoinFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }
    // +_______________________________________________
    private Lobby joinedLobby;
    protected override void InitAfterAwake()
    {
        InitializeUnityAuthentication();
    }

    private void Update() {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        HandleLobbyHeartbeat();
        HandlePeriodicListLobbies();
    
    }
    
    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0, 100000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate, int stage)
    {
        OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
        try
        {
            var options = new CreateLobbyOptions {
            IsPrivate = isPrivate,
            Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,UserManager.Instance.UserData.UserName.ToString())}
                    }
                },
            Data = new Dictionary<string, DataObject>
                {
                    {KEY_STAGE_ID,new DataObject(DataObject.VisibilityOptions.Public,stage.ToString())},
                    {KEY_TANK_ID,new DataObject(DataObject.VisibilityOptions.Public,"0")},
                    {KEY_ARCHER_ID,new DataObject(DataObject.VisibilityOptions.Public,"0")},
                    {KEY_CASTER_ID,new DataObject(DataObject.VisibilityOptions.Public,"0")}
                    
                }
                
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER, options);

            joinedLobby = lobby;

            OnJoinStarted?.Invoke(this, EventArgs.Empty);
            Debug.Log("PlayerCount: " + joinedLobby.Players.Count);

            Debug.Log("Stage ID: " + joinedLobby.Data[KEY_STAGE_ID].Value);

            foreach (Player player in joinedLobby.Players)
            {
                Debug.Log("Player name: " + player.Data[KEY_PLAYER_NAME].Value);
            }
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE,new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });
            NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            Debug.Log("Create lobby code: " + joinedLobby.LobbyCode);
            GameMultiplayerManager.Instance.StartHost();

            // SceneManager.LoadScene("Thanva_InLobby");
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }

    }
    public async void DeleteLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public Lobby GetJoinedLobby() {
        return joinedLobby;
    }
    private async void HandleLobbyHeartbeat() {
        if (IsLobbyHost()) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }
    public bool IsLobbyHost() {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    public async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            return await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
            return default;
        }
    }
    public async void QuickJoin()
    {
        try
        {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync(new()
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,UserManager.Instance.UserData.UserName.ToString())}
                    }
                }
            });
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            Debug.Log("Lobby Id: " + joinedLobby.Data[KEY_STAGE_ID].Value);
            Debug.Log("QJ code: " + relayJoinCode);
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            GameMultiplayerManager.Instance.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }
    public async void JoinWithId(string lobbyId) {
        OnJoinStarted?.Invoke(this, EventArgs.Empty);
        try {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            GameMultiplayerManager.Instance.StartClient();
        } catch (LobbyServiceException e) {
            Debug.Log(e);
            OnJoinFailed?.Invoke(this, EventArgs.Empty);
        }
    }
    public async void JoinByCode(string lobbyCode)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new()
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {KEY_PLAYER_NAME,new(PlayerDataObject.VisibilityOptions.Member,UserManager.Instance.UserData.UserName.ToString())}
                    }
                }
            });
            Debug.Log("Join code: " + lobbyCode);

            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            GameMultiplayerManager.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYER - 1);
            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
            return default;
        }

    }
    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
            return default;
        }

    }
    public async void LeaveLobby() {
        if (joinedLobby != null) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                joinedLobby = null;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public async void KickPlayer(string playerId) {
        if (IsLobbyHost()) {
            try {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }
    public Lobby GetLobby() {
        return joinedLobby;
    }
    public async void RefreshLobbyList() {
        try {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs { lobbyList = lobbyListQueryResponse.Results });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    private async void ListLobbies() {
        try {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
                Filters = new List<QueryFilter> {
                  new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs {
                lobbyList = queryResponse.Results
            });
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }
    private void HandlePeriodicListLobbies() {
        if (joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn && 
            SceneManager.GetActiveScene().name == "Thanva_MainMenu_UserDataPersistence") {

            listLobbiesTimer -= Time.deltaTime;
            if (listLobbiesTimer <= 0f) {
                float listLobbiesTimerMax = 3f;
                listLobbiesTimer = listLobbiesTimerMax;
                ListLobbies();
            }
        }
    }
}
