using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour {


    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private GameObject playerVisual;
    // [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;


    private void Awake() {
        // kickButton.onClick.AddListener(() => {
        //     PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
        //     GameLobbyManager.Instance.KickPlayer(playerData.playerId.ToString());
        //     GameMultiplayerManager  .Instance.KickPlayer(playerData.clientId);
        // });
    }

    private void Start() {
        GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        // kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }

    private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e) {
        UpdatePlayer();
    }
    [Command]
    public void UpdatePlayer() {
        if (GameMultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex)) {
            Show();

            PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            // readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            readyGameObject.transform.GetChild(0).GetComponent<MeshRenderer> ().material = GameMultiplayerManager.Instance.GetMaterial(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();
            // // foreach (GameObject child in playerVisual.transform){
            // //     Destroy(child);
            // // }
            // Destroy(playerVisual.transform.GetChild(0).gameObject);

            // GameObject characterIdle = GameMultiplayerManager.Instance.GetPlayerClass(playerData.classId);
            for(int i = 0;i<3;i++){
                GameObject classSelect = playerVisual.transform.GetChild(i).gameObject;
                classSelect.SetActive(false);
                if(playerData.classId == i){
                    classSelect.SetActive(true);
                }
            }
            // Instantiate(characterIdle,playerVisual.transform);

        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        GameMultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
    }


}