using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(BackToMenu);
    }

    private void BackToMenu()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
        PlayerUIManager.Instance.HideResultUI();
        Debug.Log("ResultUI");
        Destroy(GameMultiplayerManager.Instance.gameObject);
        Loader.Load(Loader.Scene.LobbyScene);

    }

}
