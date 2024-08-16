using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class LobbyCreateUI : MonoBehaviour
{


    [SerializeField] private Button closeButton;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_Dropdown gameStage;
    [SerializeField] private List<string> mapName;



    private void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false, mapName[gameStage.value]);
            Hide();
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true, mapName[gameStage.value]);
            Hide();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        createPublicButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

}

