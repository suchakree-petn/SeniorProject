using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake() {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start() {
        GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayer_OnFailedToJoinGame;
        GameLobbyManager.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
        GameLobbyManager.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
        GameLobbyManager.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
        GameLobbyManager.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
        GameLobbyManager.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void GameLobby_OnQuickJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void GameLobby_OnJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to join Lobby!");
    }

    private void GameLobby_OnJoinStarted(object sender, System.EventArgs e) {
        ShowMessage("Joining Lobby...");
    }

    private void GameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to create Lobby!");
    }

    private void GameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
        ShowMessage("Creating Lobby...");
    }

    private void GameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        if (NetworkManager.Singleton.DisconnectReason == "") {
            ShowMessage("Failed to connect");
        } else {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayer_OnFailedToJoinGame;
        GameLobbyManager.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
        GameLobbyManager.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
        GameLobbyManager.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
        GameLobbyManager.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
        GameLobbyManager.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
    }

}