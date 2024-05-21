using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour {

    
    // [SerializeField] private TextMeshProUGUI lobbyNameText;
    // [SerializeField] private TextMeshProUGUI playersText;
    // [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshPro lobbyNameText;
    [SerializeField] private TextMeshPro playersText;
    [SerializeField] private TextMeshPro stageText;


    private Lobby lobby;


    private void Awake() {

    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = "Member: "+ lobby.Players.Count + "/" + lobby.MaxPlayers;
        stageText.text = "Map: "+ lobby.Data[GameLobbyManager.KEY_STAGE_ID].Value;
    }
    public void JoinLobby(){
        GameLobbyManager.Instance.JoinWithId(lobby.Id);
    }


}