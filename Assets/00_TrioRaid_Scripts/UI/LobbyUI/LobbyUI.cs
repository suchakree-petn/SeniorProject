using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class LobbyUI : MonoBehaviour
{


    // [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform paperList;
    [SerializeField] private TextMeshProUGUI pageText;

    public int lobbyPages = 0;
    double lobbyPageAmount = 1;

    public void SetActiveUILobby(bool active)
    {
        if (active)
        {
            UnFadeButton(mainMenuButton);
            UnFadeButton(createLobbyButton);
            UnFadeButton(quickJoinButton);
            UnFadeButton(joinCodeButton);
            UnFadeButton(nextButton);
            UnFadeButton(backButton);
            UnFadeInputField(joinCodeInputField);
            UnFadeInputField(playerNameInputField);
            pageText.DOFade(1, 0.3f);
        }
        else
        {
            FadeButton(mainMenuButton);
            FadeButton(createLobbyButton);
            FadeButton(quickJoinButton);
            FadeButton(joinCodeButton);
            FadeButton(nextButton);
            FadeButton(backButton);
            FadeInputField(joinCodeInputField);
            FadeInputField(playerNameInputField);
            pageText.DOFade(0, 0.3f);
        }
        // mainMenuButton.gameObject.SetActive(active);
        // createLobbyButton.gameObject.SetActive(active);
        // quickJoinButton.gameObject.SetActive(active);
        // joinCodeButton.gameObject.SetActive(active);
        // nextButton.gameObject.SetActive(active);
        // backButton.gameObject.SetActive(active);
        // joinCodeInputField.gameObject.SetActive(active);
        // playerNameInputField.gameObject.SetActive(active);
        // pageText.gameObject.SetActive(active);
    }

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
            if (lobbyPages + 1 < lobbyPageAmount) lobbyPages++;
            GameLobbyManager.Instance.RefreshLobbyList();
        });
        backButton.onClick.AddListener(() =>
        {
            if (lobbyPages + 1 > lobbyPageAmount) lobbyPages--;
            GameLobbyManager.Instance.RefreshLobbyList();
        });

        SetActiveUILobby(false);
        Debug.Log("setfalseUIlobby");
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
    private void UnFadeButton(Button button)
    {
        button.interactable = true;
        button.GetComponent<Image>().DOFade(1, 0.3f);
        if (button.GetComponentInChildren<TextMeshProUGUI>() != null) button.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.3f);
    }

    private void FadeButton(Button button)
    {
        button.interactable = false;
        button.GetComponent<Image>().DOFade(0, 0.3f);
        if (button.GetComponentInChildren<TextMeshProUGUI>() != null) button.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.3f);
    }

    private void UnFadeInputField(TMP_InputField inputField)
    {
        inputField.interactable = true;
        inputField.GetComponent<Image>().DOFade(1, 0.3f);
        inputField.GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.3f);
    }

    private void FadeInputField(TMP_InputField inputField)
    {
        inputField.interactable = false;
        inputField.GetComponent<Image>().DOFade(0, 0.3f);
        inputField.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, 0.3f);
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

    private void UpdateStatusPage(List<Lobby> lobbyList)
    {
        lobbyPageAmount = Math.Ceiling(lobbyList.Count / 6.0f);
        if (lobbyPageAmount == 0) lobbyPageAmount = 1;
        pageText.text = lobbyPages + 1 + "/" + lobbyPageAmount;

        if (lobbyPages == 0)
        {
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }
        if (lobbyPages + 1 >= lobbyPageAmount)
        {
            nextButton.interactable = false;
        }
        else
        {
            nextButton.interactable = true;
        }
    }
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        UpdateStatusPage(lobbyList);
        foreach (Transform child in paperList)
        {
            child.gameObject.SetActive(false);
        }

        int lobbyShowAmount = lobbyPages * 6;

        for (int i = lobbyShowAmount; i < lobbyList.Count; i++)
        {
            GameObject paperGameObject;
            for (int j = 0; j < 6; j++)
            {
                if (!paperList.GetChild(j).gameObject.activeSelf)
                {
                    paperGameObject = paperList.GetChild(j).gameObject;
                    paperGameObject.gameObject.SetActive(true);
                    paperGameObject.GetComponent<LobbyListSingleUI>().UpdateLobby(lobbyList[i]);
                    paperGameObject.GetComponentInChildren<ShowClassUI>().UpdateLobby(lobbyList[i]);
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