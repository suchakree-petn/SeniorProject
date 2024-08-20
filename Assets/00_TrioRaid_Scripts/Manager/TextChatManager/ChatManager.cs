using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using UnityEngine.UI;

public class ChatManager : NetworkSingleton<ChatManager>
{
    [SerializeField] ChatMessage chatMessagePrefab;
    [SerializeField] CanvasGroup chatContent;
    [SerializeField] TMP_InputField chatInput;
    [SerializeField] GameObject bgChat;
    [SerializeField] GameObject rootScrollbar;
    [SerializeField] GameObject childScrollbar;
    [SerializeField] GameObject chatInputGameObject;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] string toggleTagName = "ToggleTextChat";
    [SerializeField] Toggle toggleActive{
        get
        {
            return GameObject.FindGameObjectWithTag(toggleTagName).GetComponent<Toggle>();
        }
    }
    public bool isUsingChat = false;
    public Action OnOpenChat;
    public Action OnCloseChat;
    protected override void InitAfterAwake() { }
    private void Start()
    {
        OnOpenChat += OpenChat;
        OnOpenChat += () => { SetShowChatUI(true); };
        OnCloseChat += CloseChat;
        OnCloseChat += () => { SetShowChatUI(false); };
        SetShowChatUI(false);
    }
    void Update()
    {
        if (PlayerManager.Instance != null)
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
                chatInput.text = string.Empty;
                chatInput.Select();
                chatInput.ActivateInputField();
                Debug.Log("isUsingChat");
            }
        }

    }
    IEnumerator AutoScrollChat(){
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0);
    }
    void SetShowChatUI(bool show)
    {
        bgChat.SetActive(show);
        rootScrollbar.GetComponent<Image>().enabled = show;
        childScrollbar.SetActive(show);
        chatInputGameObject.SetActive(show);
        if (show){
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    }
    void OpenChat()
    {
        isUsingChat = true;
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

        SendChatManagerServerRpc(_message, clientId);
    }
    void AddMessage(string msg)
    {
        ChatMessage CM = Instantiate(chatMessagePrefab, chatContent.transform);
        CM.SetText(msg);
        bool doAutoScroll = scrollRect.normalizedPosition.y < 0.001f;
        if (doAutoScroll){
            StartCoroutine(AutoScrollChat());
        }
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
    [ServerRpc(RequireOwnership = false)]
    public void ActiveTextChatServerRpc(bool active)
    {
        ActiveTextChatClientRpc(active);
    }
    [ClientRpc]
    void ActiveTextChatClientRpc(bool active)
    {
        toggleActive.isOn = active;
        ChatManager.Instance.gameObject.SetActive(active);
    }
}
