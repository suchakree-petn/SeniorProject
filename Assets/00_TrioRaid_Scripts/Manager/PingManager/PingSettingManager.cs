using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PingSettingManager : MonoBehaviour
{
    [SerializeField]Toggle togglePing;
    private void Awake() {
        if(NetworkManager.Singleton.IsServer){
            PingMenuManager.Instance.isActive.Value = false;
        }
        togglePing.isOn = PingMenuManager.Instance.gameObject.activeSelf;
        togglePing.onValueChanged.AddListener (PingMenuManager.Instance.ActivePingServerRpc) ;
    }
    
    private void Update() {
        togglePing.isOn = PingMenuManager.Instance.isActive.Value;
    }
}
