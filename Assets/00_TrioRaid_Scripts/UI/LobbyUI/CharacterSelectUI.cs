using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                BackToMenu();
            }
            else
            {
                GameLobbyManager.Instance.LeaveLobby();
                BackToMenu();
            }

        });
        readyButton.onClick.AddListener(() =>
        {
            CharacterSelectReady.Instance.SetPlayerReady();
            readyButton.interactable = false;
        });
    }
    private void BackToMenu()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        Destroy(GameMultiplayerManager.Instance.gameObject);
        SceneManager.LoadScene("Thanva_MainMenu_UserDataPersistence");

    }
}