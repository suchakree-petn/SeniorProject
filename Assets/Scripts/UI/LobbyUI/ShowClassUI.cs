using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
public class ShowClassUI : MonoBehaviour
{
    [SerializeField] private GameObject selectTank;
    [SerializeField] private GameObject selectArcher;
    [SerializeField] private GameObject selectCaster;
    // [SerializeField] private TextMeshProUGUI playersText;
    // [SerializeField] private TextMeshProUGUI stageText;


    private Lobby lobby;


    // private void Awake() {
    //     GetComponent<Button>().onClick.AddListener(() => {
    //         GameLobbyManager.Instance.JoinWithId(lobby.Id);
    //         GetComponent<Button>().Select();
    //     });
    // }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        selectTank.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_TANK_ID].Value));
        selectArcher.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_ARCHER_ID].Value));
        selectCaster.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_CASTER_ID].Value));
    }
}
