using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class ChatManager : NetworkSingleton<ChatManager>
{
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    public bool isUsingChat = false;
    public Action OnOpenChat;
    public Action OnCloseChat;
    protected override void InitAfterAwake() { }
    private void Start()
    {
        OnOpenChat += OpenChat;
        OnCloseChat += CloseChat;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && !isUsingChat)
        {
            OnOpenChat.Invoke();
            Debug.Log("!isUsingChat");
        }
        else if (Input.GetKeyDown(KeyCode.Return) && string.IsNullOrEmpty(chatInput.text) && isUsingChat)
        {
            OnCloseChat.Invoke();
            Debug.Log("CloseChat");
        }
        else if (Input.GetKeyDown(KeyCode.Return) && isUsingChat)
        {
                
            SendChatManager(chatInput.text, NetworkManager.LocalClientId);
            chatInput.text = "";
            chatInput.ActivateInputField();
            Debug.Log("isUsingChat");
        }
        
    }
    void OpenChat()
    {
        isUsingChat = true;
        chatInput.Select();
        PlayerManager.Instance.LocalPlayerController.SetCanPlayerMove(false);
        PlayerManager.Instance.LocalPlayerController.SetIsReadyToAttack(false);
        PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityE(false);
        PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityQ(false);
    }
    void CloseChat()
    {
        isUsingChat = false;
        PlayerManager.Instance.LocalPlayerController.SetCanPlayerMove(true);
        PlayerManager.Instance.LocalPlayerController.SetIsReadyToAttack(true);
        PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityE(true);
        PlayerManager.Instance.LocalPlayerController.SetCanUseAbilityQ(true);
    }
    void SendChatManager(string _message, ulong clientId)
    {
        if (string.IsNullOrWhiteSpace(_message)) return;

        SendChatManagerServerRpc(_message,clientId);
    }
    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }
    [ServerRpc(RequireOwnership = false)]
    void SendChatManagerServerRpc(string message, ulong clientId)
    {
        PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromClientId(clientId);
        ReceiveChatMessageClientRpc(message, playerData.playerName.ToString());
    }
    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message, string playerName)
    {
        string package = playerName + " : " + message;
        AddMessage(package);
    }
}
