using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ChatManager : NetworkSingleton<ChatManager>
{
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    public string playerName;

    protected override void InitAfterAwake(){}
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)){
            chatInput.Select();
            SendChatManager(chatInput.text, playerName);
            chatInput.text = "";
        }
    }
    void SendChatManager(string _message, string _fromWho = null){
        if(string.IsNullOrWhiteSpace(_message))return;

        string _package = _fromWho + " : " + _message;
        SendChatManagerServerRpc(_package);
    }
    void AddMessage(string msg){
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
    }
    [ServerRpc(RequireOwnership = false)]
    void SendChatManagerServerRpc(string message){
        ReceiveChatMessageClientRpc(message);
    }
    [ClientRpc]
    void ReceiveChatMessageClientRpc(string message){
        ChatManager.Instance.AddMessage(message);
    }
}
