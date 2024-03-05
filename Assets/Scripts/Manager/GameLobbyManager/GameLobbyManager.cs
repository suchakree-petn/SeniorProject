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
    private Lobby joinedLobby;
    protected override void InitAfterAwake()
    {
        InitializeUnityAuthentication();

    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(Random.Range(0, 100000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            createLobbyButton.onClick.AddListener(() => CreateLobby("TestLobby", false));
            quickJoinLobbyButton.onClick.AddListener(() => QuickJoin());
        }
    }

    private async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER, new()
            {
                IsPrivate = isPrivate,
            });
            Allocation allocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(allocation);
            Debug.Log("Create lobby code: "+relayJoinCode);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions()
            {
                Data = new Dictionary<string, DataObject>{
                    {KEY_RELAY_JOIN_CODE,new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)}
                }
            });
            NetworkManager.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            NetworkManager.StartHost();
            NetworkManager.SceneManager.LoadScene("Tutor_Test_FPS_Movement&FreeLook", LoadSceneMode.Single);
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
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            string relayJoinCode = joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;
            Debug.Log("QJ code: "+relayJoinCode);
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
