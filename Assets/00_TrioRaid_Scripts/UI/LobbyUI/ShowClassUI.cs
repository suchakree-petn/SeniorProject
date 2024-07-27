using UnityEngine;
using Unity.Services.Lobbies.Models;
public class ShowClassUI : MonoBehaviour
{
    [SerializeField] private GameObject selectTank;
    [SerializeField] private GameObject selectArcher;
    [SerializeField] private GameObject selectCaster;


    public void UpdateLobby(Lobby lobby) {
        selectTank.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_TANK_ID].Value));
        selectArcher.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_ARCHER_ID].Value));
        selectCaster.SetActive(bool.Parse(lobby.Data[GameLobbyManager.KEY_CASTER_ID].Value));
    }
}
