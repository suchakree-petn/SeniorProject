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

public partial class GameLobbyManager : NetworkSingleton<GameLobbyManager>
{
    [SerializeField] private const int MAX_PLAYER = 3;
    [SerializeField] private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    [SerializeField] private const string KEY_PLAYER_NAME = "PlayerName";
    [SerializeField] private const string KEY_STAGE_ID = "StageId";
    private Lobby joinedLobby;
    protected override void InitAfterAwake()
    {
        InitializeUnityAuthentication();

        createLobbyButton.onClick.AddListener(() => CreateLobby("TestLobby", false));
        quickJoinLobbyButton.onClick.AddListener(() => QuickJoin());
        codeJoinLobbyButton.onClick.AddListener(() => JoinByCode(codeJoinLobbyInputField.text));
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(Random.Range(0, 100000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    private async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER, new()
            {
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
                    {KEY_STAGE_ID,new(DataObject.VisibilityOptions.Public,"01")}
                }
            });
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
            NetworkManager.SceneManager.LoadScene("Thanva_Map_Tester", LoadSceneMode.Single);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogWarning(e);
        }

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
}
