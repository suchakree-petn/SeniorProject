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
            toggleTextChat.onValueChanged.AddListener (ChatManager.Instance.ActiveTextChatServerRpc) ;
        }
        toggleTextChat.isOn = ChatManager.Instance.isActive.Value;
    }

    private void Update() {
        toggleTextChat.isOn = ChatManager.Instance.isActive.Value;
    }

}
