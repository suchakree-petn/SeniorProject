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



    private void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false, gameStage.value + 1);
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true, gameStage.value + 1);
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

