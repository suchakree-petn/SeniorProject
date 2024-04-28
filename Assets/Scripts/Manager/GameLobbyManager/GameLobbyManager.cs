using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
    [SerializeField] public const string KEY_TANK_ID = "0";
    [SerializeField] public const string KEY_ARCHER_ID = "0";
    [SerializeField] public const string KEY_CASTER_ID = "0";
    [SerializeField] private TMP_InputField lobbyname;
    [SerializeField] private GameObject popUpCreateLobby;
    [SerializeField] private TMP_Dropdown gameStage;
    private float heartbeatTimer;
    // +_______________________________________________
    public event EventHandler OnLeftLobby;

    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobbyUpdate;
    public event EventHandler<LobbyEventArgs> OnKickedFromLobby;
    public event EventHandler<LobbyEventArgs> OnLobbyGameModeChanged;
    public class LobbyEventArgs : EventArgs {
        public Lobby lobby;
    }
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs {
        public List<Lobby> lobbyList;
    }
    // +_______________________________________________
    private Lobby joinedLobby;
    protected override void InitAfterAwake()
    {
        InitializeUnityAuthentication();
        createLobbyButton.onClick.AddListener(() => popUpCreateLobby.SetActive(true));
        popUpCreateButton.onClick.AddListener(() => {CreateLobby(lobbyname.text, false, gameStage.value+1);popUpCreateLobby.SetActive(false);});
        quickJoinLobbyButton.onClick.AddListener(() => QuickJoin());
        codeJoinLobbyButton.onClick.AddListener(() => JoinByCode(codeJoinLobbyInputField.text));
    }
    private void Update() {
        //HandleRefreshLobbyList(); // Disabled Auto Refresh for testing with multiple builds
        // HandleLobbyHeartbeat();
        // HandleLobbyPolling();
    
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
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions {
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
                    {KEY_STAGE_ID,new DataObject(DataObject.VisibilityOptions.Public,stage.ToString())}
                    // {KEY_TANK_ID,new DataObject(DataObject.VisibilityOptions.Public,stage.ToString())},
                    // {KEY_ARCHER_ID,new DataObject(DataObject.VisibilityOptions.Public,stage.ToString())},
                    // {KEY_CASTER_ID,new DataObject(DataObject.VisibilityOptions.Public,stage.ToString())}
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER, options);

            joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
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
            NetworkManager.StartHost();
            NetworkManager.SceneManager.LoadScene("Thanva_InLobby", LoadSceneMode.Single);
            // Loader.LoadNetwork(Loader.Scene.Thanva_InLobby);
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

            NetworkManager.StartClient();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
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

            NetworkManager.StartClient();
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
}
