using TMPro;
using UnityEngine;

public class LobbyCodeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyCode;
    void Start()
    {
        lobbyCode.text = "Code: " + GameLobbyManager.Instance.GetLobby().LobbyCode;

    }
}
