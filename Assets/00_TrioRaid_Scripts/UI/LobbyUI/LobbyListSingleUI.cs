using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro lobbyNameText;
    [SerializeField] private TextMeshPro playersText;
    [SerializeField] private TextMeshPro stageText;


    private Lobby lobby;


    private void Awake()
    {

    }

    public void UpdateLobby(Lobby lobby)
    {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = "Member: " + lobby.Players.Count + "/" + lobby.MaxPlayers;
        stageText.text = TranslateMapName(lobby.Data[GameLobbyManager.KEY_STAGE_ID].Value);
    }
    public void JoinLobby()
    {
        GameLobbyManager.Instance.JoinWithId(lobby.Id);
    }

    private void OnMouseDown()
    {
        JoinLobby();
    }

    string TranslateMapName(string mapName){
        string stage = mapName.Split('_')[2];
        string name = mapName.Split('_')[3];
        
        return stage + " " + name;
    }
}