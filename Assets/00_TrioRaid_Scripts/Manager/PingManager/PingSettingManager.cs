using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingSettingManager : MonoBehaviour
{
    [SerializeField]Toggle toggleTextChat;
    private void Awake() {
    }
    
    void Start()
    {
        toggleTextChat.onValueChanged.AddListener (PingMenuManager.Instance.ActivePingServerRpc) ;
        PingMenuManager.Instance.ActivePingServerRpc(toggleTextChat.isOn);
    }
}
