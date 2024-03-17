using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : NetworkSingleton<PlayerUIManager>
{
    [Header("Reference")]
    public GameObject PlayerCanvas;
    [SerializeField] private GameObject crossHair;
    [Header("Reference select UI")]
    [SerializeField] private GameObject selectCharacterMenu;
    [SerializeField] private TMP_Dropdown selectCharacterDropdown;
    [SerializeField] private Button selectCharacterButton_Host;
    [SerializeField] private Button selectCharacterButton_Client;

    public void SetSelectCharacter(ulong clientId)
    {
        Debug.Log("Dropdown Value: " + selectCharacterDropdown.value);
        Debug.Log("Id Value: " + clientId);
        ulong PLAYER_CHAR_ID;
        switch (selectCharacterDropdown.value)
        {
            case 0:
                PLAYER_CHAR_ID = 1001;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            case 1:
                PLAYER_CHAR_ID = 1002;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            case 2:
                PLAYER_CHAR_ID = 1003;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
            default:
                PLAYER_CHAR_ID = 1000;
                PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(PLAYER_CHAR_ID);
                break;
        }
        // PlayerManager.Instance.SwitchPlayerCharacter_ServerRpc(UserManager.Instance.UserData.PlayerCharacterId);
    }
    protected override void InitAfterAwake()
    {
        selectCharacterButton_Host.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        selectCharacterButton_Client.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

    public override void OnNetworkSpawn()
    {
        PlayerManager.Instance.OnAfterClientConnect += SetSelectCharacter;

    }



    public override void OnNetworkDespawn()
    {

        PlayerManager.Instance.OnAfterClientConnect -= SetSelectCharacter;

    }
    public void SetPlayerCrossHairState(bool active)
    {
        crossHair.SetActive(active);
    }

}
