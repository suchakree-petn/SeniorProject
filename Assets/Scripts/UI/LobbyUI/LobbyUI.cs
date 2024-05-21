using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{


    // [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    [SerializeField] private Transform paperList;
    [SerializeField] private TextMeshProUGUI pageText;
    
    public int lobbyPages = 0;
    double lobbyPageAmount = 1;



    private void Awake()
    {
        // mainMenuButton.onClick.AddListener(() => {
        //     GameLobbyManager.Instance.LeaveLobby();
        //     Loader.Load(Loader.Scene.MainMenuScene);
        // });
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.JoinByCode(joinCodeInputField.text);
        });
        nextButton.onClick.AddListener(() =>
        {
            if(lobbyPages+1<lobbyPageAmount)lobbyPages++;
            GameLobbyManager.Instance.RefreshLobbyList();
        });
        backButton.onClick.AddListener(() =>
        {
            if(lobbyPages+1>lobbyPageAmount)lobbyPages--;
            GameLobbyManager.Instance.RefreshLobbyList();
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerNameInputField.text = GameMultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayerManager.Instance.SetPlayerName(newText);
        });
        GameLobbyManager.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        GameLobbyManager.Instance.OnKickedFromLobby += GameLobbyManager_OnKickedFromLobby;

        // UpdateLobbyList(new List<Lobby>());
    }

    private void GameLobbyManager_OnKickedFromLobby(object sender, GameLobbyManager.LobbyEventArgs e)
    {
        Debug.Log("Load back to lobby");
        if (NetworkManager.Singleton.IsServer)
            GameMultiplayerManager.Instance.GetPlayerDataNetworkList().Clear();
        Loader.Load(Loader.Scene.LobbyScene);
        NetworkManager.Singleton.Shutdown();
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    // private void UpdateLobbyList(List<Lobby> lobbyList)
    // {
    //     foreach (Transform child in lobbyContainer)
    //     {
    //         if (child == lobbyTemplate) continue;
    //         Destroy(child.gameObject);
    //     }

    //     foreach (Lobby lobby in lobbyList)
    //     {
    //         Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
    //         lobbyTransform.gameObject.SetActive(true);
    //         lobbyTransform.GetComponent<LobbyListSingleUI>().UpdateLobby(lobby);
    //         lobbyTransform.GetChild(3).GetComponent<ShowClassUI>().UpdateLobby(lobby);
    //     }
    // }

    private void UpdateStatusPage(List<Lobby> lobbyList){
        lobbyPageAmount = Math.Ceiling(lobbyList.Count/6.0f);
        if(lobbyPageAmount==0)lobbyPageAmount=1;
        pageText.text = lobbyPages+1 +"/"+lobbyPageAmount;
        
        if(lobbyPages==0){
            backButton.interactable = false;
        }else{
            backButton.interactable = true;
        }
        if(lobbyPages+1>=lobbyPageAmount){
            nextButton.interactable = false;
        }else{
            nextButton.interactable = true;
        }
    }
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        UpdateStatusPage(lobbyList);
        foreach (Transform child in paperList)
        {
            if (child == paperList) continue;
            child.gameObject.SetActive(false);
        }

        int lobbyShowAmount = lobbyPages*6;

        for (int i = lobbyShowAmount;i<lobbyList.Count;i++){
            GameObject paperGameObject;
            for(int j = 0;j<6;j++){
                if(!paperList.GetChild(j).gameObject.active){
                    paperGameObject = paperList.GetChild(j).gameObject;
                    paperGameObject.gameObject.SetActive(true);
                    paperGameObject.GetComponent<LobbyListSingleUI>().UpdateLobby(lobbyList[i]);
                    paperGameObject.transform.GetChild(4).GetComponent<ShowClassUI>().UpdateLobby(lobbyList[i]);
                    break;
                }
            }

        }

    }

    private void OnDestroy()
    {
        GameLobbyManager.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
    }

}