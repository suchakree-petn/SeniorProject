using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using JetBrains.Annotations;
public class ShowClassUI : MonoBehaviour
{
    [SerializeField] private GameObject selectTank;
    [SerializeField] private GameObject selectArcher;
    [SerializeField] private GameObject selectCaster;

    private Lobby lobby;


    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;
        Debug.Log(lobby.Data[GameLobbyManager.KEY_TANK_ID].Value);
        if(bool.Parse(lobby.Data[GameLobbyManager.KEY_TANK_ID].Value)){
            Debug.Log("It trueeeeeeee");
        }else{
            Debug.Log("Cant parse");
        }
        selectTank.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_TANK_ID].Value));
        selectArcher.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_ARCHER_ID].Value));
        selectCaster.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_CASTER_ID].Value));
    }
}
