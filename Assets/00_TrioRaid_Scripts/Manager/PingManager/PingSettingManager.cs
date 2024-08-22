using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PingSettingManager : MonoBehaviour
{
    [SerializeField]Toggle toggleTextChat;
    private void Awake() {
        toggleTextChat.onValueChanged.AddListener (PingMenuManager.Instance.ActivePingServerRpc) ;
    }
    
    void Start()
    {
        // PingMenuManager.Instance.ActivePingServerRpc(toggleTextChat.isOn);
    }
}
