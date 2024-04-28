using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour {

    
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI stageText;


    private Lobby lobby;


    private void Awake() {
        // GetComponent<Button>().onClick.AddListener(() => {
        //     GameLobbyManager.Instance.JoinLobby(lobby);
        // });
    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        stageText.text = lobby.Data[GameLobbyManager.KEY_STAGE_ID].Value;;
    }


}