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
    [SerializeField] private List<string> mapsName;



    private void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false, mapsName[gameStage.value]);
            Hide();
        });
        createPrivateButton.onClick.AddListener(() =>
        {
            GameLobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true, mapsName[gameStage.value]);
            Hide();
        });
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        AddNewMapInDropDown();
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
    void AddNewMapInDropDown(){
        foreach(string mapName in mapsName){
            gameStage.options.Add(new (TranslateMapName(mapName), null));
        }
    }
    string TranslateMapName(string mapName){
        string stage = mapName.Split('_')[2];
        string name = mapName.Split('_')[3];
        
        return stage + " " + name;
    }
}

