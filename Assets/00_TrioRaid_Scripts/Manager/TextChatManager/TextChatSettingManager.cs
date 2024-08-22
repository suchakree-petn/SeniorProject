using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChatSettingManager : MonoBehaviour
{
    [SerializeField]Toggle toggleTextChat;
    private void Awake() {
        toggleTextChat.onValueChanged.AddListener (ChatManager.Instance.ActiveTextChatServerRpc) ;
    }
    
    void  Start()
    {
        // ChatManager.Instance.ActiveTextChatServerRpc(toggleTextChat.isOn);
    }
}
