using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TextChatSettingManager : MonoBehaviour
{
    [SerializeField]Toggle toggleTextChat;
    private void Awake() {
        if(NetworkManager.Singleton.IsServer){
            ChatManager.Instance.isActive.Value = false;
        }
        toggleTextChat.isOn = ChatManager.Instance.gameObject.activeSelf;
        toggleTextChat.onValueChanged.AddListener (ChatManager.Instance.ActiveTextChatServerRpc) ;
    }

    private void Update() {
        toggleTextChat.isOn = ChatManager.Instance.isActive.Value;
    }

}
