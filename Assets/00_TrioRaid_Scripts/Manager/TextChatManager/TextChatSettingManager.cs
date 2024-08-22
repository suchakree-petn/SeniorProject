using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChatSettingManager : MonoBehaviour
{
    [SerializeField]Toggle toggleTextChat;
    private void Awake() {
    }
    
    void Start()
    {
        toggleTextChat.onValueChanged.AddListener (ChatManager.Instance.ActiveTextChatServerRpc) ;
        ChatManager.Instance.ActiveTextChatServerRpc(toggleTextChat.isOn);
    }
}
