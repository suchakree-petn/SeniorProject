using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{


    // [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        // mainMenuButton.onClick.AddListener(() => {
        //     GameLobbyManager.Instance.LeaveLobby();
        //     Loader.Load(Loader.Scene.MainMenuScene);
        // });
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.JoinByCode(joinCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = GameMultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayerManager.Instance.SetPlayerName(newText);
        });
        GameLobbyManager.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        GameLobbyManager.Instance.OnKickedFromLobby += GameLobbyManager_OnKickedFromLobby;

        // UpdateLobbyList(new List<Lobby>());
    }

    private void GameLobbyManager_OnKickedFromLobby(object sender, GameLobbyManager.LobbyEventArgs e)
    {
        Debug.Log("Load back to lobby");
        if (NetworkManager.Singleton.IsServer)
            GameMultiplayerManager.Instance.GetPlayerDataNetworkList().Clear();
        Loader.Load(Loader.Scene.LobbyScene);
        NetworkManager.Singleton.Shutdown();
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().UpdateLobby(lobby);
            lobbyTransform.GetChild(3).GetComponent<ShowClassUI>().UpdateLobby(lobby);
        }
    }

    private void OnDestroy()
    {
        GameLobbyManager.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }

}