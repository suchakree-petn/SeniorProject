using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {



    private void Start() {
        GameMultiplayerManager.Instance.OnTryingToJoinGame += GameManager_OnFailedToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayer_OnTryingToJoinGame;

        Hide();
    }

    private void GameManager_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        GameMultiplayerManager.Instance.OnTryingToJoinGame -= GameManager_OnFailedToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayer_OnTryingToJoinGame;
    }

}