using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{


    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            if (GameLobbyManager.Instance.IsLobbyHost())
            {
                GameLobbyManager.Instance.DeleteLobby();
            }
            else
            {
                GameLobbyManager.Instance.LeaveLobby();
            }

        });
        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
            readyButton.interactable = false;
        });
    }

}